using CurieBot.Enums;
using DataHandling.Core.Interfaces;

namespace CurieBot.Interfaces
{
    public interface IDataProviderFactory
    {
        IDataProvider CreateDataProvider(DataProviderType type);
    }
}