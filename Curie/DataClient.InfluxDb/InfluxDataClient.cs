using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataClient.InfluxDb.Settings;
using DataHandling.Core.Entities;
using DataHandling.Core.Interfaces;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using Serilog;

namespace DataClient.InfluxDb
{
    public class InfluxDataClient : IDataClient
    {
        private readonly IInfluxDbClient _influxDbClient;
        private readonly string _dbName;

        public InfluxDataClient(string influxDbUrl, string userName, string password, string dbName)
        {
            _influxDbClient = new InfluxDbClient(influxDbUrl, userName, password, InfluxDbVersion.Latest);
            _dbName = dbName;
        }

        public InfluxDataClient(IDatabaseSettings settings)
        {
            _influxDbClient = new InfluxDbClient(settings.InfluxDbUrl, settings.UserName, settings.Password, InfluxDbVersion.Latest);
            _dbName = settings.DbName;
        }

        public async Task<bool> SendData(Co2Reading value)
        {
            if (value.IsSuccess)
            {
                Log.Debug($"Sending '{value.ToNiceString()}' to the database...");
                var pointToWrite = AssembleData(value);
                try
                {
                    var response = await _influxDbClient.Client.WriteAsync(pointToWrite, _dbName);
                    return response.Success;
                }
                catch (Exception ex)
                {
                    Log.Error($"An exception occurred while sending data to the database. {ex}");
                    return false;
                }
            }
            else
            {
                Log.Debug($"Data '{value.ToNiceString()}' will not be sent to the database.");
                return false;
            }
        }

        private Point AssembleData(Co2Reading value)
        {
            var pointToWrite = new Point
            {
                Name = "CO2",       // serie/measurement/table to write into
                Tags = new Dictionary<string, object>
                {
                    {"Room", "B09"}
                },
                Fields = new Dictionary<string, object>
                {
                    {"Co2Level", value.Co2Level},
                    {"Temperature", value.Temperature}
                },
                Timestamp = DateTime.UtcNow
                //Timestamp = DateTime.Today + value.Time // optional (can be set to any DateTime moment)
            };

            return pointToWrite;
        }
    }
}