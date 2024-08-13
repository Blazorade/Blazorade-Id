using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Represents the results of a token acquisition call.
    /// </summary>
    public class TokenAcquisitionResult
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public TokenAcquisitionResult(JwtSecurityToken? identityToken, JwtSecurityToken? accessToken)
        {
            this.AccessToken = accessToken;
            this.IdentityToken = identityToken;
        }

        /// <summary>
        /// The access token acquired.
        /// </summary>
        /// <remarks>
        /// If a valid token could not be resolved, <c>null</c> is returned.
        /// </remarks>
        public JwtSecurityToken? AccessToken { get; private set; }

        /// <summary>
        /// The identity token acquired.
        /// </summary>
        /// <remarks>
        /// If a valid token could not be resolved, <c>null</c> is returned.
        /// </remarks>
        public JwtSecurityToken? IdentityToken {  get; private set; }

    }
}
