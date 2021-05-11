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
    /// Represents an Imap Body Part
    /// </summary>
    #endregion
    public class ImapMessageBodyPart
    {
        #region constants
        const string non_attach = "^\\((?<type>(\"[^\"]*\"|NIL))\\s(?<subtype>(\"[^\"]*\"|NIL))\\s(?<attr>(\\([^\\)]*\\)|NIL))\\s(?<id>(\"[^\"]*\"|NIL))\\s(?<desc>(\"[^\"]*\"|NIL))\\s(?<encoding>(\"[^\"]*\"|NIL))\\s(?<size>(\\d+|NIL))\\s(?<lines>(\\d+|NIL))\\s(?<md5>(\"[^\"]*\"|NIL))\\s(?<disposition>(\\([^\\)]*\\)|NIL))\\s(?<lang>(\"[^\"]*\"|NIL))\\)$";
        const string attachment = "^\\((?<type>(\"[^\"]*\"|NIL))\\s(?<subtype>(\"[^\"]*\"|NIL))\\s(?<attr>(\\([^\\)]*\\)|NIL))\\s(?<id>(\"[^\"]*\"|NIL))\\s(?<desc>(\"[^\"]*\"|NIL))\\s(?<encoding>(\"[^\"]*\"|NIL))\\s(?<size>(\\d+|NIL))\\s((?<data>(.*))\\s|)(?<lines>(\"[^\"]*\"|NIL))\\s(?<disposition>((?>\\((?<LEVEL>)|\\)(?<-LEVEL>)|(?!\\(|\\)).)+(?(LEVEL)(?!))|NIL))\\s(?<lang>(\"[^\"]*\"|NIL))\\)$";
        #endregion
        
        #region private variables
        System.Net.Mime.ContentType _ContentType = new System.Net.Mime.ContentType();
        bool _attachment;
        string _contentid;
        string _contentdescription;
        BodyPartEncoding _encoding;
        long _size;
        long _lines;
        string _hash;
        int index = 0;
        string _data;
        string _language;
        string _bodytype;
        string _filename;
        string _bodypart;
        string _disposition;
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets a boolean value representing if the body part is an attachment.
        /// </summary>
        public bool Attachment
        {
            set { _attachment = value; }
            get { return _attachment; }
        }
        /// <summary>
        /// Gets or sets the message data (Encoded)
        /// </summary>
        public string Data
        {
            set { _data = value; }
            get { return _data; }
        }
        /// <summary>
        /// Gets the message data in binary format.
        /// </summary>
        public byte[] DataBinary
        {
            get { return Convert.FromBase64String(_data); }
        }
        /// <summary>
        /// Gets the text that identfies the body part of the message for retevial from the server.
        /// </summary>
        public string BodyPart
        {
            internal set { _bodypart = value; }
            get { return _bodypart; }
        }
        /// <summary>
        /// Gets or sets the file name of an attachment.
        /// </summary>
        public string FileName 
        {
            set {
                try
                {
                    System.Text.Encoding encodingTR = System.Text.Encoding.GetEncoding("ISO-8859-9");
                    string tmp = value;
                    byte[] b = System.Convert.FromBase64String(tmp);
                    tmp = encodingTR.GetString(b);
                    _filename = tmp;
                }
                catch
                {
                    _filename = value; 
                }
                
            
            }
            get { return _filename; }
        }
        /// <summary>
        /// Gets or sets the content type of an body type.
        /// </summary>
        public System.Net.Mime.ContentType ContentType
        {
            set { _ContentType = value; }
            get { return _ContentType; }
        }
        /// <summary>
        /// Gets or sets the content ID.
        /// </summary>
        public string ContentID
        {
            set { _contentid = value; }
            get { return _contentid; }
        }
        /// <summary>
        /// Gets or sets the content description.
        /// </summary>
        public string ContentDescription
        {
            set { _contentdescription = value; }
            get { return _contentdescription; }
        }
        /// <summary>
        /// Gets or sets the encoding of body part.
        /// </summary>
        public BodyPartEncoding ContentEncoding
        {
            set { _encoding = value; }
            get { return _encoding; }
        }
        /// <summary>
        /// Gets the encoding type of a message.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                try
                {
                    return System.Text.Encoding.GetEncoding(this.ContentType.CharSet.Split('/')[1]);
                }
                catch (NullReferenceException)
                {
                    return Encoding.Default;
                }
            }
        }
        /// <summary>
        /// Gets or sets the size of body part.
        /// </summary>
        public long Size
        {
            set { _size = value; }
            get { return _size; }
        }
        /// <summary>
        /// Gets or sets the number of lines in a body part.
        /// </summary>
        public long Lines
        {
            set { _lines = value; }
            get { return _lines; }
        }
        /// <summary>
        /// Gets or sets the MD5 hash of a body part.
        /// </summary>
        public string ContentMD5
        {
            set { _hash = value; }
            get { return _hash; }
        }
        /// <summary>
        /// Gets or sets the content language of a body part.
        /// </summary>
        public string ContentLanguage
        {
            set { _language = value; }
            get { return _language; }
        }
        /// <summary>
        /// Gets or sets the body type of a body part.
        /// </summary>
        public string BodyType 
        { 
            set { _bodytype = value; }
            get { return _bodytype; }
        }
        /// <summary>
        /// Gets or sets the content disposition.
        /// </summary>
        public string Disposition {
            set { _disposition = value; }
            get { return _disposition; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an instance of the IampMessageBodyPart class.
        /// </summary>
        /// <param name="data">A string containing the message headers for a body part.</param>
        public ImapMessageBodyPart(string data)
        {
            
            string tFileName = "";
            Match match;
            bool control = false;
            if ((match = Regex.Match(data, non_attach, RegexOptions.ExplicitCapture)).Success && !data.Contains("\"name\""))
            {
                this.Attachment = false;
                this.ContentType.MediaType = string.Format("{0}/{1}", match.Groups["type"].Value.Replace("\"", ""), match.Groups["subtype"].Value.Replace("\"", ""));
                ParseCharacterSet(ParseNIL(match.Groups["attr"].Value));
                this.ContentID = ParseNIL(match.Groups["id"].Value);
                this.ContentDescription = ParseNIL(match.Groups["desc"].Value);
                this.ContentEncoding = ParseEncoding(ParseNIL(match.Groups["encoding"].Value));
                this.Size = Convert.ToInt64(ParseNIL(match.Groups["size"].Value));
                this.Lines = Convert.ToInt64(ParseNIL(match.Groups["lines"].Value));
                this.ContentMD5 = ParseNIL(match.Groups["md5"].Value);
                this.Disposition = ParseNIL(match.Groups["disposition"].Value);
                this.ContentLanguage = ParseNIL(match.Groups["lang"].Value);
                control = true;
            }
            else
            {
                if (data.IndexOf("name") != -1)
                {
                    int num = 0;

                    int x = data[data.IndexOf("name")+6] == '"' ?data.IndexOf("name")+7:data.IndexOf("name")+6;
                    for(; x < data.Length; x++)
                    {
                        if (data[x] == '(')
                        {
                            tFileName = tFileName + data[x];
                            num++;
                        }
                        else if (num > 0 && data[x] == ')')
                        {
                            tFileName = tFileName + data[x];
                            num--;
                        }
                        else if (num == 0 && data[x] == ')')
                            break;
                        else if (data[x] == '"')
                            break;
                        else
                            tFileName = tFileName + data[x];
                    }
                    tFileName = tFileName.TrimStart().TrimEnd();
                    if (data.IndexOf(("\"" + tFileName + "\"")) != -1)
                        data = data.Replace(("\"" + tFileName + "\""), "\"temp.txt\"");
                    else
                        data = data.Replace(tFileName,"\"temp.txt\"");
                    
                    //if(data[data.IndexOf("name")+7] == '\"')
                      //  tFileName = data.Substring(data.IndexOf("name")+7,)

                }
            }
            if (!control && (match = Regex.Match(data, attachment, RegexOptions.ExplicitCapture)).Success)
            {
                this.Attachment = true;
                this.ContentType.MediaType = string.Format("{0}/{1}", match.Groups["type"].Value.Replace("\"", ""), match.Groups["subtype"].Value.Replace("\"", ""));
                ParseFileName(tFileName);
                this.ContentID = ParseNIL(match.Groups["id"].Value);
                this.ContentDescription = ParseNIL(match.Groups["desc"].Value);
                this.ContentEncoding = ParseEncoding(ParseNIL(match.Groups["encoding"].Value));
                this.Size = Convert.ToInt64(ParseNIL(match.Groups["size"].Value));
                this.Lines = Convert.ToInt64(ParseNIL(match.Groups["lines"].Value));
                this.Disposition = ParseNIL(match.Groups["disposition"].Value);
                this.ContentLanguage = ParseNIL(match.Groups["lang"].Value);
            }
            else if (!control)
                throw new Exception("Invalid format could not parse body part headers.");

        }
        #endregion

        #region private methods
        private void ParseContentType(string data)
        {
            string[] part = new string[2];
            part[0] = data.Substring(data.IndexOf("\"") + 1, data.IndexOf("\"", data.IndexOf("\"") + 1) - (data.IndexOf("\"") + 1));
            part[1] = data.Substring(data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1, data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1) - (data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1));
            this.ContentType.MediaType = string.Format("{0}/{1}", part[0], part[1]);
            index = data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1) + 1;
        }
        private void ParseCharacterSet(string data)
        {
            if (data != null)
            {

                Match match = Regex.Match(data, "\"charset\"\\s\"(?<set>([^\"]*))\"", RegexOptions.ExplicitCapture);
                if (match.Success)
                    this.ContentType.CharSet = string.Format("charset/{0}", match.Groups["set"].Value);
            }
        }
        private void ParseFileName(string data)
        {
            if (data != null)
            {

                /*try
                {
                    
                    string temp = data;    
                    System.Text.Encoding encodUTF = System.Text.Encoding.GetEncoding("UTF-8");
                    System.Text.Encoding encodIso = System.Text.Encoding.GetEncoding("ISO-8859-9");
                    byte[] t = encodIso.GetBytes(temp);
                    temp = encodUTF.GetString(t);
                    data = temp;
                }
                catch
                { 
                    
                }*/
                this.FileName = data;
                /*Match match = Regex.Match(data, "\"name\"\\s\"(?<file>([^\"]*))\"", RegexOptions.ExplicitCapture);
                if (match.Success)
                {
                    this.FileName = match.Groups["file"].Value;
                }*/
            }
        }
        private string ParseNIL(string data)
        {
            if (data.Trim() == "NIL")
                return null;
            return data;
        }
        private BodyPartEncoding ParseEncoding(string data)
        {
            if (data == null)
                return BodyPartEncoding.UNKNOWN;
            data = data.Replace("\"", "").ToUpper();
            switch (data.Substring(0,1))
            {
                case "7": return BodyPartEncoding.UTF7;
                case "8":
                    if (data.Substring(1).CompareTo("BIT") == 0)
                        return BodyPartEncoding.UNKNOWN;
                    return BodyPartEncoding.UTF8;
                case "B":
                    if (data.CompareTo("BASE64") == 0)
                        return BodyPartEncoding.BASE64;
                    else if (data.CompareTo("BINARY") == 0)
                        return BodyPartEncoding.NONE;
                    else
                        return BodyPartEncoding.UNKNOWN;
                case "Q":
                    if (data.CompareTo("QUOTED-PRINTABLE") == 0)
                        return BodyPartEncoding.QUOTEDPRINTABLE;
                    return BodyPartEncoding.UNKNOWN;
                default:
                    return BodyPartEncoding.UNKNOWN;
            }
        }
        #endregion
    }
}
