using System;
using System.Threading;
using DataHandling.Core.Entities;
using DataHandling.Hid;
using DataHandling.Hid.Settings;

namespace Hid.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var hidSettings = new HidManageSettings
            {
                DeviceVendorId = 1241,
                DeviceProductId = 41042,
                UpdatePeriod = TimeSpan.FromSeconds(10)
            };

            var dataProvider = new HidDataProvider(hidSettings);

            dataProvider.NewReadout += Output;

            dataProvider.Init();

            new AutoResetEvent(false).WaitOne();
        }

        private static void Output(Co2Reading reading)
        {
            Console.WriteLine(reading.ToNiceString());
        }
    }
}