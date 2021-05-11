using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Fintek.Common.IMAP
{
    public class SavedMails
    {
        #region Private Properties
        private string m_From;
        private ArrayList m_SavedFiles;
        private int m_ExceptionType;

        #endregion Private Properties

        #region Public Properties
        
        /// <summary>
        /// Who sent mail
        /// </summary>
        public string From
        {
            get { return m_From; }
            set { m_From = value; }
        }

        /// <summary>
        /// Saved file name
        /// </summary>
        public ArrayList SavedFiles
        {
            get { return m_SavedFiles; }
            set { m_SavedFiles = value; }
        }
        
        /// <summary>
        /// If an exception occurs. Type is given by this number. 1 -> File name wrong, 0 -> No exception
        /// </summary>
        public int ExceptionType
        {
            get { return m_ExceptionType; }
            set { m_ExceptionType = value; }
        }

        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public SavedMails()
        {
            m_SavedFiles = new ArrayList();
            m_From = "";
            m_ExceptionType = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fileName"></param>
        /// <param name="size"></param>
        public SavedMails(string from, string fileName, int exceptionType)
        {
            m_SavedFiles.Add(fileName);
            m_From = from;
            m_ExceptionType = exceptionType;
        }

        #endregion Constructors
    }
}
