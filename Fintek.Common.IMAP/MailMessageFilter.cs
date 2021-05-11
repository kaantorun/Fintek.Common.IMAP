using System;
using System.Collections.Generic;
using System.Text;

namespace Fintek.Common.IMAP
{
    /// <summary>
    /// 
    /// </summary>
    public class MailMessageFilter
    {

        #region Private Members

        private string _from;
        private string _subject;
        private string _fileName;
        private string _destinationPath;


        #endregion Private Members

        #region Public Members
        /// <summary>
        /// 
        /// </summary>
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; }
        }

        #endregion Public Members

        #region Constructors

        /// <summary>
        /// Create an empty filter object
        /// </summary>
        public MailMessageFilter()
        {
            From = "";
            Subject = "";
            FileName = "";
            DestinationPath = "";
        }

        /// <summary>
        /// User defined filter object
        /// </summary>
        /// <param name="from"></param>
        /// <param name="subject"></param>
        /// <param name="fileName"></param>
        /// <param name="destinationPath"></param>
        public MailMessageFilter(string from, string subject, string fileName, string destinationPath)
        {
            From = from;
            Subject = subject;
            FileName = fileName;
            DestinationPath = destinationPath;
        }

        #endregion Constructors

    }
}
