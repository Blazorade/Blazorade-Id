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
        public string? Uri {  get; set; }

        public object? ApplicationState {  get; set; }

        public string? AuthorityKey { get; set; }
    }

}
