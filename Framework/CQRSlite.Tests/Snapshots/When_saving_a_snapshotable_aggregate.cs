using CQRSlite.Domain;
using CQRSlite.Snapshots;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Snapshots
{
	
    public class When_saving_a_snapshotable_aggregate
    {
        private TestSnapshotStore _snapshotStore;

		
        public void Setup()
        {
            var eventStore = new TestInMemoryEventStore();
            var eventPublisher = new TestEventPublisher();
            _snapshotStore = new TestSnapshotStore();
            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(_snapshotStore, snapshotStrategy, new Repository(eventStore, eventPublisher), eventStore);
            var session = new Session(repository);
            var aggregate = new TestSnapshotAggregate();
            for (var i = 0; i < 30; i++)
            {
                aggregate.DoSomething();
            }
            session.Add(aggregate);
		    session.Commit();
        }

        [Fact]
        public void Should_save_snapshot()
        {
            Setup();
            Assert.True(_snapshotStore.VerifySave);
        }

        [Fact]
        public void Should_save_last_version_number()
        {
            Setup();
            Assert.Equal(30, _snapshotStore.SavedVersion);
        }
    }
}
