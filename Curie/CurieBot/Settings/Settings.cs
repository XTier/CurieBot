using CurieBot.Enums;
using DataClient.InfluxDb.Settings;
using DataHandling.Csv.Settings;
using DataHandling.Hid.Settings;

namespace CurieBot.Settings
{
    public class Settings
    {
        public DatabaseSettings DatabaseSettings { get; set; }
        public SlackSettings SlackSettings { get; set; }
        public CsvManageSettings CsvManageSettings { get; set; }
        public HidManageSettings HidManageSettings { get; set; }
        public DataProviderType DataProviderType { get; set; }
    }
}