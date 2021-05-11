

#region Refrences
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents the ImapAddressCollection object.
    /// </summary>
    #endregion
    public class ImapAddressList : List<ImapAddress>
    {
        #region public methods
        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>Returns a string representation of the object.</returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder("");
            foreach (ImapAddress value in this)
            {
                if (output.Length > 0)
                    output.Append(", ");
                output.Append(value.ToString());
            }
            return output.ToString();
        }
        #endregion
    }
}
