namespace DataClient.InfluxDb.Settings
{
    public interface IDatabaseSettings
    {
        string InfluxDbUrl { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string DbName { get; set; }
    }
}