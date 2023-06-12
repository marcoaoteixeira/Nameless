using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Nameless.Collections.Generic;
using Nameless.CommandQuery;
using Nameless.WebApplication.Domain.v1.Common.Models.Output;
using Nameless.WebApplication.Domain.v1.Users.Models.Output;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Domain.v1.Users.Queries {

    public sealed class PaginateUsersQuery : IQuery<PageOutput<UserOutput>> {

        #region Public Properties

        public string? UserNameLike { get; set; }
        public string? EmailLike { get; set; }
        public string? OrderBy { get; set; }
        public bool Ascending { get; set; }
        public int Index { get; set; }
        public int Size { get; set; }

        #endregion
    }

    public sealed class PaginateUsersQueryHandler : QueryHandlerBase<PaginateUsersQuery, PageOutput<UserOutput>> {

        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;

        #endregion

        #region Public Constructors

        public PaginateUsersQueryHandler(UserManager<User> userManager, IMapper mapper)
            : base(mapper) {
            Prevent.Null(userManager, nameof(userManager));

            _userManager = userManager;
        }

        #endregion

        #region Public Override Methods

        public override Task<PageOutput<UserOutput>> HandleAsync(PaginateUsersQuery query, CancellationToken cancellationToken = default) {
            var userQuery = _userManager.Users;

            if (!string.IsNullOrWhiteSpace(query.UserNameLike)) {
                userQuery = userQuery.Where(_ => _.UserName!.Contains(query.UserNameLike));
            }

            if (!string.IsNullOrWhiteSpace(query.EmailLike)) {
                userQuery = userQuery.Where(_ => _.Email!.Contains(query.EmailLike));
            }

            if (!string.IsNullOrWhiteSpace(query.OrderBy)) {
                userQuery = query.Ascending
                    ? userQuery.OrderBy(query.OrderBy)
                    : userQuery.OrderByDescending(query.OrderBy);
            }

            var result = userQuery.AsPage(query.Index, query.Size);
            var output = new PageOutput<UserOutput> {
                Items = Mapper.Map<UserOutput[]>(result),
                Index = result.Index,
                Size = result.Size,
                Total = result.Total
            };

            return Task.FromResult(output);
        }

        #endregion
    }
}
