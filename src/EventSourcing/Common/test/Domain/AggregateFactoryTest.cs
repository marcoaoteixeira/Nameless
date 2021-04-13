using System;
using Nameless.EventSourcing.Domain;
using Nameless.EventSourcing.Test.Fixtures;
using Xunit;

namespace Nameless.EventSourcing.Test.Domain {
    public class AggregateFactoryTest {
        [Fact]
        public void Can_Create_Instance () {
            // arrange
            IAggregateFactory factory;

            // act
            factory = new AggregateFactory ();

            // assert
            Assert.NotNull (factory);
        }

        [Fact]
        public void Can_Create_New_Aggregate () {
            // arrange
            var factory = new AggregateFactory ();

            // act
            var aggregate = factory.Create<OrderAggregateRoot> ();

            // assert
            Assert.NotNull (aggregate);
        }

        [Fact]
        public void Can_Create_New_Aggregate_With_Arguments () {
            // arrange
            var factory = new AggregateFactory ();
            var id = Guid.NewGuid ();

            // act
            var aggregate = factory.Create<OrderAggregateRoot> (args: new object[] { id });

            // assert
            Assert.NotNull (aggregate);
            Assert.Equal (id, aggregate.AggregateID);
        }
    }
}