using System.Threading.Tasks;
using DataHandling.Core.Entities;

namespace DataHandling.Core.Interfaces
{
    public interface IDataClient
    {
        Task<bool> SendData(Co2Reading value);
    }
}