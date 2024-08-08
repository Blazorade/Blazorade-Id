using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents an error that occured during an operation.
    /// </summary>
    public class OperationError
    {

        /// <summary>
        /// The error code.
        /// </summary>
        public string? Code {  get; set; }

        /// <summary>
        /// The description of the error.
        /// </summary>
        public string? Description { get; set; }

    }
}
