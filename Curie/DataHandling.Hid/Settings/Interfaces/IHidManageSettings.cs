using System;

namespace DataHandling.Hid.Settings
{
    public interface IHidManageSettings
    {
        int DeviceVendorId { get; }
        int DeviceProductId { get; }
        TimeSpan UpdatePeriod { get; }
    }
}