using System.Collections;
using System.Dynamic;

namespace Nameless.NHibernate;

public class HashtableDynamicObjectTests {

    [Test]
    public void Can_Create_New_HashtableDynamicObject() {
        // arrange
        IDynamicMetaObjectProvider provider;

        // act
        provider = new HashtableDynamicObject();

        // assert
        Assert.That(provider, Is.Not.Null);
    }

    [Test]
    public void Can_Insert_New_Property() {

        // arrange
        dynamic hashtable = new HashtableDynamicObject();

        // act
        hashtable.Id = 1;

        // assert
        Assert.That(hashtable, Is.Not.Null);
        Assert.That(hashtable.Id, Is.EqualTo(1));
    }

    [Test]
    public void Can_Create_Entity_With_Dynamic_Attributes() {
        // arrange
        Entity entity;

        // act
        entity = new Entity();

        // assert
        entity.Attribute.Name = "Test";
        var name = entity.GetAttributes().First().value;

        Assert.Multiple(() => {
            Assert.That(entity.Attribute, Is.Not.Null);
            Assert.That(entity.Attribute.Name, Is.EqualTo("Test"));
            Assert.That(name, Is.EqualTo("Test"));
        });
    }

    public class Entity {
        private readonly IDictionary _attributes = new Hashtable();
        private readonly HashtableDynamicObject _proxy;
        public dynamic Attribute => _proxy;

        public Entity() {
            _proxy = new HashtableDynamicObject(_attributes);
        }

        public IEnumerable<(object? key, object? value)> GetAttributes() {
            foreach (var key in _attributes.Keys) {
                yield return (key, _attributes[key]);
            }
        }
    }
}