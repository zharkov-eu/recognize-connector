using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;

namespace ExpertSystem.Aggregator.Services
{
    public struct EntityCacheOptions
    {
        public string IdPropertyName;
    }

    public class EntityCache<T> : IEnumerable<T>
        where T : IDeepCloneable<T>
    {
        private readonly Type _entityType = typeof(T);
        private readonly EntityCacheOptions _options;
        private readonly Dictionary<string, T> _cache;

        public EntityCache(EntityCacheOptions options, IReadOnlyCollection<T> entities = null)
        {
            _options = options;
            _cache = new Dictionary<string, T>();
            if (entities == null) return;
            foreach (var entity in entities)
                _cache.Add(GetEntityId(entity), entity);
        }

        public List<T> GetAll()
        {
            return _cache.Values.Select(p => p.Clone()).ToList();
        }

        public T Get(string id)
        {
            if (!EntityExists(id))
                throw new Exception($"Entity {id} not exists");
            return _cache[id].Clone();
        }

        public void Add(T entity)
        {
            _cache.Add(GetEntityId(entity), entity);
        }

        public void Update(string id, T entity)
        {
            if (!EntityExists(id))
                throw new Exception($"Entity {id} not exists");
            _cache[id] = entity;
        }

        public void Remove(string id)
        {
            _cache.Remove(id);
        }

        public bool EntityExists(string id)
        {
            return _cache.ContainsKey(id);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _cache.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string GetEntityId(T entity)
        {
            return _entityType.GetProperty(_options.IdPropertyName).GetValue(entity).ToString();
        }
    }
}