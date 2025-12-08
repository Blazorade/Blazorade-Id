using Blazorade.Id.Core.Configuration;

namespace AppRoleAdmin.Security
{
    public static class Scopes
    {
        public const string ApplicationReadAll = $"{AuthorityOptions.DefaultScope} Application.Read.All";
        public const string ApplicationReadWriteAll = $"{AuthorityOptions.DefaultScope} Application.ReadWrite.All";
        public const string DirectoryReadAll = $"{AuthorityOptions.DefaultScope} Directory.Read.All";
    }
}
