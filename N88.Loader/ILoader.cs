namespace N88.Loader
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILoader
    {
        Task<T> LoadAsync<T>(string key, CancellationToken token);
        Task<IReadOnlyList<T>> LoadAllAsync<T>(string key, CancellationToken token);
    }
}