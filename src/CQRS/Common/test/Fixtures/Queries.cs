using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.CQRS.Common.Test.Fixtures {
    public class UpperCaseStringQuery : IQuery<string> {
        public string Value { get; set; }
    }

    public class UpperCaseStringQueryHandler : IQueryHandler<UpperCaseStringQuery, string> {
        public Task<string> HandleAsync (UpperCaseStringQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            return Task.FromResult (query.Value.ToUpper ());
        }
    }
}