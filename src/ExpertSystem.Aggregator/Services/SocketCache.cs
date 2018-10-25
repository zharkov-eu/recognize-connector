using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator.Services
{
    public class SocketCache : IEnumerable<CustomSocket>
    {
        private readonly Dictionary<string, CustomSocket> _cache;

        public SocketCache(IReadOnlyCollection<CustomSocket> sockets = null)
        {
            _cache = new Dictionary<string, CustomSocket>();
            if (sockets == null) return;
            foreach (var socket in sockets)
                _cache.Add(socket.SocketName, socket);
        }

        public List<CustomSocket> GetAll()
        {
            return _cache.Values.Select(p => p.Clone()).ToList();
        }

        public CustomSocket Get(string socketName)
        {
            if (!SocketExists(socketName))
                throw new Exception($"Socket {socketName} not exists");
            return _cache[socketName].Clone();
        }

        public void Add(CustomSocket socket)
        {
            _cache.Add(socket.SocketName, socket);
        }

        public void Update(string socketName, CustomSocket socket)
        {
            if (!SocketExists(socketName))
                throw new Exception($"Socket {socketName} not exists");
            _cache[socketName] = socket;
        }

        public void Remove(string socketName)
        {
            _cache.Remove(socketName);
        }

        public bool SocketExists(string socketName)
        {
            return _cache.ContainsKey(socketName);
        }

        public IEnumerator<CustomSocket> GetEnumerator()
        {
            return _cache.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}