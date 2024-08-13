using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Defines different modes for acquiring tokens.
    /// </summary>
    public enum TokenAcquisitionMode
    {
        /// <summary>
        /// No user interaction is required.
        /// </summary>
        /// <remarks>
        /// If a token cannot be resolved without user interaction, a <c>null</c>
        /// reference is returned
        /// </remarks>
        Silent,
        /// <summary>
        /// Allows user interaction if necessary to acquire a token.
        /// </summary>
        /// <remarks>
        /// No user interaction is the preferred way for token acquisition, but
        /// this option allows interaction if necessary.
        /// </remarks>
        AllowInteraction
    }
}
