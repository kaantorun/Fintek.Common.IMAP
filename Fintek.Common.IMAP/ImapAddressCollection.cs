

#region References
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents a collection of addresses.
    /// </summary>
    #endregion
    public class ImapAddressCollection
    {
        #region private variables
        private ImapAddress _from;
        private ImapAddress _sender;
        private ImapAddress _replyto;
        private ImapAddressList _to;
        private ImapAddressList _cc;
        private ImapAddressList _bcc;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an instance of the ImapAddressCollection class.
        /// </summary>
        public ImapAddressCollection()
        {
            To = new ImapAddressList();
            CC = new ImapAddressList();
            BCC = new ImapAddressList();
        }
        #endregion

        #region public properties
        /// <summary>
        /// Represents the list of addresses in the To field.
        /// </summary>
        public ImapAddressList To
        {
            get { return _to; }
            set { _to = value; }
        }
        /// <summary>
        /// Represents the list of addresses in the CC field.
        /// </summary>
        public ImapAddressList CC
        {
            get { return _cc; }
            set { _cc = value; }
        }
        /// <summary>
        /// Represents the list of addresses in the BCC field.
        /// </summary>
        public ImapAddressList BCC
        {
            get { return _bcc; }
            set { _bcc = value; }
        }
        /// <summary>
        /// Represents the address in the Sender field.
        /// </summary>
        public ImapAddress Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        /// <summary>
        /// Represents the address in the ReplyTo field.
        /// </summary>
        public ImapAddress ReplyTo
        {
            get { return _replyto; }
            set { _replyto = value; }
        }
        /// <summary>
        /// Represents the address in the From field.
        /// </summary>
        public ImapAddress From
        {
            get { return _from; }
            set { _from = value; }
        }
        #endregion

        #region private methods
        private ImapAddressList CreateCollection(string AddressList)
        {
            if (AddressList.ToCharArray()[0] == '(')
            {
                int num = 1;
                string temp = "(";
                for (int i = 1; i < AddressList.Length; i++)
                {
                    if (AddressList.ToCharArray()[i] == '(')
                        num++;
                    else if (AddressList.ToCharArray()[i] == ')' && num > 1)
                        num--;
                    else
                        temp += AddressList.ToCharArray()[i];
                }
                AddressList = temp;
            }
                AddressList = Regex.Replace(AddressList, "\\\\\"", "'");
            AddressList = Regex.Replace(AddressList, @"\.\.", ".");
            //if (AddressList.Contains("Carleton"))
            //    throw new Exception();
            ImapAddressList Addresses = new ImapAddressList();
            MatchCollection matches = Regex.Matches(AddressList, @"\(((?>\((?<LEVEL>)|\)(?<-LEVEL>)|(?! \( | \) ).)+(?(LEVEL)(?!)))\)$");
            foreach (Match match in matches)
            {
                StringBuilder displayName = new StringBuilder("");
                StringBuilder email = new StringBuilder("");
                string value = match.Groups[1].ToString().Trim();
                // Display Name
                if (!(value.StartsWith("NIL")))
                {
                    Match sub;
                    if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success) {
                        value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
                    }
                    sub = Regex.Match(value, @"""([^""]+)""");
                    displayName.Append(sub.Groups[1].ToString());
                    value = value.Substring(sub.Length).Trim();
                }
                else
                    value = value.Substring(3).Trim();
                // Display Name Extended
                if (!(value.StartsWith("NIL")))
                {
                    Match sub;
                    if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success)
                    {
                        value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
                    }
                    sub = Regex.Match(value, @"""([^""]+)""");
                    displayName.Append(" " + sub.Groups[1].ToString());
                    value = value.Substring(sub.Length).Trim();
                }
                else
                    value = value.Substring(3).Trim();
                // Email Prefix
                if (!(value.StartsWith("NIL")))
                {
                    Match sub;
                    if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success)
                    {
                        value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
                    }
                    sub = Regex.Match(value, @"""([^""]+)""");
                    email.Append(sub.Groups[1].ToString());
                    value = value.Substring(sub.Length).Trim();
                }
                else
                    value = value.Substring(3).Trim();
                // Email Suffix
                if (!(value.StartsWith("NIL")))
                {
                    Match sub;
                    if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success)
                    {
                        value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
                    }
                    sub = Regex.Match(value, @"""([^""]+)""");
                    email.Append("@" + sub.Groups[1].ToString());
                    value = value.Substring(sub.Length).Trim();
                }
                else
                    value = value.Substring(3).Trim();
                if (email.ToString().IndexOf('@') < 0)
                    email = new StringBuilder("undisclosed-recipients@localhost");
                ImapAddress address = new ImapAddress(email.ToString(), ImapDecode.Decode(displayName.ToString()));
                Addresses.Add(address);
                return Addresses;
            }
            return Addresses;
        }
        #endregion

        #region internal methods
        internal ImapAddressCollection ParseAddresses(string addresses)
        {
            string addressList;
            addresses = addresses.Trim();
            // From address
            if (addresses.StartsWith("NIL"))
                addresses = addresses.Remove(0, 3).Trim();
            else
            {
                addressList = addresses.Remove(addresses.IndexOf("))") + 2);
                // Remove the current address from the string
                addresses = addresses.Substring(addressList.Length).Trim();
                // Remove leading and trailing ()
                addressList = addressList.Substring(1, addressList.Length - 2);
                this.From = CreateCollection(addressList)[0];
            }
            // Sender address
            if (addresses.StartsWith("NIL"))
                addresses = addresses.Remove(0, 3).Trim();
            else
            {
                addressList = addresses.Remove(addresses.IndexOf("))") + 2);
                // Remove the current address from the string
                addresses = addresses.Substring(addressList.Length).Trim();
                // Remove leading and trailing ()
                addressList = addressList.Substring(1, addressList.Length - 2);
                this.Sender = CreateCollection(addressList)[0];
            }
            // ReplyTo address
            if (addresses.StartsWith("NIL"))
                addresses = addresses.Remove(0, 3).Trim();
            else
            {
                addressList = addresses.Remove(addresses.IndexOf("))") + 2);
                // Remove the current address from the string
                addresses = addresses.Substring(addressList.Length).Trim();
                // Remove leading and trailing ()
                addressList = addressList.Substring(1, addressList.Length - 2);
                this.ReplyTo = CreateCollection(addressList)[0];
            }
            // To address
            if (addresses.StartsWith("NIL"))
                addresses = addresses.Remove(0, 3).Trim();
            else
            {
                addressList = addresses.Remove(addresses.IndexOf("))") + 2);
                // Remove the current address from the string
                addresses = addresses.Substring(addressList.Length).Trim();
                // Remove leading and trailing ()
                addressList = addressList.Substring(1, addressList.Length - 2);
                this.To = CreateCollection(addressList);
            }
            // CC address
            if (addresses.StartsWith("NIL"))
                addresses = addresses.Remove(0, 3).Trim();
            else
            {
                addressList = addresses.Remove(addresses.IndexOf("))") + 2);
                // Remove the current address from the string
                addresses = addresses.Substring(addressList.Length).Trim();
                // Remove leading and trailing ()
                addressList = addressList.Substring(1, addressList.Length - 2);
                this.CC = CreateCollection(addressList);
            }
            // BCC address
            if (addresses.StartsWith("NIL"))
                addresses = addresses.Remove(0, 3).Trim();
            else
            {
                addressList = addresses.Remove(addresses.IndexOf("))") + 2);
                // Remove the current address from the string
                addresses = addresses.Substring(addressList.Length).Trim();
                // Remove leading and trailing ()
                addressList = addressList.Substring(1, addressList.Length - 2);
                this.BCC = CreateCollection(addressList);
            }
            return this;
        }
        #endregion
    }
}
