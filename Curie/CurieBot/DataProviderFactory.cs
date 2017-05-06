using CurieBot.Enums;
using CurieBot.Interfaces;
using DataHandling.Core.Exceptions;
using DataHandling.Core.Interfaces;
using DataHandling.Csv;
using DataHandling.Csv.Settings;
using DataHandling.Hid;
using DataHandling.Hid.Settings;

namespace CurieBot
{
    public class DataProviderFactory : IDataProviderFactory
    {
        private readonly ICsvManageSettings _csvManageSettings;
        private readonly IHidManageSettings _hidManageSettings;

        public DataProviderFactory(ICsvManageSettings csvManageSettings, IHidManageSettings hidManageSettings)
        {
            _csvManageSettings = csvManageSettings;
            _hidManageSettings = hidManageSettings;
        }

        public IDataProvider CreateDataProvider(DataProviderType type)
        {
            switch (type)
            {
                case DataProviderType.LogFile:
                    return new TimedCsvDataProvider(_csvManageSettings);
                    break;
                case DataProviderType.Device:
                    return new HidDataProvider(_hidManageSettings);
                    break;
                default:
                    throw new CustomException($"A data provider is not defined for type '{type}'");
            }
        }
    }
}