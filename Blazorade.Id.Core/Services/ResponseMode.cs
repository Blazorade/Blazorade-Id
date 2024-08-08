﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Defines response modes for how tokens and other information is returned to a client.
    /// </summary>
    public enum ResponseMode
    {
        /// <summary>
        /// Information is returned as query string parameters.
        /// </summary>
        Query,

        /// <summary>
        /// Information is returned as parameters in the fragment of the URI.
        /// </summary>
        Fragment,

        /// <summary>
        /// Information is returned as a form post.
        /// </summary>
        Form_Post
    }
}
