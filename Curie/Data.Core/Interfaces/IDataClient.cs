using System.Threading.Tasks;
using Data.Core.Entities;

namespace Data.Core.Interfaces
{
    public interface IDataClient
    {
        Task<bool> SendData(Co2Reading value);
    }
}