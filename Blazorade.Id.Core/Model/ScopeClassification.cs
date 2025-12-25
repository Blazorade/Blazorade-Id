using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Defines different classifications for scope values.
    /// </summary>
    public enum ScopeClassification
    {
        /// <summary>
        /// Routine, low-impact delegated scopes that are safe to use in most scenarios.
        /// </summary>
        Default,
        /// <summary>
        /// Read access to user content or PII, or anything that can materially impact privacy even without write permissions.
        /// </summary>
        Sensitive,
        /// <summary>
        /// Write, admin, directory-wide, mailbox-wide, or other high-impact scopes.
        /// </summary>
        Elevated
    }
}
