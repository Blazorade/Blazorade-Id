
namespace Blazorade.Id.Model
{
    internal class AuthorizationPopupFailure
    {
        public string Error { get; set; } = string.Empty;

        public AuthorizationCodeFailureReason Reason { get; set; }
    }
}
