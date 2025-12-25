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
    public class ScopeList : ListBase<Scope>
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public ScopeList() : base() { }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public ScopeList(IEnumerable<string> scopes)
        {
            foreach(var scope in scopes)
            {
                this.Add(new Scope(scope));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeList"/> class that contains elements copied from the
        /// specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list. Cannot be <see langword="null"/>.</param>
        public ScopeList(IEnumerable<Scope> collection) : base(collection) { }

        /// <summary>
        /// A collection of standardized identity scopes as defined by OpenID Connect.
        /// </summary>
        public static IEnumerable<Scope> OpenIdScopes = new Scope[]
        {
            new Scope("openid"),
            new Scope("profile"),
            new Scope("email"),
            new Scope("address", ScopeClassification.Sensitive),
            new Scope("phone", ScopeClassification.Sensitive),
            new Scope("offline_access", ScopeClassification.Elevated)
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

        /// <summary>
        /// Returns the values of teh scopes in the list.
        /// </summary>
        public IEnumerable<string> Values()
        {
            return from x in this select x.Value;
        }
    }
}
