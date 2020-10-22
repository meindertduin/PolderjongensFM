using System.Collections.Generic;
using System.Linq;
using Pjfm.Application.Identity;

namespace pjfm.Hubs
{
    public class ConnectionMapping<Tkey, Tvalue>
    {
        private readonly Dictionary<Tkey, HashSet<Tvalue>> _connections = new Dictionary<Tkey, HashSet<Tvalue>>();

        public int Count => _connections.Count;

        public void Add(Tkey key, Tvalue value)
        {
            lock (_connections)
            {
                HashSet<Tvalue> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<Tvalue>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(value);
                }
            }
        }
        
        public IEnumerable<Tvalue> GetConnections(Tkey key)
        {
            HashSet<Tvalue> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<Tvalue>();
        }
        
        public void Remove(Tkey key, Tvalue value)
        {
            lock (_connections)
            {
                HashSet<Tvalue> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(value);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}