using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents a scope value.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public Scope() { }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="fullValue">The full value of the scope.</param>
        /// <remarks>
        /// The constructor will split the <paramref name="fullValue"/> 
        /// into the resource ID and the scope value.
        /// </remarks>
        public Scope(string fullValue)
        {
            if(null == fullValue) throw new ArgumentNullException(nameof(fullValue));
            if(fullValue.Contains('/'))
            {
                this.ResourceId = fullValue.Substring(0, fullValue.LastIndexOf('/'));
                this.Value = fullValue.Substring(fullValue.LastIndexOf('/') + 1);
            }
            else
            {
                this.Value = fullValue;
            }
        }

        /// <summary>
        /// Optional resource ID part of the scope value.
        /// </summary>
        public string? ResourceId { get; set; }

        /// <summary>
        /// The scope value without the resource ID.
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// Returns the full scope value which includes the resource ID if available.
        /// </summary>
        public override string ToString()
        {
            return this.ResourceId?.Length > 0 ? $"{this.ResourceId}/{this.Value}" : this.Value;
        }
    }
}
