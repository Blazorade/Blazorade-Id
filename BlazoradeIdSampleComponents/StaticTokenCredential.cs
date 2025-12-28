using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;

namespace BlazoradeIdSampleComponents
{
    public class StaticTokenCredential : TokenCredential
    {
        private readonly AccessToken AccessToken;

        public StaticTokenCredential(string token, DateTimeOffset expiresOn)
        {
            this.AccessToken = new AccessToken(token, expiresOn);
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(this.AccessToken);
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return this.AccessToken;
        }

    }
}
