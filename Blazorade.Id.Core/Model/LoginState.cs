using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// A state object used to transfer state across the login process.
    /// </summary>
    public class LoginState
    {
        /// <summary>
        /// The URI in the application where the user should be taken back after the login process.
        /// </summary>
        /// <remarks>
        /// This is usually the URL where the login process was started.
        /// </remarks>
        public string? Uri {  get; set; }
    }

}
