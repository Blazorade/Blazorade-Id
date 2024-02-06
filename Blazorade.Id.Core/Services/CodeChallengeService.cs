using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Blazorade.Id.Core.Services
{
    public class CodeChallengeService
    {

        private Random Rnd = new Random();
        private const string CodeVerifierChars = "abcdefghijklmnopqrstuvwxyz0123456789";

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
