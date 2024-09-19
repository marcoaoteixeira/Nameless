using System.Linq.Expressions;
using Nameless.Fixtures;

namespace Nameless;

public class ExpressionExtensionTests {
    [Test]
    public void GetExpressionPath_Should_Return_Property_Path_Lambda() {
        // arrange
        LambdaExpression lambda = (Category category) => category.Children[123].Children[456].Name;
        const string expected = "category.Children[123].Children[456].Name";

        // act
        var actual = ExpressionExtension.GetExpressionPath(lambda);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetExpressionPath_Should_Return_Empty_If_Method_Path_Lambda() {
        // arrange
        LambdaExpression lambda = (Category category) => category.Children[123].ToString();

        // act
        var actual = ExpressionExtension.GetExpressionPath(lambda);

        // assert
        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void GetExpressionPath_Should_Return_Empty_For_Method_Calls() {
        // arrange
        LambdaExpression lambda = (Category category) => category.DoSomething(10);

        // act
        var actual = ExpressionExtension.GetExpressionPath(lambda);

        // assert
        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void And_Concat_Two_Expressions_Into_An_And_Operator() {
        // arrange
        var students = new[] {
            new Student { Name = "John", Age = 21 },
            new Student { Name = "Chris", Age = 21 }
        };

        var expression = ExpressionExtension.And<Student>(
            self: student => student.Age == 21,
            expression: student => student.Name == "John"
        );

        // act
        var actual = students.Single(expression.Compile());

        // assert
        Assert.That(actual, Is.EqualTo(students[0]));
    }

    [Test]
    public void Or_Concat_Two_Expressions_Into_An_Or_Operator() {
        // arrange
        var students = new[] {
            new Student { Name = "John", Age = 21 },
            new Student { Name = "Chris", Age = 21 }
        };

        Expression<Func<Student, bool>> where = student => student.Age == 21;

        // extend
        where = where.Or(student => student.Name == "Chris");

        // act
        var actual = students.Where(where.Compile());

        // assert
        Assert.That(actual, Is.EquivalentTo(students));
    }
}