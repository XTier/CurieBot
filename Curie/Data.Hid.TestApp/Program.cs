using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Hid.Core;
using Data.Hid.Core.Settings;
using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Encodings;
using HidSharp.Reports.Input;

namespace Data.Hid.TestApp
{
    // TODO AV: mesurements -> CombineLatest -> Sample
    // TODO AV: just use function Sample and that's it eh???
    public class MeasurementFilter
    {
        private readonly TimeSpan _interval;

        public MeasurementFilter(TimeSpan interval)
        {
            _interval = interval;
        }

        IObservable<Measurement> Reduce(IObservable<Measurement> input) // TODO AV: generic?
        {
            return input.Sample(_interval);
        }
    }

    public abstract class Reading
    {
        public DateTimeOffset Time { get; set; }

        protected Reading(DateTimeOffset time)
        {
            Time = time;
        }
    }

    public class TemperatureReading : Reading
    {
        public double Temperature { get; }

        public TemperatureReading(DateTimeOffset time, double temperature) : base(time)
        {
            Temperature = temperature;
        }
    }

    public class Co2Reading : Reading
    {
        public int Co2Level { get; }

        public Co2Reading(DateTimeOffset time, int co2Level) : base(time)
        {
            Co2Level = co2Level;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // 1
            NormalTest();

            // 2
            //DebugTest();

            // 3
            //await OldTest();
        }

        private static void NormalTest()
        {
            var settings = new HidManageSettings { DeviceVendorId = 1241, DeviceProductId = 41042 };

            var hidReader = new HidReader(settings);
            var decryptor = new MeasurementDecryptor();

            hidReader
                .Read()
                .Select(data => decryptor.RetrieveMeasurement(data))
                .Where(m => m.Success)
                .Subscribe(m => Console.WriteLine(m.Output), () => Console.WriteLine("End"));
        }

        private static void DebugTest()
        {
            var devices = DeviceList.Local.GetHidDevices().ToArray();
            var co2Device = devices.First(d => d.VendorID == 1241 && d.ProductID == 41042);
            var reportDescriptor = co2Device.GetReportDescriptor();

            var measurementDecryptor = new MeasurementDecryptor();


            var rawReportDescriptor = co2Device.GetRawReportDescriptor();
            Console.WriteLine("Report Descriptor:");
            Console.WriteLine("  {0} ({1} bytes)", string.Join(" ", rawReportDescriptor.Select(d => d.ToString("X2"))), rawReportDescriptor.Length);

            int indent = 0;
            foreach (var element in EncodedItem.DecodeItems(rawReportDescriptor, 0, rawReportDescriptor.Length))
            {
                if (element.ItemType == ItemType.Main && element.TagForMain == MainItemTag.EndCollection) { indent -= 2; }

                Console.WriteLine("  {0}{1}", new string(' ', indent), element);

                if (element.ItemType == ItemType.Main && element.TagForMain == MainItemTag.Collection) { indent += 2; }
            }

            // Lengths should match.
            Debug.Assert(co2Device.GetMaxInputReportLength() == reportDescriptor.MaxInputReportLength);
            Debug.Assert(co2Device.GetMaxOutputReportLength() == reportDescriptor.MaxOutputReportLength);
            Debug.Assert(co2Device.GetMaxFeatureReportLength() == reportDescriptor.MaxFeatureReportLength);

            foreach (var deviceItem in reportDescriptor.DeviceItems)
            {
                foreach (var usage in deviceItem.Usages.GetAllValues())
                {
                    Console.WriteLine($"Usage: {usage:X4} {(Usage) usage}");
                }
                foreach (var report in deviceItem.Reports)
                {
                    Console.WriteLine(
                        $"{report.ReportType}: ReportID={report.ReportID}, Length={report.Length}, Items={report.DataItems.Count}");
                    foreach (var dataItem in report.DataItems)
                    {
                        Console.WriteLine(
                            $"  {dataItem.ElementCount} Elements x {dataItem.ElementBits} Bits, Units: {dataItem.Unit.System}, Expected Usage Type: {dataItem.ExpectedUsageType}, Flags: {dataItem.Flags}, Usages: {string.Join(", ", dataItem.Usages.GetAllValues().Select(usage => usage.ToString("X4") + " " + ((Usage) usage).ToString()))}");
                    }
                }

                {
                    Console.WriteLine("Opening device for 20 seconds...");

                    HidStream hidStream;
                    if (co2Device.TryOpen(out hidStream))
                    {
                        Console.WriteLine("Opened device.");
                        hidStream.ReadTimeout = Timeout.Infinite;

                        using (hidStream)
                        {
                            var inputReportBuffer = new byte[co2Device.GetMaxInputReportLength()];
                            var inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
                            var inputParser = deviceItem.CreateDeviceItemInputParser();

                            inputReceiver.Start(hidStream);

                            while (true)
                            {
                                if (inputReceiver.WaitHandle.WaitOne(1000))
                                {
                                    if (!inputReceiver.IsRunning)
                                    {
                                        break;
                                    } // Disconnected?

                                    while (inputReceiver.TryRead(inputReportBuffer, 0, out var report))
                                    {
                                        // Parse the report if possible.
                                        // This will return false if (for example) the report applies to a different DeviceItem.
                                        if (inputParser.TryParseReport(inputReportBuffer, 0, report))
                                        {
                                            var output = measurementDecryptor.RetrieveMeasurement(inputReportBuffer).Output;
                                            Console.WriteLine(output);

                                            WriteDeviceItemInputParserResult(inputParser);
                                        }
                                    }

                                    var data = hidStream.Read();
                                    Console.WriteLine(measurementDecryptor.RetrieveMeasurement(data).Output);
                                }
                            }
                        }
                    }
                }
            }

            void WriteDeviceItemInputParserResult(DeviceItemInputParser parser)
            {
                while (parser.HasChanged)
                {
                    int changedIndex = parser.GetNextChangedIndex();
                    var previousDataValue = parser.GetPreviousValue(changedIndex);
                    var dataValue = parser.GetValue(changedIndex);

                    Console.WriteLine(
                        $"  {(Usage) dataValue.Usages.FirstOrDefault()}: {previousDataValue.GetPhysicalValue()} -> {dataValue.GetPhysicalValue()}");
                }
            }
        }

        private static async Task OldTest()
        {
            var devices = DeviceList.Local.GetHidDevices().ToArray();
            Console.WriteLine("All devices:");
            foreach (var device in devices)
            {
                //Console.WriteLine(device + " @ " + device.DevicePath);
                Console.WriteLine(device);
            }

            var co2Device = devices.First(d => d.VendorID == 1241 && d.ProductID == 41042);

            if (co2Device.TryOpen(out var hidStream))
            {
                Console.WriteLine("Opened device.");
                hidStream.ReadTimeout = Timeout.Infinite;

                await using (hidStream)
                {
                    var measurementDecryptor = new MeasurementDecryptor();

                    for (int i = 0; i < 100; i++)
                    {
                        var data = hidStream.Read();
                        Console.WriteLine(measurementDecryptor.RetrieveMeasurement(data).Output);
                    }
                }

                Console.WriteLine("Closed device.");
            }
            else
            {
                Console.WriteLine("Failed to open device.");
            }
        }
    }
}