#region Refrences
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Fintek.Common.IMAP
{
    #region public exceptions
    /// <summary>
    /// Represents an exception for the IMAP library.
    /// </summary>
    public class ImapException : Exception
    {
        /// <summary>
        /// Initalizes an Exception for the IMAP library.
        /// </summary>
        public ImapException() : base() { }
        /// <summary>
        /// Initalizes an Exception for the IMAP library using a message.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        public ImapException(string message) : base(message) { }
        /// <summary>
        /// Initalizes an Exception for the IMAP library using a message and inner exception.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        /// <param name="inner">An exception that this exception is includes.</param>
        public ImapException(string message, Exception inner) : base(message, inner) { }
    }
    /// <summary>
    /// Represents an exception of type ImapException.ImapConnectionException.
    /// </summary>
    public class ImapConnectionException : ImapException
    {
        /// <summary>
        /// Initalizes an Exception in the connect object.
        /// </summary>
        public ImapConnectionException() : base() { }
        /// <summary>
        /// Initalizes an Exception in the connect object using a message.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        public ImapConnectionException(string message) : base(message) { }
        /// <summary>
        /// Initalizes an Exception in the connect object using a message and inner exception.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        /// <param name="inner">An exception that this exception is includes.</param>
        public ImapConnectionException(string message, Exception inner) : base(message, inner) { }
    }
    /// <summary>
    /// Represents an exception in the authentication object.
    /// </summary>
    public class ImapAuthenticationException : ImapException
    {
        /// <summary>
        /// Initalizes an exception in the authentication object.
        /// </summary>
        public ImapAuthenticationException() : base() { }
        /// <summary>
        /// Initalizes an exception in the authentication object with a message.
        /// </summary>
        /// <param name="message"></param>
        public ImapAuthenticationException(string message) : base(message) { }
        /// <summary>
        /// Initalizes an exception in the authentication object with a message and inner exception.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        /// <param name="inner">An exception that this exception is includes.</param>
        public ImapAuthenticationException(string message, Exception inner) : base(message, inner) { }
    }
    /// <summary>
    /// Represents an exception in the authentication object for a unsupported authentication type.
    /// </summary>
    public class ImapAuthenticationNotSupportedException : ImapAuthenticationException
    {
        /// <summary>
        /// Initalizes an exception in the authentication object for a unsupported authentication type.
        /// </summary>
        public ImapAuthenticationNotSupportedException() : base() { }
        /// <summary>
        /// Initalizes an exception in the authentication object for a unsupported authentication type with a message.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        public ImapAuthenticationNotSupportedException(string message) : base(message) { }
        /// <summary>
        /// Initalizes an exception in the authentication object for a unsupported authentication type with a message and inner exception.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        /// <param name="inner">An exception that this exception is includes.</param>
        public ImapAuthenticationNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
    /// <summary>
    /// Represents an exception in the command object.
    /// </summary>
    public class ImapCommandException : ImapException
    {
        /// <summary>
        /// Initalizes an exception in the command object.
        /// </summary>
        public ImapCommandException() : base() { }
        /// <summary>
        /// Initalizes an exception in the command object with a message.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        public ImapCommandException(string message) : base(message) { }
        /// <summary>
        /// Initalizes an exception in the command object with a message and inner exception.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        /// <param name="inner">An exception that this exception is includes.</param>
        public ImapCommandException(string message, Exception inner) : base(message, inner) { }

    }
    /// <summary>
    /// Represents an exception in the command object for an invalid message number.
    /// </summary>
    public class ImapCommandInvalidMessageNumber : ImapCommandException
    {
        /// <summary>
        /// Initalizes an exception in the command object for an invalid message number.
        /// </summary>
        public ImapCommandInvalidMessageNumber() : base() { }
        /// <summary>
        /// Initalizes an exception in the command object for an invalid message number with a message.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        public ImapCommandInvalidMessageNumber(string message) : base(message) { }
        /// <summary>
        /// Initalizes an exception in the command object for an invalid message number with a message and inner exception.
        /// </summary>
        /// <param name="message">A string containing some text to describe the error.</param>
        /// <param name="inner">An exception that this exception is includes.</param>
        public ImapCommandInvalidMessageNumber(string message, Exception inner) : base(message, inner) { }
    }
    #endregion
}