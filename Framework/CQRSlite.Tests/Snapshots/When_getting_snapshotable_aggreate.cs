using System;
using CQRSlite.Domain;
using CQRSlite.Snapshots;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Snapshots
{
	
    public class When_getting_snapshotable_aggreate
    {
        private TestSnapshotStore _snapshotStore;
        private TestSnapshotAggregate _aggregate;

		
        public void Setup()
        {
            var eventStore = new TestInMemoryEventStore();
            var eventPublisher = new TestEventPublisher();
            _snapshotStore = new TestSnapshotStore();
            var snapshotStrategy = new DefaultSnapshotStrategy();
		    var repository = new SnapshotRepository(_snapshotStore, snapshotStrategy, new Repository(eventStore, eventPublisher), eventStore);
            var session = new Session(repository);

            _aggregate = session.Get<TestSnapshotAggregate>(Guid.NewGuid());
        }

        [Fact]
        public void Should_ask_for_snapshot()
        {
            Setup();
            Assert.True(_snapshotStore.VerifyGet);
        }

        [Fact]
        public void Should_run_restore_method()
        {
            Setup();
            Assert.True(_aggregate.Restored);
        }
    }
}
