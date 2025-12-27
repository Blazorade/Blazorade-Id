
namespace Blazorade.Id.Model
{
    internal class AuthorizationEndpointFailure
    {
        public string Error { get; set; } = string.Empty;

        public AuthorizationCodeFailureReason Reason { get; set; }
    }
}
