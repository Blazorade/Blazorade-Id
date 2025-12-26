using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Represents a scope in a OAuth access token.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public Scope() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class with the specified value and classification.
        /// </summary>
        /// <param name="value">The string value that identifies the scope. Cannot be <see langword="null"/>.</param>
        /// <param name="classification">The classification of the scope. Defaults to <see cref="ScopeClassification.Default"/> if not specified.</param>
        public Scope(string value, ScopeClassification classification = ScopeClassification.Default)
        {
            if(string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            this.Value = value;
            this.Classification = classification;
        }



        /// <summary>
        /// The scope value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// The classification of the scope.
        /// </summary>
        public ScopeClassification Classification { get; set; }
        }
}
