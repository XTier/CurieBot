using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataHandling.Core.Entities;
using DataHandling.Core.Exceptions;
using DataHandling.Core.Interfaces;
using DataHandling.Hid.Settings;
using HidLibrary;
using Serilog;

namespace DataHandling.Hid
{
    /// <summary>
    /// based on code by henryk ploetz
    /// https://hackaday.io/project/5301-reverse-engineering-a-low-cost-usb-co-monitor/log/17909-all-your-base-are-belong-to-us    
    /// </summary>
    public class HidDataProvider : IDataProvider
    {
        private HidDevice _device;
        private readonly int _vendorId;
        private readonly int _productId;
        private readonly TimeSpan _updateTime;
        private readonly ManualResetEvent _event = new ManualResetEvent(false);

        public HidDataProvider(int vendorId, int productId, TimeSpan updateTime)
        {
            _vendorId = vendorId;
            _productId = productId;
            _updateTime = updateTime;
        }

        public HidDataProvider(IHidManageSettings settings)
        {
            _vendorId = settings.DeviceVendorId;
            _productId = settings.DeviceProductId;
            _updateTime = settings.UpdatePeriod;
        }

        private void InitDevice()
        {
            try
            {
                _device = HidDevices.Enumerate(_vendorId, _productId).First();

                _device.OpenDevice();
                _device.WriteFeatureData(HidConstants.Report);

                Log.Information($"USB device '{_device.Description}' connected.");
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to connect to device '{_vendorId}-{_productId}'. {ex}");
            }
        }

        private void CloseDevice()
        {
            _device?.OpenDevice();
            Log.Information($"USB device '{_device?.Description}' dicconnected.");
        }

        private void ReconnectDevice()
        {
            Log.Information("Reconnecting device...");
            CloseDevice();
            InitDevice();
        }

        private async void Work()
        {
            InitDevice();
            while (_event.WaitOne())
            {
                var lastReading = await RetrieveValue();

                NewReadout?.Invoke(lastReading);

                Task.Delay(_updateTime).Wait();

                // if error, try to reconnect before new reading attempt
                if (!lastReading.IsSuccess)
                    ReconnectDevice();
            }
            CloseDevice();
        }

        private async Task<Co2Reading> RetrieveValue()
        {
            var measure = new Measure();
            for (int i = 0; i < HidConstants.ReadLoopCount; i++)
            {
                var read = await ReadFromDevice();
                measure.Apply(read);
            }

            if (measure.Co2Level == null || measure.Temperature == null)
            {
                return Co2Reading.CreateError("No data.");
            }

            return Co2Reading.Create(DateTime.Now.TimeOfDay, measure.Co2Level.Value, measure.Temperature.Value);
        }

        private async Task<Measure> ReadFromDevice()
        {
            try
            {
                var deviceData = await _device.ReadAsync();
                var data = deviceData?.Data?.Skip(1).ToArray();

                if (data == null || data.Length != 8)
                    throw new CustomException("Data from device is missing or corrupt.");

                var decrypted = Decrypt(data);
                var parsed = Parse(decrypted);
                return parsed;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex}");
                return new Measure();
            }
        }

        private static byte[] Decrypt(byte[] data, byte[] key = null)
        {
            key = key ?? HidConstants.Key;
            var cstate = HidConstants.Cstate;
            var shuffle = HidConstants.Shuffle;

            var phase1 = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                byte j = shuffle[i];
                phase1[j] = data[i];
            }

            var phase2 = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                phase2[i] = (byte)(phase1[i] ^ key[i]);
            }

            var phase3 = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                phase3[i] = (byte)(((phase2[i] >> 3) | (phase2[(i - 1 + 8) % 8] << 5)) & 0xff);
            }

            var ctmp = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                ctmp[i] = (byte)(((cstate[i] >> 4) | (cstate[i] << 4)) & 0xff);
            }

            var result = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                result[i] = (byte)((0x100 + phase3[i] - ctmp[i]) & 0xff);
            }

            return result;
        }

        private static Measure Parse(byte[] decrypted)
        {
            var measure = new Measure();
            bool checksum = decrypted[4] == 0x0d && ((byte)decrypted.Take(3).Sum(_ => _) == decrypted[3]);

            if (checksum)
            {
                byte op = decrypted[0];
                int value = decrypted[1] << 8 | decrypted[2];
                switch (op)
                {
                    case HidConstants.Co2:
                        measure.Co2Level = value;
                        break;
                    case HidConstants.Temperature:
                        measure.Temperature = value / 16.0 - 273.15;
                        break;
                }
            }
            return measure;
        }

        public void Init()
        {
            // TODO BUG: multiple inits cause parallel working processes
            _event.Set();
            Task.Run(() => Work());
        }

        public void Stop()
        {
            _event.Reset();
        }

        public event Action<Co2Reading> NewReadout;
    }
}