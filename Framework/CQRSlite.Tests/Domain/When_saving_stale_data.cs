using System;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Domain
{
    
    public class When_saving_stale_data
    {
        private TestInMemoryEventStore _eventStore;
        private TestAggregate _aggregate;
        private TestEventPublisher _eventPublisher;
        private Repository _rep;
        private Session _session;

        
        public void Setup()
        {
            _eventStore = new TestInMemoryEventStore();
            _eventPublisher = new TestEventPublisher();
            _rep = new Repository(_eventStore, _eventPublisher);
            _session = new Session(_rep);

            _aggregate = new TestAggregate(Guid.NewGuid());
            _aggregate.DoSomething();
            _rep.Save(_aggregate);
        }

        [Fact]
        public void Should_throw_concurrency_exception_from_repository()
        {
            Setup();
            Assert.Throws<ConcurrencyException>(() => _rep.Save(_aggregate, 0));
        }

        [Fact]
        public void Should_throw_concurrency_exception_from_session()
        {
            Setup();
            _session.Add(_aggregate);
            _aggregate.DoSomething();
            _rep.Save(_aggregate);
            Assert.Throws<ConcurrencyException>(() => _session.Commit());
        }
    }
}