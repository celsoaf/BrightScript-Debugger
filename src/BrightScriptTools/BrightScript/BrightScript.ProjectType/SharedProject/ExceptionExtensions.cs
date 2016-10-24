﻿using System;
using System.Threading;

namespace BrightScript.SharedProject
{
    static class ExceptionExtensions
    {
        /// <summary>
        /// Returns true if an exception should not be handled by logging code.
        /// </summary>
        public static bool IsCriticalException(this Exception ex)
        {
            return ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is ThreadAbortException ||
                ex is AccessViolationException ||
                ex is CriticalException;
        }
    }

    /// <summary>
    /// An exception that should not be silently handled and logged.
    /// </summary>
    [Serializable]
    class CriticalException : Exception
    {
        public CriticalException() { }
        public CriticalException(string message) : base(message) { }
        public CriticalException(string message, Exception inner) : base(message, inner) { }
        protected CriticalException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}