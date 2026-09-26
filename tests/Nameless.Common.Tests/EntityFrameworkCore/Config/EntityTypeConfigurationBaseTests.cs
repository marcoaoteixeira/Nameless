using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.EntityFrameworkCore.Entities;

namespace Nameless.EntityFrameworkCore.Config;

[UnitTest]
public class EntityTypeConfigurationBaseTests {
    public sealed class TestEntity : EntityBase {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class TestConfiguration : EntityTypeConfigurationBase<TestEntity> {
        public bool ConfigureCoreCalled { get; private set; }

        public TestConfiguration(string tableName) : base(tableName) { }

        protected override void ConfigureCore(EntityTypeBuilder<TestEntity> builder) {
            ConfigureCoreCalled = true;
        }
    }

    [Fact]
    public void Constructor_WithNullOrWhiteSpaceTableName_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => new TestConfiguration(tableName: " "));
    }

    [Fact]
    public void Configure_SetsTableNameKeyAndOptionalDateProperties() {
        // arrange
        var sut = new TestConfiguration("TestEntities");
        var modelBuilder = new ModelBuilder();

        // act
        modelBuilder.Entity<TestEntity>(sut.Configure);

        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));

        // assert
        Assert.Multiple(
            () => Assert.NotNull(entityType),
            () => Assert.Equal("TestEntities", entityType!.GetTableName()),
            () => Assert.Contains(nameof(TestEntity.ID), entityType!.FindPrimaryKey()!.Properties.Select(p => p.Name)),
            () => Assert.True(entityType!.FindProperty(nameof(TestEntity.CreationDate))!.IsNullable),
            () => Assert.True(entityType!.FindProperty(nameof(TestEntity.ModificationDate))!.IsNullable),
            () => Assert.True(sut.ConfigureCoreCalled)
        );
    }
}
