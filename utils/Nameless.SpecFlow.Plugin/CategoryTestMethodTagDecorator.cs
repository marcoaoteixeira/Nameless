using System.CodeDom;
using Nameless.Test.Utils;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;

namespace Nameless.SpecFlow.Plugin {
    public sealed class CategoryTestMethodTagDecorator : ITestMethodTagDecorator {
        internal const string TAG_NAME = "MCategory";

        private readonly ITagFilterMatcher _tagFilterMatcher;

        public int Priority { get; }
        public bool RemoveProcessedTags { get; }
        public bool ApplyOtherDecoratorsForProcessedTags { get; }

        public CategoryTestMethodTagDecorator(ITagFilterMatcher tagFilterMatcher) {
            _tagFilterMatcher = tagFilterMatcher ?? throw new ArgumentNullException(nameof(tagFilterMatcher));
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            => _tagFilterMatcher.Match(TAG_NAME, tagName);

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod) {
            const string nUnitNamespace = "NUnit.Framework.CategoryAttribute";

            var codeTypeReferenceExpression = new CodeTypeReferenceExpression(typeof(Categories));

            var codeFieldReferenceExpression = new CodeFieldReferenceExpression(
                codeTypeReferenceExpression,
                nameof(Categories.RunsOnDevMachine)
            );

            var codeAttributeArgument = new CodeAttributeArgument(
                codeFieldReferenceExpression
            );

            var codeAttributeDeclaration = new CodeAttributeDeclaration(
                nUnitNamespace,
                codeAttributeArgument
            );

            testMethod.CustomAttributes.Add(codeAttributeDeclaration);
        }
    }
}
