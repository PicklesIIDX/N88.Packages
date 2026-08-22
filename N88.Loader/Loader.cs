namespace N88.Loader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic system to handle registering sources implemented to load and deserialize data.
    /// </summary>
    public class Loader : ILoader, IRegister
    {
        private readonly Dictionary<Type, ISourceAdapter> _registry = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="LoadException"></exception>
        public async Task<T> LoadAsync<T>(string key, CancellationToken token)
        {
            var items = await LoadAllAsync<T>(key, token);
            if (items.Count != 1)
            {
                throw new LoadException($"expected exactly 1 result for key '{key}' but loaded '{items.Count}'");
            }

            return items[0];
        }

        public async Task<IReadOnlyList<T>> LoadAllAsync<T>(string key, CancellationToken token)
        {
            if (!_registry.TryGetValue(typeof(T), out var bestAdapter))
            {
                Type? bestType = null;
                foreach (var (registeredType, adapter) in _registry)
                {
                    if (!registeredType.IsAssignableFrom(typeof(T))) { continue; }
                    if(bestType != null && !bestType.IsAssignableFrom(registeredType)) { continue; }
                    bestType = registeredType;
                    bestAdapter = adapter;
                }

                if (bestAdapter == null)
                {
                    throw new RegistrationException($"failed to find resolver for type '{typeof(T).FullName}' in {nameof(_registry)}");
                }
            }
            var items = await bestAdapter.LoadAsync(key, token);
            var itemsOfType = items.OfType<T>();
            var listOfItemsOfType = itemsOfType.ToList();
            return listOfItemsOfType;
        }

        public void Register<T>(ISource<T> source)
        {
            _registry[typeof(T)] 
                = new SourceAdapter<T>(source);
        }

        public void RegisterDecoded<T>(ISource<byte[]> source, IDecoder<T> decoder)
        {
            _registry[typeof(T)] = new SourceAdapter<T>(new DecodedSource<T>(source, decoder));
        }

        /// <summary>
        /// Handles decoding byte arrays from an <see cref="ISource{T}"/> to
        /// type <see cref="T"/> via the given decoder <see cref="IDecoder{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class DecodedSource<T> : ISource<T>
        {
            private readonly ISource<byte[]> _source;
            private readonly IDecoder<T> _decoder;

            public DecodedSource(ISource<byte[]> source, IDecoder<T> decoder)
            {
                _source = source;
                _decoder = decoder;
            }

            public async Task<IReadOnlyList<T>> LoadAsync(string key, CancellationToken token)
            {
                var raw = await _source.LoadAsync(key, token);
                var results = new List<T>(raw.Count);
                foreach (var bytes in raw)
                {
                    var decoded = await _decoder.Decode(bytes);
                    results.Add(decoded);
                }
                return results;
            }
        }

        private class SourceAdapter<T> : ISourceAdapter
        {
            private readonly ISource<T> _inner;
            public SourceAdapter(ISource<T> inner)
            { 
                _inner = inner;
            }
            public async Task<IReadOnlyList<object>> LoadAsync(string key, CancellationToken token)
            {
                var result = await _inner.LoadAsync(key, token);
                var resultObject = result.Cast<object>();
                var resultList = resultObject.ToList();
                return resultList;
            }
        }
        
        /// <summary>
        /// Ensures that when we register we do not have to have a loader
        /// of a generic type, as it is all casted to object.
        /// </summary>
        private interface ISourceAdapter
        {
            Task<IReadOnlyList<object>> LoadAsync(string key, CancellationToken token);
        }
    }
    
    /// <summary>
    /// Interface for implementing how an object of type <see cref="T"/>
    /// will be loaded. Use this if you have a way to load and deserialize
    /// to type <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to load, deserialize, and return.</typeparam>
    public interface ISource<T>
    {
        Task<IReadOnlyList<T>> LoadAsync(string key, CancellationToken token);
    }
    
    /// <summary>
    /// Interface for a reusable deserialization approach ('decoding').
    /// This is used for protocols that can be deserialized to many generic types
    /// but have an intermediary format of bytes.
    /// Examples include raw text, json, yaml, DataTables, etc.
    /// </summary>
    /// <typeparam name="T">The type of the object to load, deserialize, and return.</typeparam>
    public interface IDecoder<T>
    {
        Task<T> Decode(byte[] raw);
    }
}