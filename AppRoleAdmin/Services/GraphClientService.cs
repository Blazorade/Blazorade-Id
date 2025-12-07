using Blazorade.Id.Core.Services;

namespace AppRoleAdmin.Services
{
    public class GraphClientService
    {
        public GraphClientService(ITokenService tokenService)
        {
            this.TokenService = tokenService;
        }

        private readonly ITokenService TokenService;

    }
}
