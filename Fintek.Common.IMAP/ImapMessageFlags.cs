#region References
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents IMAP message flags.
    /// </summary>
    #endregion
    public class ImapMessageFlags
    {
        #region private variables
        private bool _answered;
        private bool _deleted;
        private bool _draft;
        private bool _flagged;
        private bool _recent;
        private bool _seen;
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets a boolean value representing if the message has been seen.
        /// </summary>
        public bool Seen
        {
            get { return _seen; }
            set { _seen = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value representing if the message is deleted.
        /// </summary>
        public bool Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value representing if the message is a draft.
        /// </summary>
        public bool Draft
        {
            get { return _draft; }
            set { _draft = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value representing if the message was answered.
        /// </summary>
        public bool Answered
        {
            get { return _answered; }
            set { _answered = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value representing if the message is flagged.
        /// </summary>
        public bool Flagged
        {
            get { return _flagged; }
            set { _flagged = value; }
        }
        /// <summary>
        /// Gets or sets a boolean value representing if the message is recent.
        /// </summary>
        public bool Recent
        {
            get { return _recent; }
            set { _recent = value; }
        }
        #endregion
        
        #region public constructors
        /// <summary>
        /// Initalizes a instance of the ImapMailbox
        /// </summary>
        public ImapMessageFlags() { }
        /// <summary>
        /// Initalizes a instance of the ImapMailbox
        /// </summary>
        /// <param name="draft">A boolean value representing the draft flag.</param>
        /// <param name="answered">A boolean value representing the answered flag.</param>
        /// <param name="flagged">A boolean value representing the flagged flag.</param>
        /// <param name="deleted">A boolean value representing the deleted flag.</param>
        /// <param name="seen">A boolean value representing the seen flag.</param>
        /// <param name="recent">A boolean value representing the recent flag.</param>
        public ImapMessageFlags(bool draft, bool answered, bool flagged, bool deleted, bool seen, bool recent)
        {
            this.Draft = draft;
            this.Answered = answered;
            this.Flagged = flagged;
            this.Deleted = deleted;
            this.Seen = seen;
            this.Recent = recent;
        }
        /// <summary>
        /// Initalizes a instance of the ImapMailbox
        /// </summary>
        /// <param name="flags">A string value containing a list of flags.</param>
        public ImapMessageFlags(string flags)
        {
            ParseFlags(flags);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Parses the flags of the message and sets boolean fields.
        /// </summary>
        /// <param name="flags">A string containing the list of flags.</param>
        public void ParseFlags(string flags)
        {
            string[] key;
             
            //Split on spaces instead.  Not all flags will start with \
            //key = flags.Split('\\');
            key = flags.Split();
            
            if (key.Length > 0)
            {
                for (int i = 0; i < key.Length; i++)
                    switch (key[i].Trim())
                    {
                        case "\\Draft":
                            this.Draft = true;
                            break;
                        case "\\Answered":
                            this.Answered = true;
                            break;
                        case "\\Flagged":
                            this.Flagged = true;
                            break;
                        case "\\Deleted":
                            this.Deleted = true;
                            break;
                        case "\\Seen":
                            this.Seen = true;
                            break;
                        case "\\Recent":
                            this.Recent = true;
                            break;
                        /*default:
                            throw new Exception(key[i].ToString());*/
                    }
            }
        }
        #endregion

    }
}
