using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nameless.Utils.UnitTests {
    
    public class DeepCopyTests {
        [Test]
        public void DeepCopy_Simple_Object() {
            // arrange
            var user = new User { ID = 1, Name = "Test", Email = "test@test.com" };

            // act
            var clone = DeepCopy.Clone(user)!;

            // assert
            Assert.Multiple(() => {
                Assert.That(clone.ID, Is.EqualTo(user.ID));
                Assert.That(clone.Name, Is.EqualTo(user.Name));
                Assert.That(clone.Email, Is.EqualTo(user.Email));
            });
        }

        [Test]
        public void DeepCopy_Simple_Object_With_List() {
            // arrange
            var order = new Order { ID = 1, Name = "Test", Products = {
                new() { ID = 1, Name = "Product1", },
                new() { ID = 2, Name = "Product2", },
            } };

            // act
            var clone = DeepCopy.Clone(order)!;

            // assert
            Assert.Multiple(() => {
                Assert.That(clone.ID, Is.EqualTo(order.ID));
                Assert.That(clone.Name, Is.EqualTo(order.Name));
                Assert.That(clone.Products, Has.Count.EqualTo(2));
                Assert.That(clone.Products[0].ID, Is.EqualTo(order.Products[0].ID));
                Assert.That(clone.Products[0].Name, Is.EqualTo(order.Products[0].Name));
                Assert.That(clone.Products[1].ID, Is.EqualTo(order.Products[1].ID));
                Assert.That(clone.Products[1].Name, Is.EqualTo(order.Products[1].Name));
            });
        }

        [Test]
        public void DeepCopy_Simple_Object_With_Seed() {
            // arrange
            var order = new SeededOrder {
                ID = 1,
                Name = "Test"
            };

            // act
            var clone = DeepCopy.Clone(order)!;

            // assert
            Assert.Multiple(() => {
                Assert.That(clone.ID, Is.EqualTo(order.ID));
                Assert.That(clone.Name, Is.EqualTo(order.Name));
                Assert.That(clone.Products, Has.Count.EqualTo(2));
                Assert.That(clone.Products[0].ID, Is.EqualTo(order.Products[0].ID));
                Assert.That(clone.Products[0].Name, Is.EqualTo(order.Products[0].Name));
                Assert.That(clone.Products[1].ID, Is.EqualTo(order.Products[1].ID));
                Assert.That(clone.Products[1].Name, Is.EqualTo(order.Products[1].Name));
            });
        }
    }
}
