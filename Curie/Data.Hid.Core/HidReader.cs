using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Data.Hid.Core.Settings;
using HidSharp;
using HidSharp.Reports;

namespace Data.Hid.Core
{
    public class HidReader
    {
        private readonly HidDevice _device;
        private readonly ReportDescriptor _reportDescriptor;
        private readonly DeviceItem _deviceItem;
        private readonly int _readTimeoutSeconds = 10; // TODO AV: config?

        public HidReader(HidManageSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            var vendorId = settings.DeviceVendorId;
            var productId = settings.DeviceProductId;

            _device = DeviceList.Local.GetHidDevices().FirstOrDefault(d => d.VendorID == vendorId && d.ProductID == productId);

            if (_device is null)
                throw new InvalidOperationException(
                    $"No device found with vendor id = '{vendorId}', product id = '{productId}'");

            _reportDescriptor = _device.GetReportDescriptor();
            _deviceItem = _reportDescriptor.DeviceItems.Single();
        }

        public IObservable<byte[]> Read() => Observable.Using(() => _device.Open(),
            hidStream => EnumerateReads(hidStream).ToObservable());

        private IEnumerable<byte[]> EnumerateReads(HidStream hidStream)
        {
            var inputReportBuffer = new byte[_device.GetMaxInputReportLength()];
            var inputReceiver = _reportDescriptor.CreateHidDeviceInputReceiver();
            var inputParser = _deviceItem.CreateDeviceItemInputParser();

            inputReceiver.Start(hidStream);

            while (true)
            {
                if (inputReceiver.WaitHandle.WaitOne(_readTimeoutSeconds))
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
                            yield return inputReportBuffer.ToArray();
                        }
                    }
                }
            }
        }
    }
}