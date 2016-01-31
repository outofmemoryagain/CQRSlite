using System;
using CQRSlite.Bus;
using CQRSlite.Tests.Substitutes;
using Xunit;

namespace CQRSlite.Tests.Bus
{
	
    public class When_sending_command
    {
        //private InProcessBus _bus;
        
        //public When_sending_command()
        //{
        //    _bus = new InProcessBus();
        //}

        [Fact]
        public void Should_run_handler()
        {
            var bus = new InProcessBus();
            var handler = new TestAggregateDoSomethingHandler();
            bus.RegisterHandler<TestAggregateDoSomething>(handler.Handle);
            bus.Send(new TestAggregateDoSomething());

            Assert.Equal(1,handler.TimesRun);
        }

        [Fact]
        public void Should_throw_if_more_handlers()
        {
            var bus = new InProcessBus();
            var x = new TestAggregateDoSomethingHandler();
            bus.RegisterHandler<TestAggregateDoSomething>(x.Handle);
            bus.RegisterHandler<TestAggregateDoSomething>(x.Handle);

            Assert.Throws<InvalidOperationException>(() => bus.Send(new TestAggregateDoSomething()));
        }

        [Fact]
        public void Should_throw_if_no_handlers()
        {
            var bus = new InProcessBus();
            Assert.Throws<InvalidOperationException>(() => bus.Send(new TestAggregateDoSomething()));
        }
    }
}