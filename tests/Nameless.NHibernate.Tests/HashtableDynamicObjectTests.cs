using System.Collections;
using System.Dynamic;
using Nameless.NHibernate.Infrastructure;

namespace Nameless.NHibernate;

public class HashtableDynamicObjectTests {
    [Fact]
    public void Can_Create_New_HashtableDynamicObject() {
        // arrange
        IDynamicMetaObjectProvider provider;

        // act
        provider = new HashtableDynamicObject();

        // assert
        Assert.NotNull(provider);
    }

    [Fact]
    public void Can_Insert_New_Property() {
        // arrange
        dynamic hashtable = new HashtableDynamicObject();

        // act
        hashtable.Id = 1;

        // assert
        Assert.NotNull(hashtable);
        Assert.Equal(1, hashtable.Id);
    }

    [Fact]
    public void Can_Create_Entity_With_Dynamic_Attributes() {
        // arrange
        Entity entity;

        // act
        entity = new Entity();

        // assert
        entity.Attribute.Name = "Test";
        var name = entity.GetAttributes().First().value;

        Assert.Multiple(() => {
            Assert.NotNull(entity.Attribute);
            Assert.Equal("Test", entity.Attribute.Name);
            Assert.Equal("Test", name);
        });
    }

    public class Entity {
        private readonly IDictionary _attributes = new Hashtable();
        private readonly HashtableDynamicObject _proxy;
        public dynamic Attribute => _proxy;

        public Entity() {
            _proxy = new HashtableDynamicObject(_attributes);
        }

        public IEnumerable<(object key, object value)> GetAttributes() {
            foreach (var key in _attributes.Keys) {
                yield return (key, _attributes[key]);
            }
        }
    }
}