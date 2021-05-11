#region Refrences
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents an Imap Mailbox.
    /// </summary>
    #endregion
    public partial class ImapMailbox
    {
        #region private variables
        private string _mailbox;
        private int _exist;
        private int _recent;
        private bool _readwrite;
        private ImapMessageFlags _flags;
        private List<ImapMailboxMessage> _messages;
        #endregion

        #region public properties
        /// <summary>
        /// Gets a string value representing the current mailbox.
        /// </summary>
        public string Mailbox
        {
            get { return _mailbox; }
            internal set { _mailbox = value; }
        }
        /// <summary>
        /// Gets a integer value representing the number of messages in the mailbox.
        /// </summary>
        public int Exist
        {
            get { return _exist; }
            internal set { _exist = value; }
        }
        /// <summary>
        /// Gets a integer value representing the number of "Recent" messages in the mailbox.
        /// </summary>
        public int Recent
        {
            get { return _recent; }
            internal set { _recent = value; }
        }
        /// <summary>
        /// Gets a object of type <see cref="ImapMessageFlags" /> that contains the flags in the mailbox.
        /// </summary>
        public ImapMessageFlags Flags
        {
            get { return _flags; }
            internal set { _flags = value; }
        }
        /// <summary>
        /// Gets or sets the list object that contains a collection of ImapMailboxMessage.
        /// </summary>
        public List<ImapMailboxMessage> Messages
        {
            get { return (List<ImapMailboxMessage>)_messages; }
            set { _messages = (List<ImapMailboxMessage>)value; }
        }
        /// <summary>
        /// Gets a boolean value representing if the mailbox is writable and readable.
        /// </summary>
        /// 
        public bool ReadWrite
        {
            get { return _readwrite; }
            internal set { _readwrite = value; }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Initalizes an instance of the mailbox object.
        /// </summary>
        public ImapMailbox() { }
        /// <summary>
        /// Initalizes an instance of the mailbox object specifying the mailbox name.
        /// </summary>
        /// <param name="mailbox">A string representing the mailbox name.</param>
        public ImapMailbox(string mailbox)
        {
            this.Mailbox = mailbox;
        }
        /// <summary>
        /// Initalizes an instance of the mailbox object specifiying the mailbox name and how man messages it contains.
        /// </summary>
        /// <param name="mailbox">A string representing the mailbox name.</param>
        /// <param name="exist">An integer value representing the number of messages that exist in the mailbox.</param>
        public ImapMailbox(string mailbox, int exist)
        {
            this.Mailbox = mailbox;
            this.Exist = exist;
        }
        /// <summary>
        /// Initalizes an instance of the mailbox object specifiying the mailbox name and how man messages it contains.
        /// </summary>
        /// <param name="mailbox">A string representing the mailbox name.</param>
        /// <param name="exist">An integer value representing the number of messages that exist in the mailbox.</param>
        /// <param name="recent">A integer value representing the number of messages that are marked recent in the mailbox.</param>
        public ImapMailbox(string mailbox, int exist, int recent)
        {
            this.Mailbox = mailbox;
            this.Exist = exist;
            this.Recent = recent;
        }
        #endregion
    }
}
