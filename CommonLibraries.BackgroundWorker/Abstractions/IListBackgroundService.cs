using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibraries.BackgroundWorker.Abstractions
{
    public interface IListBackgroundService<TItem>
    {
        Task<List<TItem>> GetItemsAsync();

        Task ProcessItemAsync(TItem item, CancellationToken stoppinngToken);
    }
}
