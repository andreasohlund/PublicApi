using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace PublicAPI.Tests
{
    public class TestCollector<T>:IAsyncCollector<T>
    {
        public TestCollector()
        {
            Items = new List<T>();
        }

        public ICollection<T> Items { get; }

        public Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            Items.Add(item);

            return Task.CompletedTask;
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
