#region Refrences
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents the ImapConnect object.
    /// </summary>
    #endregion
    public class ImapConnect : IDisposable
    {

        #region private variables
        private string _hostname = null;
        private int _port = 143;
        private TcpClient _connection;
        private Stream _stream;
        private StreamReader _streamReader;
        private ConnectionState _connectionState;
        private int _receiveTimeout = 1000000;
        private int _sendTimeout = 1000000;
        private LoginType _loginType = LoginType.NONE;
        private int _tag = 0;
        #endregion

        #region internal properties
        internal TcpClient Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        internal Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        internal StreamReader StreamReader
        {
            get { return _streamReader; }
            set { _streamReader = value; }
        }

        internal ConnectionState ConnectionState
        {
            set { _connectionState = value; }
            get { return _connectionState; }
        }

        internal string tag
        {
            get {
                return "kw" + ((int)_tag++).ToString().PadLeft(4, '0') + " "; 
            }
        }
        internal string CurrentTag
        {
            get
            {
                return "kw" + ((int)_tag - 1).ToString().PadLeft(4, '0') + " ";
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the login type of the IMAP server.
        /// </summary>
        public LoginType LoginType
        {
            get { return _loginType; }
            set { _loginType = value; }
        }
        /// <summary>
        /// Gets or sets the hostname of the IMAP server to connect to.
        /// </summary>
        public string Hostname
        {
            get { return _hostname; }
            set { _hostname = value; }
        }
        /// <summary>
        /// Gets or sets the port to connect on.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        /// <summary>
        /// Gets or sets the timeout for receiving commands.
        /// </summary>
        public int ReceiveTimeout
        {
            get { return _receiveTimeout; }
            set { _receiveTimeout = value; }
        }
        /// <summary>
        /// Gets or sets the timeout for sending commands.
        /// </summary>
        public int SendTimeout
        {
            get { return _sendTimeout; }
            set { _sendTimeout = value; }
        }
        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        public ConnectionState State
        {
            get { return _connectionState; }
        }

        #endregion

        #region constructors
        /// <summary>
        /// Initalizes an instance of the ImapConnect object using hostname and port.
        /// </summary>
        /// <param name="hostname">A string value representing the hostname of the mail server to connect to.</param>
        /// <param name="port">A integer value representing the port number on which imap connects on the specified mail server.</param>
        public ImapConnect(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }
        /// <summary>
        /// Initalizes an instance of the ImapConnect object using hostname.
        /// </summary>
        /// <param name="hostname">A string value representing the hostname of the mail server to connect to.</param>
        public ImapConnect(string hostname)
        {
            Hostname = hostname;
            // Port is default
        }
        /// <summary>
        /// Initalizes an instance of the ImapConnect object
        /// </summary>
        public ImapConnect()
        {
            Hostname = "127.0.0.1";
        }
        #endregion

        #region public methods
        /// <summary>
        /// Opens a connection to the IMAP server.
        /// </summary>
        /// <returns>Returns a boolean value of true if the connection succeded.</returns>
        public bool Open()
        {
            string read;
            _connectionState = ConnectionState.Connecting;
            Connection = new TcpClient();
            Connection.ReceiveTimeout = ReceiveTimeout;
            Connection.SendTimeout = SendTimeout;
            try
            {
                Connection.Connect(Hostname, Port);
                Stream = Connection.GetStream();
                // ------------------------------------------------
                System.Text.Encoding encodingTR = System.Text.Encoding.GetEncoding("ISO-8859-9");
                // StreamReader = new StreamReader(Stream, System.Text.Encoding.ASCII);
                StreamReader = new StreamReader(Stream, encodingTR);
                // ------------------------------------------------
                //read = StreamReader.ReadLine();
                read = Read();
                if (read.StartsWith("* OK "))
                {
                    ConnectionState = ConnectionState.Connected;
                    return true;
                }
                else
                    throw new ImapConnectionException(read);
            }
            catch (Exception ex)
            {
                _connectionState = ConnectionState.Closed;
                throw new ImapConnectionException("Connection Failed", ex);
            }
        }

        /// <summary>
        /// Closes the current connection.
        /// </summary>
        /// <returns>Returns a boolean value indicating if the connection was closed.</returns>
        /// <exception cref="ImapConnectionException" />
        public bool Close()
        {
            try
            {
                Connection.Close();
                ConnectionState = ConnectionState.Closed;
                return true;
            }
            catch (Exception ex)
            {
                throw new ImapConnectionException("Error Closing Connection", ex);
            }
        }

        /// <summary>
        /// Releases all resources used by the ImapConnect object.
        /// </summary>
        public void Dispose()
        {
            Connection = null;
            StreamReader = null;
            Stream = null;
        }
        #endregion

        #region internal methods
        internal void Write(string message)
        {
            message = tag + message;
            byte[] command = System.Text.Encoding.ASCII.GetBytes(message.ToCharArray());
            try
            {
                //System.Web.HttpContext.Current.Response.Write(message + "<BR>");
                Stream.Write(command, 0, command.Length);
            }
            catch (Exception e)
            {
                throw new Exception("Write error :" + e.Message);
            }
        }

        internal void UntaggedWrite(string message)
        {
            byte[] command = System.Text.Encoding.ASCII.GetBytes(message.ToCharArray());
            try
            {
                Stream.Write(command, 0, command.Length);
            }
            catch (Exception e)
            {
                throw new Exception("Write error :" + e.Message);
            }
        }

        internal string Read()
        {
            string response = StreamReader.ReadLine();
            //System.Web.HttpContext.Current.Response.Write(response + "<BR>");
            return response;
        }
        #endregion
    }
}
