using System;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Domain
{
	
    public class When_getting_aggregate_without_contructor
    {
	    private ISession _session;

	    
        public void Setup()
        {
            var eventStore = new TestInMemoryEventStore();
            var eventPublisher = new TestEventPublisher();
            _session = new Session(new Repository(eventStore, eventPublisher));
        }

        [Fact]
        public void Should_throw_missing_parameterless_constructor_exception()
        {
            Setup();
            Assert.Throws<MissingParameterLessConstructorException>(() => _session.Get<TestAggregateNoParameterLessConstructor>(Guid.NewGuid()));
        }
    }
}