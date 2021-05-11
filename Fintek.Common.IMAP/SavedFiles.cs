using System;
using System.Collections.Generic;
using System.Text;

namespace Fintek.Common.IMAP
{
    public class SavedFiles
    {
        private int m_Size;
        private string m_FileName;


        public int Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }
        

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

    }
}
