namespace N88.Worlds
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An object to track entities and the components bound to them.
    /// Supports component pooling.
    /// </summary>
    public class World
    {
        private readonly Dictionary<Type, Dictionary<int, object>> _components = new();
        private readonly Dictionary<Type, List<object>> _componentPools = new();
        private int _idCounter;

        /// <summary>
        /// Entities are just IDs. Creating one increments the counter.
        /// Please keep track of your entities.
        /// </summary>
        /// <returns>A unique identifier for the entity.</returns>
        public int CreateEntity()
        {
            _idCounter++;
            return _idCounter;
        }
        
        /// <summary>
        /// Returns a component that is currently active and mapped to an entity. 
        /// </summary>
        public bool TryGetComponentMappedToEntity<T>(int id, out T? component) where T : class
        {
            component = default;
            if (_components.TryGetValue(typeof(T), out var dictionary))
            {
                if (dictionary.TryGetValue(id, out var value))
                {
                    component = value as T;
                    return true;
                }
            } 
            return false;
        }

        /// <summary>
        /// Binds a component to an entity by id.
        /// </summary>
        public bool TryBindComponentToEntity<T>(int id, T component)
        {
            if (component == null)
            {
                throw new NullReferenceException("Cannot bind to null components");
            }
            if (id > _idCounter) { return false; }
            if (_components.TryGetValue(typeof(T), out var dictionary))
            {
                // todo: multiple components of the same type?
                if (!dictionary.TryAdd(id, component))
                {
                    throw new ComponentBindException("no support for multiple components");
                }
                return true;
            }
            dictionary = new Dictionary<int, object> { { id, component } };
            _components.Add(typeof(T), dictionary);
            _componentPools.Add(typeof(T), new List<object>());
            return true;
        }

        /// <summary>
        /// Unbinds all components from an entity id, returning the components to their respective pools,
        /// and disposing any components that are disposable.
        /// </summary>
        public bool TryReleaseEntity(int id)
        {
            if (id > _idCounter) { return false; }
            foreach (var (componentType, componentMap) in _components)
            {
                if (!componentMap.TryGetValue(id, out var componentObject)) {continue;}
                if (componentObject is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _componentPools[componentType].Add(componentObject);
                componentMap.Remove(id);
            }

            return true;
        }

        /// <summary>
        /// If a component of type <see cref="T"/> is not mapped to an entity, it will be returned.
        /// Otherwise, a default component will be returned. 
        /// </summary>
        public T? GetUnboundComponent<T>() where T : class
        {
            if (_componentPools.TryGetValue(typeof(T), out var pool))
            {
                if (pool.Count == 0)
                {
                    return default;
                }
                var component = pool[0] as T;
                pool.RemoveAt(0);
                return component;
            }
            return default;
        }

        /// <summary>
        /// Get all components of type <see cref="T"/> bound to any entities. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public IEnumerable<T?> GetComponentsForAllEntities<T>() where T : class
        {
            var type = typeof(T);
            var components = new List<T?>();
            if (_components.TryGetValue(type, out var map))
            {
                foreach (var (_, component) in map)
                {
                    components.Add(component as T);
                }
            }
            return components;
        }

        /// <summary>
        /// Returns all entities with a component of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public IEnumerable<int> GetEntitiesWithComponent<T>() where T : class
        {
            var type = typeof(T);
            var entities = new List<int>();
            if (_components.TryGetValue(type, out var map))
            {
                foreach (var (entity, _) in map)
                {
                    entities.Add(entity);
                }
            }
            return entities;
        }

        /// <summary>
        /// Releases a binding of a component from the given entity and returns true if
        /// that is done. Disposes any disposable components that are released and
        /// returns them to the component pool for reuse.
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <typeparam name="T"></typeparam>
        public bool TryReleaseComponent<T>(int id) where T : class
        {
            var type = typeof(T);
            if (_components.TryGetValue(type, out var components))
            {
                if (components.TryGetValue(id, out var boundComponent))
                {
                    _componentPools[type].Add(boundComponent);
                    components.Remove(id);
                    if (boundComponent is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    return true;
                }
            }
            return false;
        }
    }
}