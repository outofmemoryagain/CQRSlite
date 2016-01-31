using System;
using System.Collections.Concurrent;
using System.Linq;
using CQRSlite.Domain;
using CQRSlite.Events;
using Microsoft.Extensions.Caching.Memory;

namespace CQRSlite.Cache
{
    public class CacheRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly IEventStore _eventStore;
        public static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        readonly MemoryCacheEntryOptions _cacheOptions;
        private static readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();

        public CacheRepository(IRepository repository, IEventStore eventStore)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));

            _repository = repository;
            _eventStore = eventStore;

            _cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = new TimeSpan(0, 0, 15, 0)
            };
            _cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                object o;
                _locks.TryRemove(key.ToString(), out o);
            });
        }

        public void Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot
        {
            var idstring = aggregate.Id.ToString();
            try
            {
                lock (_locks.GetOrAdd(idstring, _ => new object()))
                {
                    if (aggregate.Id != Guid.Empty && !IsTracked(aggregate.Id))
                        Cache.Set(idstring, aggregate, _cacheOptions);
                    _repository.Save(aggregate, expectedVersion);
                }
            }
            catch (Exception)
            {
                lock (_locks.GetOrAdd(idstring, _ => new object()))
                {
                    Cache.Remove(idstring);
                }
                throw;
            }
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            var idstring = aggregateId.ToString();
            try
            {
                lock (_locks.GetOrAdd(idstring, _ => new object()))
                {
                    T aggregate;
                    if (IsTracked(aggregateId))
                    {
                        aggregate = (T) Cache.Get(idstring);
                        var events = _eventStore.Get(aggregateId, aggregate.Version);
                        if (events.Any() && events.First().Version != aggregate.Version + 1)
                        {
                            Cache.Remove(idstring);
                        }
                        else
                        {
                            aggregate.LoadFromHistory(events);
                            return aggregate;
                        }
                    }

                    aggregate = _repository.Get<T>(aggregateId);
                    Cache.Set(idstring, aggregate, _cacheOptions);
                    return aggregate;
                }
            }
            catch (Exception)
            {
                lock (_locks.GetOrAdd(idstring, _ => new object()))
                {
                    Cache.Remove(idstring);
                }
                throw;
            }
        }

        private bool IsTracked(Guid id)
        {
            object output;
            return Cache.TryGetValue(id.ToString(), out output);
        }
    }
}