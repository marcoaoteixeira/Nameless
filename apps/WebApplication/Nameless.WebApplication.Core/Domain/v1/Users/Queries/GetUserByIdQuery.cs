using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Nameless.CommandQuery;
using Nameless.WebApplication.Domain.v1.Users.Models.Output;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Domain.v1.Users.Queries {

    public sealed class GetUserByIdQuery : IQuery<UserOutput?> {

        #region Public Properties

        public Guid UserId { get; set; }

        #endregion
    }

    public sealed class GetUserByIdCommandHandler : QueryHandlerBase<GetUserByIdQuery, UserOutput?> {

        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;

        #endregion

        #region Public Constructors

        public GetUserByIdCommandHandler(UserManager<User> userManager, IMapper mapper) 
            : base(mapper) {
            Prevent.Null(userManager, nameof(userManager));

            _userManager = userManager;
        }

        #endregion

        #region Public Override Methods
        
        public override async Task<UserOutput?> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default) {
            var user = await _userManager.FindByIdAsync(query.UserId.ToString());

            return user != default ? Mapper.Map<UserOutput>(user) : default;
        } 

        #endregion
    }
}
