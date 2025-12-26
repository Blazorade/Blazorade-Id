using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A base implementation for various types of stores.
    /// </summary>
    public abstract class StoreBase
    {
        /// <summary>
        /// Returns a key that can be used to store/retrieve a token of the specified <paramref name="tokenType"/>.
        /// </summary>
        protected string GetKey(TokenType tokenType, string? suffix = null)
        {
            return this.GetKey(tokenType.ToString(), suffix: suffix);
        }

        /// <summary>
        /// Returns the prefix for all keys stored by Blazorade Id token store implementations.
        /// </summary>
        /// <returns></returns>
        protected string GetKeyPrefix()
        {
            return "blazorade.id.";
        }

        /// <summary>
        /// Returns a fully qualified key for the specified <paramref name="name"/>.
        /// </summary>
        protected string GetKey(string name, string? suffix = null)
        {
            var prefix = this.GetKeyPrefix();

            suffix = null != suffix ? $".{suffix}" : string.Empty;
            return $"{prefix}{name.ToLower()}{suffix}";
        }
    }
}
