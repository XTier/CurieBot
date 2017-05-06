using System;

namespace DataHandling.Hid.Settings
{
    public interface IHidManageSettings
    {
        int DeviceVendorId { get; set; }
        int DeviceProductId { get; set; }
        TimeSpan UpdatePeriod { get; set; }
    }
}