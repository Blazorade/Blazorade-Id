using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Represents a result of an operation.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class OperationResult<TValue, TError> where TValue : class where TError : class
    {
        public OperationResult(TValue? value = null, TError? error = null)
        {
            this.Value = value;
            this.Error = error;
            this.IsSuccess = null != this.Value && null == this.Error;
        }

        /// <summary>
        /// The expected result of the operation.
        /// </summary>
        public TValue? Value { get; private set; }

        /// <summary>
        /// Determines whether the operation was successful or not.
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// The error of the operation.
        /// </summary>
        public TError? Error { get; private set; }

    }

    /// <summary>
    /// Represents a result of an operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class OperationResult<TValue> : OperationResult<TValue, OperationError> where TValue : class
    {
        public OperationResult(TValue? value = null, OperationError? error = null) : base(value, null) { }
    }
}
