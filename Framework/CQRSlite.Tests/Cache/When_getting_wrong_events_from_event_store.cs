﻿using System;
using CQRSlite.Cache;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Cache
{
    
    public class When_getting_earlier_than_expected_events_from_event_store
    {
        private CacheRepository _rep;
        private TestAggregate _aggregate;
        
        public When_getting_earlier_than_expected_events_from_event_store()
        {
            _rep = new CacheRepository(new TestRepository(), new TestEventStoreWithBugs());
            _aggregate = _rep.Get<TestAggregate>(Guid.NewGuid());
        }

        [Fact]
        public void Should_evict_old_object_from_cache()
        {
            _rep.Get<TestAggregate>(_aggregate.Id);

            var aggregate = _rep.Get<TestAggregate>(_aggregate.Id);
            Assert.NotEqual(aggregate, _aggregate);
        }

        [Fact]
        public void Should_get_events_from_start()
        {
            var aggregate =_rep.Get<TestAggregate>(_aggregate.Id);
            Assert.Equal(aggregate.Version, 1);
        }
    }
}