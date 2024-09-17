using System.Reflection;
using Nameless.Fixtures;

namespace Nameless.Attributes;

public class SingletonAttributeTests {
    // NOTE: Use SingletonAttribute does not prevent developers from
    // creating a class that does not abie to the Singleton Object Pattern.
    // They also might expose a public static property named with the same
    // value of DEFAULT_ACCESSOR_NAME and trick SingletonAttribute.GetInstance<T>
    // into retrieve that property's value.
    // So, what I'm trying to say is that there are plenty of ways to
    // workaround and get what you want when programming. But if you do
    // it so, maybe you need to think about the cleanliness of your own code.

    [Test]
    public void When_Type_Uses_SingletonAttribute_Then_GetInstance_Returns_Singleton_Instance() {
        // act
        var instance = SingletonAttribute.GetInstance<MySingletonClass>();

        // assert
        Assert.That(instance, Is.Not.Null);
    }

    [Test]
    public void When_Type_Is_Not_A_Singleton_And_Do_Not_Uses_SingletonAttribute_Then_GetInstance_Returns_Null() {
        // act
        var instance = SingletonAttribute.GetInstance<MyNonSingletonClass>();

        // assert
        Assert.That(instance, Is.Null);
    }

    [Test]
    public void When_Type_Uses_SingletonAttribute_And_Have_A_Different_Accessor_Name_Then_GetInstance_Returns_Singleton_Instance() {
        // act
        var instance = SingletonAttribute.GetInstance<MySingletonClassWithDifferentAccessor>();
        var attr = typeof(MySingletonClassWithDifferentAccessor).GetCustomAttribute<SingletonAttribute>(inherit: false);

        // assert
        Assert.Multiple(() => {
            Assert.That(instance, Is.Not.Null);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.AccessorName, Is.Not.EqualTo(SingletonAttribute.DEFAULT_ACCESSOR_NAME));
        });
    }

    [Test]
    public void When_Type_Do_Not_Uses_SingletonAttribute_Then_GetInstance_Returns_Null() {
        // act
        var instance = SingletonAttribute.GetInstance<MySingletonClassWithoutAttribute>();
        var attr = typeof(MySingletonClassWithoutAttribute).GetCustomAttribute<SingletonAttribute>(inherit: false);

        // assert
        Assert.Multiple(() => {
            Assert.That(instance, Is.Null);
            Assert.That(attr, Is.Null);
        });
    }
}