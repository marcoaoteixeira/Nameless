using Nameless.Fixtures;

namespace Nameless {
    public class QueryableExtensionTests {
        [Test]
        public void OrderBy_Should_Order_Asc_By_Property_Name() {
            // arrange
            var array = new[] {
                new Student { Name = "John" },
                new Student { Name = "Chris" },
                new Student { Name = "Aaron" },
                new Student { Name = "Jennifer" },
                new Student { Name = "Martin" },
            };

            var expected = new[] {
                new Student { Name = "Aaron" },
                new Student { Name = "Chris" },
                new Student { Name = "Jennifer" },
                new Student { Name = "John" },
                new Student { Name = "Martin" },
            };

            // act
            var actual = array
                .AsQueryable()
                .OrderBy(nameof(Student.Name));

            // assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void OrderBy_Should_Order_Desc_By_Property_Name() {
            // arrange
            var array = new[] {
                new Student { Name = "John" },
                new Student { Name = "Chris" },
                new Student { Name = "Aaron" },
                new Student { Name = "Jennifer" },
                new Student { Name = "Martin" },
            };

            var expected = new[] {
                new Student { Name = "Martin" },
                new Student { Name = "John" },
                new Student { Name = "Jennifer" },
                new Student { Name = "Chris" },
                new Student { Name = "Aaron" },
            };

            // act
            var actual = array
                .AsQueryable()
                .OrderByDescending(nameof(Student.Name));

            // assert
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void OrderBy_Should_Throw_MissingMemberException_If_Property_Not_Found() {
            // arrange
            var array = Array.Empty<Student>();

            // act

            // assert
            Assert.Throws<MissingMemberException>(
                code: () => array.AsQueryable().OrderBy("Address")
            );
        }
    }
}
