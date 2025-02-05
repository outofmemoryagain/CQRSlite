using System;
using System.Linq;
using CQRSlite.Domain.Exception;
using CQRSlite.Domain.Factories;
using CQRSlite.Events;

namespace CQRSlite.Domain
{
    public class Repository : IRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _publisher;

        public Repository(IEventStore eventStore, IEventPublisher publisher)
        {
            if(eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if(publisher == null)
                throw new ArgumentNullException(nameof(publisher));

            _eventStore = eventStore;
            _publisher = publisher;
        }

        public void Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot
        {
            if (expectedVersion != null && _eventStore.Get(aggregate.Id, expectedVersion.Value).Any())
                throw new ConcurrencyException(aggregate.Id);

            var changes = aggregate.FlushUncommitedChanges();
            _eventStore.Save(changes);
            foreach (var @event in changes)
                _publisher.Publish(@event);
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            return LoadAggregate<T>(aggregateId);
        }

        private T LoadAggregate<T>(Guid id) where T : AggregateRoot
        {
            var aggregate = AggregateFactory.CreateAggregate<T>();

            var events = _eventStore.Get(id, -1);
            if (!events.Any())
                throw new AggregateNotFoundException(id);

            aggregate.LoadFromHistory(events);
            return aggregate;
        }
    }
}
