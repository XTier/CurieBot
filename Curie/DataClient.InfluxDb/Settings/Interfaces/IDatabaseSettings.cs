namespace DataClient.InfluxDb.Settings
{
    public interface IDatabaseSettings
    {
        string InfluxDbUrl { get; }
        string UserName { get; }
        string Password { get; }
        string DbName { get; }
    }
}