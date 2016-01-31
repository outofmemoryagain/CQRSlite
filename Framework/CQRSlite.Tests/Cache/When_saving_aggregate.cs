﻿using System;
using CQRSlite.Cache;
using CQRSlite.Tests.Substitutes;
using Xunit;
using CQRSlite.Tests.Extensions;

namespace CQRSlite.Tests.Cache
{
    
    public class When_saving_aggregate
    {
        private CacheRepository _rep;
        private TestAggregate _aggregate;
        private TestRepository _testRep;

        
        public When_saving_aggregate()
        {
            _testRep = new TestRepository();
            _rep = new CacheRepository(_testRep, new TestInMemoryEventStore());
            _aggregate = _testRep.Get<TestAggregate>(Guid.NewGuid());
            _aggregate.DoSomething();
            _rep.Save(_aggregate,-1);
        }

        [Fact]
        public void Should_get_same_aggregate_on_get()
        {
            var aggregate = _rep.Get<TestAggregate>(_aggregate.Id);
            Assert.Equal(aggregate, _aggregate);
        }

        [Fact]
        public void Should_save_to_repository()
        {
            Assert.Equal(_testRep.Saved, _aggregate);
        }

        [Fact]
        public void Should_not_cache_empty_id()
        {
            var aggregate = new TestAggregate(Guid.Empty);
            _rep.Save(aggregate);
            Assert.NotEqual(_rep.Get<TestAggregate>(Guid.Empty), aggregate);
        }
    }
}