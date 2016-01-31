using System;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Domain
{
    
    public class When_saving_events_without_id
    {
        private TestInMemoryEventStore _eventStore;
        private TestAggregate _aggregate;
        private TestEventPublisher _eventPublisher;
        private Repository _rep;

        
        public void Setup()
        {
            _eventStore = new TestInMemoryEventStore();
            _eventPublisher = new TestEventPublisher();
            _rep = new Repository(_eventStore, _eventPublisher);

            _aggregate = new TestAggregate(Guid.Empty);
        }

        [Fact]
        public void Should_throw_aggregate_or_event_missing_id_exception_from_repository()
        {
            Setup();
            Assert.Throws<AggregateOrEventMissingIdException>(() => _rep.Save(_aggregate, 0));
        }
    }
}