using System.Threading.Tasks;

namespace FlickrOffloadr.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
    }
}