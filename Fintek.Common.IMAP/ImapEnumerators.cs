
#region Refrences

#endregion

namespace Fintek.Common.IMAP
{
    #region public enumerators
    /// <summary>
    /// Contains a list of possible values for the connection state of the current instance.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Indicates the connection is being established.
        /// </summary>
        Connecting, 
        /// <summary>
        /// Indicates the connection has been established.
        /// </summary>
        Connected, 
        /// <summary>
        /// Indicates that the connection is attempting to authenticate.
        /// </summary>
        Authenticating, 
        /// <summary>
        /// Indicates the connection is open and has been authenticated.
        /// </summary>
        Open, 
        /// <summary>
        /// Indicates that something in the connection has failed.
        /// </summary>
        Broken, 
        /// <summary>
        /// Indicates the connection has been closed.
        /// </summary>
        Closed
    }
    /// <summary>
    /// Contains a list of possible values for the login type on the specified server.
    /// </summary>
    public enum LoginType
    {
    	/// <summary>
    	/// NONE.  Indicates that the LoginType has yet to be set
    	/// </summary>
    	NONE = 0,
    	/// <summary>
        /// Plain Text Authentication.
        /// </summary>
        PLAIN,
        /// <summary>
        /// Login Authentication (Outlook authentication similar to SASL)
        /// </summary>
        LOGIN,
        /// <summary>
        /// CRAM-MD5 Authentication.
        /// </summary>
        CRAM_MD5
        
    }
    /// <summary>
    /// Type of encoding used in message part
    /// </summary>
    public enum BodyPartEncoding
    {
        /// <summary>
        /// 7BIT Encoding
        /// </summary>
        UTF7,
        /// <summary>
        /// 8BIT Encoding (no encoding)
        /// </summary>
        UTF8,
        /// <summary>
        /// Base64 Encoding
        /// </summary>
        BASE64,
        /// <summary>
        /// Quoted Printable (ASCII)
        /// </summary>
        QUOTEDPRINTABLE,
        /// <summary>
        /// Unknown Encoding
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// Binary Encoding
        /// </summary>
        NONE
    }
    #endregion
}
