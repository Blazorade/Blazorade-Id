using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service class that is used for creating code challenges for use with
    /// an authorization code flow with PKCE (Proof Key Code Exchange).
    /// </summary>
    public class CodeChallengeService : ICodeChallengeService
    {

        private Random Rnd = new Random();
        private const string CodeVerifierChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        /// <inheritdoc/>
        public string CreateCodeVerifier()
        {
            var length = this.Rnd.Next(43, 60);
            var arr = new char[length];

            for (int i = 0; i < length; i++)
            {
                var ix = this.Rnd.Next(0, CodeVerifierChars.Length - 1);
                arr[i] = CodeVerifierChars[ix];
            }

            return string.Join("", arr);
        }

        private const string CodeChallengeMethodValueS256 = "S256";

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
        public CodeChallenge CreateCodeChallenge(string codeVerifier)
        {
            if(null == codeVerifier || codeVerifier.Length < 43)
            {
                throw new ArgumentException("The code verifier must be at least 43 chars long.", nameof(codeVerifier));
            }

            var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var challenge = Convert.ToBase64String(hash);
            challenge = Regex.Replace(challenge, "\\+", "-");
            challenge = Regex.Replace(challenge, "\\/", "_");
            challenge = Regex.Replace(challenge, "=+$", "");

            return new CodeChallenge
            {
                ChallengeValue = challenge,
                ChallengeMethod = CodeChallengeMethodValueS256
            };
        }
    }
}
