using System;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Domain
{
	
    public class When_getting_events_out_of_order
    {
	    private ISession _session;

	    
        public void Setup()
        {
            var eventStore = new TestEventStoreWithBugs();
            var testEventPublisher = new TestEventPublisher();
            _session = new Session(new Repository(eventStore, testEventPublisher));
        }

        [Fact]
        public void Should_throw_concurrency_exception()
        {
            Setup();
            var id = Guid.NewGuid();
            Assert.Throws<EventsOutOfOrderException>(() => _session.Get<TestAggregate>(id, 3));
        }
    }
}