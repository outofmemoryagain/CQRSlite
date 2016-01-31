using System;
using CQRSlite.Cache;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Cache
{
    
    public class When_getting_aggregate
    {
        private CacheRepository _rep;
        private TestAggregate _aggregate;

        
        public void Setup()
        {
            _rep = new CacheRepository(new TestRepository(), new TestEventStore());
            _aggregate = _rep.Get<TestAggregate>(Guid.NewGuid());
        }

        [Fact]
        public void Should_get_aggregate()
        {
            Setup();
            Assert.NotNull(_aggregate);
        }

        [Fact]
        public void Should_get_same_aggregate_on_second_try()
        {
            Setup();
            var aggregate =_rep.Get<TestAggregate>(_aggregate.Id);
            Assert.Equal(aggregate, _aggregate);
        }

        [Fact]
        public void Should_update_if_version_changed_in_event_store()
        {
            Setup();
            var aggregate = _rep.Get<TestAggregate>(_aggregate.Id);
            Assert.Equal(aggregate.Version, 3);
        }

        [Fact]
        public void Should_get_same_aggregate_from_different_cache_repository()
        {
            Setup();
            var rep = new CacheRepository(new TestRepository(), new TestInMemoryEventStore());
            var aggregate = rep.Get<TestAggregate>(_aggregate.Id);
            Assert.Equal(aggregate, _aggregate);
        }
    }
}