using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents a code challenge value that is an authorization code flow with PKCE (Proof Key Code Exchange).
    /// </summary>
    /// <remarks>
    /// Use the <see cref="CodeChallengeService"/> to create a code verifier and a corresponding code challenge.
    /// </remarks>
    public class CodeChallenge
    {
        /// <summary>
        /// The code challenge value.
        /// </summary>
        public string ChallengeValue { get; set; } = null!;

        /// <summary>
        /// The method that was used to produce the code challenge in <see cref="ChallengeValue"/>.
        /// </summary>
        public string ChallengeMethod { get; set; } = null!;
    }
}
