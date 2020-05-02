using System;
using System.Data;
using System.IO;
using Dasync.Collections;
using Moq;
using Nameless.Data;
using Nameless.EventSourcing.Event;
using Nameless.EventSourcing.Test.Fixtures;
using Nameless.FileProvider;
using Xunit;

namespace Nameless.EventSourcing.Test.Events {
    public class EventStoreTest {
        [Fact]
        public void Can_Create_Instance () {
            // arrange
            var database = new Mock<IDatabase> ();
            var fileProvider = new Mock<IFileProvider> ();
            var eventSerializer = new Mock<IEventSerializer> ();
            IEventStore eventStore;

            // act
            eventStore = new EventStore (database.Object, fileProvider.Object, eventSerializer.Object);

            // assert
            Assert.NotNull (eventStore);
        }

        // [Fact]
        // public async void Can_Get_Events () {
        //     // arrange
        //     var database = new Mock<IDatabase> ();
        //     database
        //         .Setup (_ => _.ExecuteReaderAsync (It.IsAny<string> (), It.IsAny<Func<IDataRecord, IEvent>> (), It.IsAny<CommandType> (), It.IsAny<Parameter[]> ()))
        //         .Returns (AsyncEnumerable.Empty<ChangeOrderDeliveryDateEvent> ());
        //     var file = new Mock<IFile> ();
        //     file
        //         .Setup (_ => _.GetStream ())
        //         .Returns (Stream.Null);
        //     var fileProvider = new Mock<IFileProvider> ();
        //     fileProvider
        //         .Setup (_ => _.GetFile (It.IsAny<string> ()))
        //         .Returns (file.Object);
        //     var eventSerializer = new Mock<IEventSerializer> ();
        //     var eventStore = new EventStore (database.Object, fileProvider.Object, eventSerializer.Object);
        //     var aggregateID = Guid.NewGuid ();

        //     // act
        //     var events = await eventStore.GetAsync (aggregateID, null).ToArrayAsync ();

        //     // assert
        //     Assert.NotNull (events);
        // }
    }
}
