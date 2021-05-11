#region Refrences
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents the ImapCommand object.
    /// </summary>
    #endregion
    public class ImapCommand
    {
        #region private variables
        const string patFetchComplete = @"^kw\d+\WOK\W([Ff][Ee][Tt][Cc][Hh]\W|)[Cc][Oo][Mm][Pp][Ll][Ee][Tt][Ee]";
        ImapConnect _connection;
        #endregion

        #region protected properties
        /// <summary>
        /// Sets the ImapConnect to use in this instance.
        /// </summary>
        public ImapConnect Connection
        {
            set { _connection = value; }
            private get { return _connection; }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Initalizes an instance of the ImapCommand object.
        /// </summary>
        public ImapCommand() { }
        /// <summary>
        /// Initalizes an instance of the ImapCommand object.
        /// </summary>
        /// <param name="connection">A ImapConnect object representing the connection to use in this instance.</param>
        public ImapCommand(ImapConnect connection)
        {
            this.Connection = connection;
        }
        #endregion

        #region public enumerators
        /// <summary>
        /// Properties that messages can be sorted on.
        /// </summary>
        public enum SortMethod
        {
            /// <summary>
            /// No property.
            /// </summary>
            NONE, 
            /// <summary>
            /// Sort on arrival (received).
            /// </summary>
            ARRIVAL, 
            /// <summary>
            /// Sort on CC addresses.
            /// </summary>
            CC, 
            /// <summary>
            /// Sort on sent date.
            /// </summary>
            DATE, 
            /// <summary>
            /// Sort on the From address.
            /// </summary>
            FROM, 
            /// <summary>
            /// Sort on message size.
            /// </summary>
            SIZE, 
            /// <summary>
            /// Sort on message subject.
            /// </summary>
            SUBJECT
        }
        /// <summary>
        /// Options for indicating the direction of the sort.
        /// </summary>
        public enum SortOrder {
            /// <summary>
            /// Sorts the messages ascending.
            /// </summary>
            ASC,
            /// <summary>
            /// Sorts the messages descending.
            /// </summary>
            DESC
        }
        #endregion

        #region public methods
        /// <summary>
        /// Examines a mailbox.
        /// </summary>
        /// <param name="mailbox">The name of the mailbox to examine.</param>
        /// <returns>Returns a mailbox object containing the properties of the mailbox.</returns>
        public ImapMailbox Examine(string mailbox) {
            ImapMailbox Mailbox = null;
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write("EXAMINE \"" + mailbox + "\"\r\n");
            Mailbox = ParseMailbox(mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Selects a mailbox to perform commands on.
        /// </summary>
        /// <param name="mailbox">The name of the mailbox to select.</param>
        /// <returns>Returns a mailbox object containing the properties of the mailbox.</returns>
        public ImapMailbox Select(string mailbox)
        {
            ImapMailbox Mailbox = null;
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write("SELECT \"" + mailbox + "\"\r\n");
            Mailbox = ParseMailbox(mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Obtains a sorted collection of messages from a mailbox.
        /// </summary>
        /// <param name="sort">A value of type SortMethod.</param>
        /// <param name="order">A value of type SortOrder that specifies ascending or descending.</param>
        /// <param name="records">An interger value containing the number of messages to return.</param>
        /// <param name="page">An integer value representing the page to display.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Sort(SortMethod sort, SortOrder order, int records, int page)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("SORT ({0}{1}) US-ASCII ALL\r\n", OrderToString(order), SortToString(sort)));
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                Connection.Read();
                MatchCollection matches = Regex.Matches(response, @"\d+");
                if (matches.Count > 0)
                {
                    int[] ids;
                    if ((page + 1) * records > matches.Count)
                    {
                        page = matches.Count / records;
                        ids = new int[matches.Count % records];
                    }
                    else
                        ids = new int[records];
                    for (int i = page * records; i < matches.Count && i < (page + 1) * records; i++)
                        ids[i - page * records] = Convert.ToInt16(matches[i].Value);
                    return Fetch(ids);
                }
            }
            return new ImapMailbox();

        }
        /// <summary>
        /// Obtains a sorted collection of messages from a mailbox.
        /// </summary>
        /// <param name="sort">A value of type SortMethod.</param>
        /// <param name="order">A value of type SortOrder that specifies ascending or descending.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Sort(SortMethod sort, SortOrder order)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("SORT ({0){1}) US-ASCII ALL\r\n", OrderToString(order), SortToString(sort)));
            string response = Connection.Read();
            if (response.StartsWith("*")) {
                Connection.Read();
                MatchCollection matches = Regex.Matches(response, @"\d+");
                if (matches.Count > 0) {
                    int[] ids = new int[matches.Count];
                    for (int i = 0; i < matches.Count; i++)
                        ids[i] = Convert.ToInt16(matches[i].Value);
                    return Fetch(ids);
                }
            }
            return new ImapMailbox();
        }
        /// <summary>
        /// Obtains message from a mailbox.
        /// </summary>
        /// <param name="begin">The first message to retreive.</param>
        /// <param name="end">The last message to retreive.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(int begin, int end)
        {
            ImapMailbox Mailbox = new ImapMailbox();
            return Fetch(Mailbox, begin, end);
        }
        /// <summary>
        /// Obtains messages from a mailbox.
        /// </summary>
        /// <param name="messages">A interger array of message ids.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(int[] messages)
        {
            ImapMailbox Mailbox = new ImapMailbox();
            return Fetch(Mailbox, messages);
        }
        /// <summary>
        /// Retreives message headers for a message.
        /// </summary>
        /// <param name="message">A integer representing the message id.</param>
        /// <returns>Returns a ImapMailboxMessage object.</returns>
        public ImapMailboxMessage FetchHeaders(int message)
        {
            ImapMailbox Mailbox = new ImapMailbox();
            return Fetch(Mailbox, message, message).Messages[0];
        }


        private string searchTheConnString(string res)
        {
            string response = "";

            string tmp =  res;
            if (res.IndexOf('{') >= 0)
            {
                string ts = "";
                do
                {
                    response = Connection.Read();
                    ts=ts+response;
                }
                while (!ts.EndsWith("completed."));

                do
                {
                    //ts = ts.Substring(0, ts.Length - 2);
                    /*if ((response.EndsWith("completed.") || Regex.IsMatch(response, patFetchComplete)))
                        break;*/
                    response = ts;
                    if (res.IndexOf('{') >= 0)
                    {
                        tmp = res.Substring(0, res.IndexOf('{'))+"\"";
                        int y = tmp.Length;
                        tmp += response;
                        int x = Convert.ToInt32(res.Substring(res.IndexOf('{')+1, res.IndexOf('}') - res.IndexOf('{')-1));
                        tmp = tmp.Substring(0, y + x) + "\"" + tmp.Substring(y+x);
                        res = tmp;
                        
                    }
                    
                }
                while (!(response.EndsWith("completed.") || Regex.IsMatch(response, patFetchComplete)));
            }
            return tmp ;
        }

        private string getCommand()
        {
            string response = "";
            string temp = "";
            int readNum = 0;
            do
            {
                response += temp;
                if (temp.IndexOf('{') != -1)
                {
                    readNum = Convert.ToInt32(temp.Substring(temp.IndexOf('{') + 1, temp.IndexOf('}') - temp.IndexOf('{') -1 ));
                    response = response.Substring(0, response.IndexOf('{') );
                }

                temp = Connection.Read();
                temp = temp.TrimStart(' ');
            }
            while (!(temp.Contains("completed.") || Regex.IsMatch(temp, patFetchComplete)));

            return response;
        }

        /// <summary>
        /// Retreives the bodystructure of a message.
        /// </summary>
        /// <param name="message">A ImapMailboxMessage object.</param>
        /// <returns>Returns an ImapMailboxMessage object.</returns>
        public ImapMailboxMessage FetchBodyStructure(ImapMailboxMessage message)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH {0} BODYSTRUCTURE\r\n", message.ID));
            //string response = Connection.Read();
            string response = getCommand();
            if (!response.StartsWith("* " + message.ID))
            {
                response = response.Substring(response.IndexOf("* " + message.ID), response.Length - response.IndexOf("* " + message.ID));
            }
            response = ImapDecode.Decode(response);

            response = searchTheConnString(response);

            if (response.StartsWith("*"))
            {
                response = response.Substring(response.IndexOf(" (", response.IndexOf("BODYSTRUCTURE")));
                message.Errors = response;
                message.BodyParts = BodyPartSplit(response.Trim().Substring(0, response.Trim().Length - 1));//
                //response = Connection.Read();
                for (int i = 0; i < message.BodyParts.Count && i < 2; i++)
                {
                    //ToLower added 
                    if (message.BodyParts[i].ContentType.MediaType == "TEXT/HTML")
                    {
                        message.HasHTML = true;
                        message.HTML = i;
                    }
                    //ToLower added 
                    else if (message.BodyParts[i].ContentType.MediaType == "TEXT/PLAIN")
                    {
                        message.HasHTML = true;
                        message.Text = i;
                    }
                }
                //response = Connection.Read();
                return message;
            }
            else
                throw new ImapCommandInvalidMessageNumber("No UID found for message number" + message.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearBuffer()
        {
            string response="";
            
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            do
            {
                response = Connection.Read();
            }
            while(response != null && !response.Equals("") && !response.EndsWith("completed."));

        }
        /// <summary>
        /// Retreives the content of a particular body part.
        /// </summary>
        /// <param name="message">A ImapMailboxMessage object.</param>
        /// <param name="part">A numeric value representing the body part in the message.</param>
        /// <returns>Returns an ImapMailboxMessage object.</returns>
        public ImapMailboxMessage FetchBodyPart(ImapMailboxMessage message, int part)
        {
            System.Text.Encoding encodingTR = System.Text.Encoding.GetEncoding("ISO-8859-9");
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH {0} BODY[{1}]\r\n", message.ID, message.BodyParts[part].BodyPart));
            string response = Connection.Read();

            if (!response.StartsWith("* " + message.ID))
            {
                response = response.Substring(response.IndexOf("* " + message.ID), response.Length - response.IndexOf("* " + message.ID));
            }

            response = ImapDecode.Decode(response);
            //response = Connection.Read();
            if (response.StartsWith("*"))
            {
                //TODO Bu satýr araþtýrýlacak.

                message.BodyParts[part].Data = ParseBodyPart(message.BodyParts[part].ContentEncoding, encodingTR);
                //message.BodyParts[part].Data = message.BodyParts[part].Data.Substring(0, (int) message.BodyParts[part].Size);
            }
            return message;
        }
        /// <summary>
        /// Obtains message from a mailbox.
        /// </summary>
        /// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
        /// <param name="begin">The first message to retreive.</param>
        /// <param name="end">The last message to retreive.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(ImapMailbox Mailbox, int begin, int end)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH {0}:{1} ALL\r\n", begin, end));
            ParseMessages(ref Mailbox);
            return Mailbox;
        }

        /// <summary>
        /// Obtains seen messages from a mailbox
        /// </summary>
        /// <param name="mailBox">The ImapMailbox object to add the messages to</param>
        /// <returns></returns>
        public void FindSeenMessageID(ImapMailbox mailBox)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("SEARCH SEEN\r\n"));
            ParseSearchCommand(ref mailBox);
        }


        /// <summary>
        /// Obtains unseen messages from a mailbox
        /// </summary>
        /// <param name="mailBox">The ImapMailbox object to add the messages to</param>
        /// <returns></returns>
        public void FindUnseenMessageID(ImapMailbox mailBox)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("SEARCH UNSEEN\r\n"));
            ParseSearchCommand(ref mailBox);
        }

        /// <summary>
        /// Obtains messages from a mailbox.
        /// </summary>
        /// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
        /// <param name="messages">A interger array of message ids.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(ImapMailbox Mailbox, int[] messages)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            string messagelist = string.Empty;


            for (int i = 0; i < messages.Length; i++)
            {
                if (i == 0)
                {
                    messagelist = messages[i].ToString();
                }
                else
                    messagelist += (i % 100 == 1) ? messages[i].ToString() : "," + messages[i];
                
                if ((i % 100) == 0 || i == messages.Length - 1)
                {
                    
                    Connection.Write(string.Format("FETCH {0} ALL\r\n", messagelist));
                    ParseMessages(ref Mailbox, messages);
                    messagelist = string.Empty;
                }
            }

            return Mailbox;
        }

        private void ParseMessages(ref ImapMailbox Mailbox, int[] messagelist)
        {
            string response = string.Empty;
            if (Mailbox.Messages == null)
                Mailbox.Messages = new List<ImapMailboxMessage>();
            do
            {
                response += Connection.Read();
            } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete)));

            try
            {
                response = response.Substring(response.IndexOf("* " + messagelist[0].ToString()), response.Length - response.IndexOf("* " + messagelist[0].ToString()));
            }
            catch
            { 
                
            }
            response = ImapDecode.Decode(response);

            if (response.StartsWith("*"))
            {
                do
                {
                    ImapMailboxMessage Message = new ImapMailboxMessage();
                    Message.Flags = new ImapMessageFlags();
                    Message.Addresses = new ImapAddressCollection();
                    Match match;
                    if ((match = Regex.Match(response, @"\* (\d*)")).Success)
                        Message.ID = Convert.ToInt32(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"\(FLAGS \(([^\)]*)\)")).Success)
                        Message.Flags.ParseFlags(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"INTERNALDATE ""([^""]+)""")).Success)
                        Message.Received = DateTime.Parse(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"RFC822.SIZE (\d+)")).Success)
                        Message.Size = Convert.ToInt32(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"ENVELOPE")).Success)
                        response = response.Remove(0, match.Index + match.Length);
                    if ((match = Regex.Match(response, @"\(""([^""]+)""")).Success)
                    {
                        Match subMatch;
                        subMatch = Regex.Match(match.Groups[1].ToString(), @"(-\d+.*|-\d+.*|NIL.*)"); //(-\d{4}|-\d{4}[^""]+|NIL)
                        DateTime d;
                        DateTime.TryParse(match.Groups[1].ToString().Remove(subMatch.Index), out d);
                        Message.Sent = d;
                        Message.TimeZone = subMatch.Groups[1].ToString();
                        response = response.Remove(0, match.Index + match.Length);
                    }
                    Message.Subject = response.Substring(0, response.IndexOf("((")).Trim();
                    if (Message.Subject == "NIL")
                        Message.Subject = null;
                    else if ((match = Regex.Match(Message.Subject, "^\"(.*)\"$")).Success)
                        Message.Subject = match.Groups[1].ToString();
                    Message.Subject = ImapDecode.Decode(Message.Subject);
                    response = response.Remove(0, response.Substring(0, response.IndexOf("((")).Length);

                    //if ((match = Regex.Match(response, @"(""[^""]*"" \(\(|NIL)")).Success)
                    //{
                    //    Message.Subject = match.Groups[1].ToString();
                    //    if (Message.Subject == "NIL")
                    //        Message.Subject = null;
                    //    else if (Message.Subject.StartsWith("\""))
                    //        Message.Subject = Message.Subject.Substring(1, Message.Subject.Length -5);
                    //    response = response.Remove(0, match.Index + match.Length - 3);
                    //}
                    if ((match = Regex.Match(response, @"""<([^>]+)>""\)\)")).Success)
                    {
                        Message.MessageID = match.Groups[1].ToString();
                        response = response.Remove(match.Index).Trim();
                    }
                    if (response.EndsWith("NIL"))
                        response = response.Remove(response.Length - 3);
                    else
                    {
                        match = Regex.Match(response, @"""<([^>]+)>""");
                        Message.Reference = match.Groups[1].ToString();
                    }
                    try
                    {
                        Message.Addresses = Message.Addresses.ParseAddresses(response);
                    }
                    catch (Exception ex)
                    {
                        Message.Errors = response + ex.ToString();
                    }
                    Mailbox.Messages.Add(Message);
                    response = string.Empty;
                    do
                    {
                        response += Connection.Read();
                    } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete)));
                } while (response.StartsWith("*"));

                //match = Regex.Match(response, @"\(FLAGS \(([\w\\]+)\) INTERNALDATE ""([^""]+)"" RFC822\.SIZE (\d+) ENVELOPE \(""([^""]+)"" ""([^""]+)"" \(\(NIL NIL ""([^""]+""\)\)");
            }
        }
        /// <summary>
        /// Obtains message from a mailbox.
        /// </summary>
        /// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(ImapMailbox Mailbox)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH 1:* ALL\r\n"));
            ParseMessages(ref Mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Converts a message number to the server message UID.
        /// </summary>
        /// <param name="messageNumber">The message number to convert.</param>
        /// <returns>Returns the server UID of the message.</returns>
        public int FetchUID(int messageNumber)
        {
            Connection.Write(string.Format("FETCH {0} UID\r\n", messageNumber));
            string response = Connection.Read();
            int uid = 0;
            if (response.StartsWith("*"))
            {
                Match match = Regex.Match(response, @"\(UID (\d+)\)");
                uid = Convert.ToInt32(match.Groups[1].ToString());
                Connection.Read();
                return uid;
            }
            else
                throw new ImapCommandInvalidMessageNumber("No UID found for message number" + messageNumber);
        }
        /// <summary>
        /// Sets the seen flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetSeen(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Seen", value);
        }
        /// <summary>
        /// Sets the answered flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetAnswered(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Answered", value);
        }
        /// <summary>
        /// Sets the flagged flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetFlagged(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Flagged", value);
        }
        /// <summary>
        /// Sets the deleted flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetDeleted(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Deleted", value);
        }
        /// <summary>
        /// Sets the draft flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetDraft(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Draft", value);
        }
        /// <summary>
        /// Sets the recent flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetRecent(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Recent", value);
        }
        #endregion

        #region private methods

        private bool SetFlag(int uid, string flag, bool append)
        {
            string method = null;
            if (append)
                method = "+flags";
            else
                method = "-flags";
            Connection.Write(string.Format("UID STORE {0} {1} ({2})\r\n", uid.ToString(), method, flag));
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                Connection.Read();
                return true;
            }
            else
                return false;
        }

        private void ParseSearchCommand(ref ImapMailbox mailbox)
        {
            string response = string.Empty;
            int[] messagelist =null;
            if (mailbox.Messages == null)
                mailbox.Messages = new List<ImapMailboxMessage>();
            do
            {
                response = Connection.Read();

                if(response.EndsWith("completed."))
                    break;

                if (response.StartsWith("* SEARCH "))
                {

                    string msgList = response.Substring(9, response.Length-9);
                    char[] delimiterChars = { ' ' };
                    string[] ids = msgList.Split(delimiterChars);
                    messagelist =  new int [ids.GetLength(0)];
                    int i = 0;
                    foreach (string s in ids)
                    {
                        messagelist[i] = System.Convert.ToInt32(s);
                        i++;
                    }
                }
            } while (!(response.EndsWith("completed.") || Regex.IsMatch(response, patFetchComplete)));

            if (messagelist != null)
            {
                mailbox = Fetch(mailbox, messagelist);
            }

        }


        private void ParseMessages(ref ImapMailbox Mailbox)
        {
            string response = string.Empty;
            if (Mailbox.Messages == null)
                Mailbox.Messages = new List<ImapMailboxMessage>();
            do
            {
                response += Connection.Read();
            } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete)));

             
            response = ImapDecode.Decode(response);

            if (response.StartsWith("*"))
            {
                do
                {
                    ImapMailboxMessage Message = new ImapMailboxMessage();
                    Message.Flags = new ImapMessageFlags();
                    Message.Addresses = new ImapAddressCollection();
                    Match match;
                    if ((match = Regex.Match(response, @"\* (\d*)")).Success)
                        Message.ID = Convert.ToInt32(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"\(FLAGS \(([^\)]*)\)")).Success)
                        Message.Flags.ParseFlags(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"INTERNALDATE ""([^""]+)""")).Success)
                        Message.Received = DateTime.Parse(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"RFC822.SIZE (\d+)")).Success)
                        Message.Size = Convert.ToInt32(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"ENVELOPE")).Success)
                        response = response.Remove(0, match.Index + match.Length);
                    if ((match = Regex.Match(response, @"\(""([^""]+)""")).Success)
                    {
                        Match subMatch;
                        subMatch = Regex.Match(match.Groups[1].ToString(), @"(-\d+.*|-\d+.*|NIL.*)"); //(-\d{4}|-\d{4}[^""]+|NIL)
                        DateTime d;
                        DateTime.TryParse(match.Groups[1].ToString().Remove(subMatch.Index), out d);
                        Message.Sent = d;
                        Message.TimeZone = subMatch.Groups[1].ToString();
                        response = response.Remove(0, match.Index + match.Length);
                    }
                    Message.Subject = response.Substring(0, response.IndexOf("((")).Trim();
                    if (Message.Subject == "NIL")
                        Message.Subject = null;
                    else if ((match = Regex.Match(Message.Subject, "^\"(.*)\"$")).Success)
                        Message.Subject = match.Groups[1].ToString();
                    Message.Subject = ImapDecode.Decode(Message.Subject);
                    response = response.Remove(0, response.Substring(0, response.IndexOf("((")).Length);

                    //if ((match = Regex.Match(response, @"(""[^""]*"" \(\(|NIL)")).Success)
                    //{
                    //    Message.Subject = match.Groups[1].ToString();
                    //    if (Message.Subject == "NIL")
                    //        Message.Subject = null;
                    //    else if (Message.Subject.StartsWith("\""))
                    //        Message.Subject = Message.Subject.Substring(1, Message.Subject.Length -5);
                    //    response = response.Remove(0, match.Index + match.Length - 3);
                    //}
                    if ((match = Regex.Match(response, @"""<([^>]+)>""\)\)")).Success)
                    {
                        Message.MessageID = match.Groups[1].ToString();
                        response = response.Remove(match.Index).Trim();
                    }
                    if (response.EndsWith("NIL"))
                        response = response.Remove(response.Length - 3);
                    else {
                        match = Regex.Match(response, @"""<([^>]+)>""");
                        Message.Reference = match.Groups[1].ToString();
                    }
                    try
                    {
                        Message.Addresses = Message.Addresses.ParseAddresses(response);
                    }
                    catch (Exception ex)
                    {
                        Message.Errors = response + ex.ToString();
                    }
                    Mailbox.Messages.Add(Message);
                    response = string.Empty;
                    do
                    {
                        response += Connection.Read();
                    } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete)));
                } while (response.StartsWith("*"));

                //match = Regex.Match(response, @"\(FLAGS \(([\w\\]+)\) INTERNALDATE ""([^""]+)"" RFC822\.SIZE (\d+) ENVELOPE \(""([^""]+)"" ""([^""]+)"" \(\(NIL NIL ""([^""]+""\)\)");
            }
        }

        private string ParseBodyPart(IMAP.BodyPartEncoding encoding)
        {
            return ParseBodyPart(encoding, null);
        }

        private string ParseBodyPart(IMAP.BodyPartEncoding encoding, Encoding en)
        {
            string response;
            en = System.Text.Encoding.GetEncoding("ISO-8859-9");
            StringBuilder sb = new StringBuilder("");
            do
            {
                response = Connection.Read();

                if (response.IndexOf("FLAGS") != -1)
                    continue;
                if (Regex.IsMatch(response, patFetchComplete))
                    break;
                if (encoding == IMAP.BodyPartEncoding.BASE64)
                    sb.Append(response + '\r' + '\n');
                else if (encoding == IMAP.BodyPartEncoding.QUOTEDPRINTABLE)
                {
                    response = response.Replace("=FC", "ü");
                    response = response.Replace("=DC", "Ü");
                    response = response.Replace("=F0", "ð");
                    response = response.Replace("=D0", "Ð");
                    response = response.Replace("=F6", "ö");
                    response = response.Replace("=D6", "Ö");
                    response = response.Replace("=E7", "ç");
                    response = response.Replace("=C7", "Ç");
                    response = response.Replace("=FD", "ý");
                    response = response.Replace("=DD", "Ý");
                    response = response.Replace("=FE", "þ");
                    response = response.Replace("=DE", "Þ");
                    response = response.Replace("=3D", "=");
                    response = response.Replace("_", " ");
                    response = response.Replace("=5F", "_");


                    if (response.EndsWith("=") || response.EndsWith(")"))
                        sb.Append(response.Substring(0, response.Length - 1));
                    else
                        sb.AppendLine(response);
                }
                else
                    sb.AppendLine(response + '\r' + '\n');
            } while (true);
            //} while (!(response.EndsWith("==") || response == ")"));
            if (sb.ToString().Trim().EndsWith(")"))
                sb = sb.Remove(sb.ToString().IndexOf(")"), 1);
            if (encoding != BodyPartEncoding.BASE64)
                return ImapDecode.Decode(sb.ToString(), en);

           // return sb.ToString().Substring(0,sb.Length-15);
            
            //----------------------------------//
            return sb.ToString();
            //----------------------------------//
        }

        private ImapMailbox ParseMailbox(string mailbox)
        {
            ImapMailbox Mailbox = null;
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                Mailbox = new ImapMailbox(mailbox);
                Mailbox.Flags = new ImapMessageFlags();
                do
                {
                    Match match;
                    if ((match = Regex.Match(response, @"(\d+) EXISTS")).Success)
                        Mailbox.Exist = Convert.ToInt32(match.Groups[1].ToString());
                    else if ((match = Regex.Match(response, @"(\d+) RECENT")).Success)
                        Mailbox.Recent = Convert.ToInt32(match.Groups[1].ToString());
                    else if ((match = Regex.Match(response, @" FLAGS \((.*?)\)")).Success)
                        Mailbox.Flags.ParseFlags(match.Groups[1].ToString());
                    response = Connection.Read();
                } while (response.StartsWith("*"));
                if ((response.StartsWith("OK") || response.Substring(7, 2) == "OK") && (response.ToUpper().Contains("READ/WRITE") || response.ToUpper().Contains("READ-WRITE")))
                    Mailbox.ReadWrite = true;
            }
            return Mailbox;
        }

        private ImapMessageBodyPartList 
            BodyPartSplit(string response)
        {
            ImapMessageBodyPartList Parts = new ImapMessageBodyPartList();
            int i = 0;
            int index = 1;
            int count = 0;
            do
            {
                int next = index;
                do
                {
                    if (response[next] == '(')
                        i++;
                    else if (response[next] == ')')
                        i--;
                    next++;
                } while (i > 0 || response[next - 1] != ')');
                if (i >= 0 && response[index] == '(')
                {
                    count++;
                    // Parse nested body parts
                    if (response.Substring(index, next - index).StartsWith("(("))
                    {
                        ImapMessageBodyPartList temp = BodyPartSplit(response.Substring(index, next));
                        for (int j = 0; j < temp.Count; j++)
                        {
                            temp[j].BodyPart = count.ToString() + "." + temp[j].BodyPart;
                            Parts.Add(temp[j]);
                        }
                    }
                    else
                    {
                        ImapMessageBodyPart Part = new ImapMessageBodyPart(response.Substring(index, next - index));
                        Part.BodyPart = count.ToString();
                        Parts.Add(Part);
                    }
                }
                else if(Parts.Count == 0)
                {
                    ImapMessageBodyPart Part = new ImapMessageBodyPart(response);
                    Part.BodyPart = "1";
                    Parts.Add(Part);
                }
                index = next;
            } while (i >= 0);
            return Parts;
        }

        private string SortToString(SortMethod sort)
        {
            switch (sort)
            {
                case SortMethod.ARRIVAL: return "ARRIVAL";
                case SortMethod.CC: return "CC";
                case SortMethod.DATE: return "DATE";
                case SortMethod.FROM: return "FROM";
                case SortMethod.SIZE: return "SIZE";
                case SortMethod.SUBJECT: return "SUBJECT";
                default: return string.Empty;
            }
        }

        private string OrderToString(SortOrder order)
        {
            if (order == SortOrder.DESC)
                return "REVERSE ";
            return string.Empty;
        }

        private void NoOpenConnection()
        {
            throw new ImapConnectionException("Connection must be open before commands can be performed.");
        }
        #endregion
    }
}
