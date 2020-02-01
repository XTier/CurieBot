using System;

namespace Data.Hid.Core.Settings
{
    public class HidManageSettings
    {
        public int DeviceVendorId { get; set; }
        public int DeviceProductId { get; set; }
        public TimeSpan UpdatePeriod { get; set; }
    }
}