using System.CodeDom;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace Nameless.Test.Utils {
    public class CategoryAttributeDecorator : ITestClassTagDecorator {
        private const string TAG_NAME = "Category";

        private readonly ITagFilterMatcher _tagFilterMatcher;

        public int Priority { get; }
        public bool RemoveProcessedTags { get; }
        public bool ApplyOtherDecoratorsForProcessedTags { get; }

        public CategoryAttributeDecorator(ITagFilterMatcher tagFilterMatcher) {
            _tagFilterMatcher = tagFilterMatcher ?? throw new ArgumentNullException(nameof(tagFilterMatcher));
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
            => _tagFilterMatcher.Match(TAG_NAME, tagName);

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext) {
            const string nUnitNamespace = "NUnit.Framework.CategoryAttribute";

            var codeTypeReferenceExpression = new CodeTypeReferenceExpression(typeof(Categories));
            var codeFieldReferenceExpression = new CodeFieldReferenceExpression(
                codeTypeReferenceExpression,
                nameof(Categories.RunsOnDevMachine)
            );
            var codeAttributeArgument = new CodeAttributeArgument(
                codeFieldReferenceExpression
            );

            var codeAttributeDeclaration = new CodeAttributeDeclaration();

            // var attr = new CodeAttributeDeclaration(
            //     name: nUnitNamespace,
            //     arguments: [new CodeAttributeArgument()]
            // )
        }
    }
}