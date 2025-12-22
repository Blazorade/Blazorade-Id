using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Represents a list of scope values.
    /// </summary>
    public class ScopeList : ListBase<string>
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public ScopeList() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeList"/> class that contains elements copied from the
        /// specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list. Cannot be <see langword="null"/>.</param>
        public ScopeList(IEnumerable<string> collection) : base(collection) { }

        /// <summary>
        /// A collection of standardized identity scopes as defined by OpenID Connect.
        /// </summary>
        public static IEnumerable<string> OpenIdScopes = new string[]
        {
            "openid",
            "profile",
            "email",
            "address",
            "phone",
            "offline_access"
        };

        /// <summary>
        /// Returns <see langword="true"/> if the current scope list contains any identity scopes.
        /// </summary>
        /// <remarks>
        /// The <see cref="OpenIdScopes"/> collection is used to determine which scopes are considered Open ID scopes.
        /// </remarks>
        public bool ContainsOpenIdScopes()
        {
            return this.Any(s => OpenIdScopes.Contains(s));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the current scope list contains also resource scopes, i.e. scopes that
        /// are not part of the <see cref="OpenIdScopes"/> collection.
        /// </summary>
        /// <remarks>
        /// The <see cref="OpenIdScopes"/> collection is used to determine which scopes are considered Open ID scopes.
        /// </remarks>
        public bool ContainsResourceScopes()
        {
            return this.Any(x => !OpenIdScopes.Contains(x));
        }

        /// <summary>
        /// Returns a space-delimited string representation of the scopes in the list.
        /// </summary>
        public override string ToString()
        {
            return string.Join(' ', this);
        }
    }
}
