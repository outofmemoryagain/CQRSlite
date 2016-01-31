using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Snapshots;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Snapshots
{
	
    public class When_saving_a_snapshotable_aggregate_for_each_change
    {
        private TestInMemorySnapshotStore _snapshotStore;
	    private ISession _session;
	    private TestSnapshotAggregate _aggregate;

	    
        public void Setup()        
		{
            IEventStore eventStore = new TestInMemoryEventStore();
            var eventPublisher = new TestEventPublisher();
            _snapshotStore = new TestInMemorySnapshotStore();
            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(_snapshotStore, snapshotStrategy, new Repository(eventStore, eventPublisher), eventStore);
	        _session = new Session(repository);
            _aggregate = new TestSnapshotAggregate();

            for (var i = 0; i < 20; i++)
            {
                _session.Add(_aggregate);
                _aggregate.DoSomething();
                _session.Commit();
            }
        }

        [Fact]
        public void Should_snapshot_15th_change()
        {
            Setup();
            Assert.Equal(15, _snapshotStore.SavedVersion);
        }

        [Fact]
        public void Should_not_snapshot_first_event()
        {
            Setup();
            Assert.False(_snapshotStore.FirstSaved);
        }

        [Fact]
        public void Should_get_aggregate_back_correct()
        {
            Setup();
            Assert.Equal(20, _session.Get<TestSnapshotAggregate>(_aggregate.Id).Number);
        }
    }
}