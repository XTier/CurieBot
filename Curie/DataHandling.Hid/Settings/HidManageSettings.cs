using System;

namespace DataHandling.Hid.Settings
{
    public class HidManageSettings : IHidManageSettings
    {
        public int DeviceVendorId { get; set; }
        public int DeviceProductId { get; set; }
        public TimeSpan UpdatePeriod { get; set; }
    }
}