using System.CodeDom;
using System.ComponentModel;
using Nameless.Test.Utils;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace Nameless.SpecFlow.Plugin {
    public sealed class CategoryTestClassTagDecorator : ITestClassTagDecorator {
        internal const string TAG_NAME = "CCategory";

        private readonly ITagFilterMatcher _tagFilterMatcher;

        public int Priority { get; }
        public bool RemoveProcessedTags { get; }
        public bool ApplyOtherDecoratorsForProcessedTags { get; }

        public CategoryTestClassTagDecorator(ITagFilterMatcher tagFilterMatcher) {
            _tagFilterMatcher = tagFilterMatcher ?? throw new ArgumentNullException(nameof(tagFilterMatcher));
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
            => _tagFilterMatcher.Match(TAG_NAME, tagName);

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext) {
            const string nUnitNamespace = "NUnit.Framework.CategoryAttribute";
            var attr = typeof(DescriptionAttribute).FullName;
            var codeTypeReferenceExpression = new CodeTypeReferenceExpression(typeof(Categories));

            var codeFieldReferenceExpression = new CodeFieldReferenceExpression(
                codeTypeReferenceExpression,
                nameof(Categories.RunsOnDevMachine)
            );

            var codeAttributeArgument = new CodeAttributeArgument(
                codeFieldReferenceExpression
            );

            //var codeAttributeDeclaration = new CodeAttributeDeclaration(
            //    nUnitNamespace,
            //    codeAttributeArgument
            //);

            var codeAttributeDeclaration = new CodeAttributeDeclaration(
                attr
            );

            generationContext.TestClass.CustomAttributes.Add(codeAttributeDeclaration);
        }
    }
}
