using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service contract for services that are used for creating code challenges for 
    /// use with an authorization code flow with PKCE (Proof Key Code Exchange).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A code verifier is a clear text, random string that sent to the token endpoint when
    /// acquiring tokens using an issued authorization code. The code challenge is a hashed
    /// version of the code verifier and is sent to the authorization endpoint when requesting
    /// an authorization code.
    /// </para>
    /// </remarks>
    public interface ICodeChallengeService
    {
        /// <summary>
        /// Creates a code verifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A code verifier is a clear text, random string that sent to the token endpoint when
        /// acquiring tokens using an issued authorization code.
        /// </para>
        /// <para>
        /// A code verifier is always paired with a code challenge, which is a hashed version of
        /// the code verifier. Use the the <see cref="CreateCodeChallenge(string)"/> method to
        /// create a code challenge using the code verifier created by this method.
        /// </para>
        /// </remarks>
        string CreateCodeVerifier();

        /// <summary>
        /// Creates a code challenge from the given code verifier.
        /// </summary>
        /// <param name="codeVerifier">The code verifier to create the corresponding code challenge from.</param>
        /// <remarks>
        /// Use the <see cref="CreateCodeVerifier"/> method to create a random code verifier string.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The argument that is thrown if the code verifier is less than 43 chars in length.
        /// </exception>
        CodeChallenge CreateCodeChallenge(string codeVerifier);
    }
}
