namespace DataClient.InfluxDb.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string InfluxDbUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DbName { get; set; }
    }
}