using CurieBot.Net.Enums;
using Data.Core.Interfaces;

namespace CurieBot.Net.Interfaces
{
    public interface IDataProviderFactory
    {
        IDataProvider CreateDataProvider(DataProviderType type);
    }
}