using System;
using CQRSlite.Cache;
using CQRSlite.Tests.Substitutes;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace CQRSlite.Tests.Cache
{
    
    public class When_saving_fails
    {
        private CacheRepository _rep;
        private TestAggregate _aggregate;
        private TestRepository _testRep;

        public void Setup()
        {
            _testRep = new TestRepository();
            _rep = new CacheRepository(_testRep, new TestInMemoryEventStore());
            _aggregate = _testRep.Get<TestAggregate>(Guid.NewGuid());
            _aggregate.DoSomething();
            try
            {
                _rep.Save(_aggregate, 100);
            }
            catch (Exception){}
        }

        [Fact]
        public void Should_evict_old_object_from_cache()
        {
            Setup();
            var aggregate = CacheRepository.Cache.Get(_aggregate.Id.ToString());// _rep.Get<TestAggregate>(_aggregate.Id);
            Assert.Null(aggregate);
        }
    }
}