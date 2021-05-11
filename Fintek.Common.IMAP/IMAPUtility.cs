using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Fintek.Common;
using Fintek.Common.Security;
using System.IO;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace Fintek.Common.IMAP
{   
    /// <summary>
    /// 
    /// </summary>
    public class IMAPUtility
    {
        #region Private Members
        
        private string m_UserName;
        private string m_Password;
        private string m_Domain;
        private string m_Host;
        private string m_Port;
        private StringBuilder m_CreatedFileList;
        private ArrayList m_SavedMails;
        private ImapConnect m_Connection    = null;
        private ImapCommand m_ImapCommand   = null;
        private ImapMailbox m_ImapMailbox   = null;
        private DataTable m_FilterTable     = null;
        #endregion Private Members
        
        #region Constants
        private string CONFIG_FILE_NAME = "_MailMessageConfig.xml";
        private string FILTER_FILE_NAME = "_MailMessageFilter.xml";
        #endregion Constants

        #region Private Members
        private MailMessageFilter[] m_MailMessageFilters = null;


        #endregion Private Members

        #region Public Members

        
        public StringBuilder CreatedFileList
        {
            get { return m_CreatedFileList; }
        }

        /// <summary>
        /// gets MailFilter []value
        /// </summary>
        public MailMessageFilter[] MailMessageFilters
        {
            get { return m_MailMessageFilters; }
            set { m_MailMessageFilters = value; }

        }

        /// <summary>
        /// Saved Mail's list. It contains SavedMail objects.
        /// </summary>
        public ArrayList SavedMail
        {
            get { return m_SavedMails; }
        }
        
        #endregion Public Members

        #region Constructors

        /// <summary>
        /// Reads from FilterConfig file. If there is not a config file then creates an empty filter.
        /// </summary>
        public IMAPUtility()
        {
            CONFIG_FILE_NAME = "CONFIG\\" + Process.GetCurrentProcess().ProcessName.Substring(0, Process.GetCurrentProcess().ProcessName.IndexOf(".")) + CONFIG_FILE_NAME;
            FILTER_FILE_NAME = "CONFIG\\" + Process.GetCurrentProcess().ProcessName.Substring(0, Process.GetCurrentProcess().ProcessName.IndexOf(".")) + FILTER_FILE_NAME;
            m_SavedMails = new ArrayList();
            arrangeDataTable();
            m_CreatedFileList = new StringBuilder("");
            getLoginData();
            loadFilterFromFile();
        }

        /// <summary>
        /// Takes filters from user. Not Read from Config files. Must give at least one filter and login data.
        /// </summary>
        /// <param name="userName">Imap server user name</param>
        /// <param name="password">Imap server user password</param>
        /// <param name="domain">User active diractory domain name</param>
        /// <param name="serverIP">Imap server IP</param>
        /// <param name="serverPort">Imap server port</param>
        /// <param name="mailMessageFilters">Filters</param>
        public IMAPUtility(string userName,string password, string domain, string serverIP, string serverPort, MailMessageFilter[] mailMessageFilters)
        {
            arrangeDataTable();
            m_CreatedFileList = new StringBuilder("");
            m_Domain = domain;
            m_UserName = userName;
            m_Password = password;
            m_Host = serverIP;
            m_Port = serverPort;
            m_SavedMails = new ArrayList();
            //getLoginData();
            m_MailMessageFilters = mailMessageFilters;
        }

        #endregion Constructors

        #region Private Methods

        private void arrangeDataTable()
        {
            m_FilterTable = new DataTable("FILTERS");
            m_FilterTable.Columns.Add(new DataColumn("From"));
            m_FilterTable.Columns.Add(new DataColumn("Subject"));
            m_FilterTable.Columns.Add(new DataColumn("FileName"));
            m_FilterTable.Columns.Add(new DataColumn("DestinationPath"));
        }

        /// <summary>
        /// 
        /// </summary>
        private void loadFilterFromFile()
        {
            String theFile = AppDomain.CurrentDomain.BaseDirectory + FILTER_FILE_NAME;
            try
            {
                
                DataSet ds = new DataSet();
                ds.ReadXml(theFile);
                m_FilterTable = ds.Tables[0];
                m_MailMessageFilters = new MailMessageFilter[m_FilterTable.Rows.Count];
                

                for (int i = 0; i < m_FilterTable.Rows.Count; i++)
                {
                    m_MailMessageFilters[i] = new MailMessageFilter();
                    m_MailMessageFilters[i].From = m_FilterTable.Rows[i]["From"].ToString();
                    m_MailMessageFilters[i].Subject = m_FilterTable.Rows[i]["Subject"].ToString();
                    m_MailMessageFilters[i].FileName = m_FilterTable.Rows[i]["FileName"].ToString();
                    m_MailMessageFilters[i].DestinationPath = m_FilterTable.Rows[i]["DestinationPath"].ToString();
                }
            }
            catch (XmlException xEx)
            {
                throw new Exception("Mail Filter parametre dosyası uygun formatta değil! Kontrol ediniz.", xEx);
            }
            catch (FileNotFoundException fex)
            {
                throw new Exception(theFile + " parametre dosyası bulunamadı. Lütfen kontrol ediniz!", fex);
            }
            catch (Exception ex)
            {
                throw new Exception("Mail Filter Parametre dosyasının okunması sırasında hata oluştu!", ex);
            }
            
        }

        /// <summary>
        /// Gets the login data from file.
        /// </summary>
		private  void getLoginData()
		{
            String theFile = AppDomain.CurrentDomain.BaseDirectory + CONFIG_FILE_NAME;
			XmlDocument xmlDoc = new XmlDocument();

			try
			{
				xmlDoc.Load(theFile);
                XmlNodeList login = xmlDoc.GetElementsByTagName("LoginInformation");

				for (int y = 0; y < login[0].ChildNodes.Count; y++)
				{
                    if (login[0].ChildNodes[y].Name.ToString().Equals("UserCode"))
                    {
                        m_UserName = login[0].ChildNodes[y].InnerText.ToString();
                    }
                    else if (login[0].ChildNodes[y].Name.ToString().Equals("UserPassword"))
                    {
                        m_Password = login[0].ChildNodes[y].InnerText.ToString();
                    }
                    else if (login[0].ChildNodes[y].Name.ToString().Equals("UserDomain"))
                    {
                        m_Domain = login[0].ChildNodes[y].InnerText.ToString();
                    }
                    else if (login[0].ChildNodes[y].Name.ToString().Equals("IMAPServerIP"))
                    {
                        m_Host = login[0].ChildNodes[y].InnerText.ToString();
                    }
                    else if (login[0].ChildNodes[y].Name.ToString().Equals("IMAPPort"))
                    {
                        m_Port = login[0].ChildNodes[y].InnerText.ToString();
                    }
                }
			}
			catch (XmlException xEx)
			{
                throw new Exception("Parametre dosyası uygun formatta değil. Kontrol ediniz.", xEx);
			}
            catch (FileNotFoundException fex)
            {
                throw new Exception(theFile + " parametre dosyası bulunamadı. Lütfen kontrol ediniz!", fex);
            }
			catch (Exception ex)
			{
                throw new Exception("Parametre dosyasının okunması sırasında hata oluştu!", ex);
			}

        }

        /// <summary>
        /// 
        /// </summary>
        private void createFile(ImapMessageBodyPart imapMessageBodyPart, MailMessageFilter mailMessageFilter,SavedMails sm)
        {
             string message = "";
            try
            {
               
                if (imapMessageBodyPart.FileName.Equals(""))
                {
                    return;
                }

                System.Text.Encoding encodingTR = System.Text.Encoding.GetEncoding("ISO-8859-9");
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();



                if (imapMessageBodyPart.FileName.Contains("?"))
                {
                    
                    string[] dm = imapMessageBodyPart.FileName.Split('?');
                    message = imapMessageBodyPart.FileName;
                    dm[3] = dm[3].Replace("=FC", "ü");
                    dm[3] = dm[3].Replace("=DC", "Ü");
                    dm[3] = dm[3].Replace("=F0", "ğ");
                    dm[3] = dm[3].Replace("=D0", "Ğ");
                    dm[3] = dm[3].Replace("=F6", "ö");
                    dm[3] = dm[3].Replace("=D6", "Ö");
                    dm[3] = dm[3].Replace("=E7", "ç");
                    dm[3] = dm[3].Replace("=C7", "Ç");
                    dm[3] = dm[3].Replace("=FD", "ı");
                    dm[3] = dm[3].Replace("=DD", "İ");
                    dm[3] = dm[3].Replace("=FE", "ş");
                    dm[3] = dm[3].Replace("=DE", "Ş");
                    dm[3] = dm[3].Replace("_", " ");
                    dm[3] = dm[3].Replace("=5F", "_");
                    
                    imapMessageBodyPart.FileName = dm[3];
                }
                string temp = "";
                try
                {
                    temp = imapMessageBodyPart.FileName.Substring(0, imapMessageBodyPart.FileName.LastIndexOf('.'));
                    temp = temp + "_" + DateTime.Today.Year + DateTime.Today.Month.ToString().PadLeft(2, '0') + DateTime.Today.Day.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                    temp += imapMessageBodyPart.FileName.Substring(imapMessageBodyPart.FileName.LastIndexOf('.'), imapMessageBodyPart.FileName.Length - imapMessageBodyPart.FileName.LastIndexOf('.'));
                }
                catch
                {
                    sm.ExceptionType = 1;
                    sm.SavedFiles.Add(new SavedFiles());
                    ((SavedFiles)sm.SavedFiles[sm.SavedFiles.Count-1]).FileName = "";
                    ((SavedFiles)sm.SavedFiles[sm.SavedFiles.Count - 1]).Size = 0;
                    return;
                }
                //nevzat:path.combine şeklinde değiştirdim.
                if(!Directory.Exists(mailMessageFilter.DestinationPath))
                    Directory.CreateDirectory(mailMessageFilter.DestinationPath);

                string fullPath = Path.Combine(mailMessageFilter.DestinationPath, temp);//mailMessageFilter.DestinationPath.EndsWith("\\") ? mailMessageFilter.DestinationPath : (mailMessageFilter.DestinationPath + "\\") + temp;
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Append),encodingTR))
                {
                    
                    byte[] b;
                    string result = "";
                    try
                    {
                        if (imapMessageBodyPart.Data.Length != 0)
                            b = System.Convert.FromBase64String(imapMessageBodyPart.Data.Substring(0, imapMessageBodyPart.Data.Length - 1));
                        else
                            b = System.Convert.FromBase64String("");
                        result = encodingTR.GetString(b);
                    }
                    catch 
                    {
                        result = imapMessageBodyPart.Data;
                    }
                    
                    binWriter.Write(encodingTR.GetBytes(result));
                    binWriter.Close();
                    //nevzat: maildeki isim değil, kaydedilen dosya adı şeklinde değiştirdim
                    m_CreatedFileList.AppendLine(fullPath /*imapMessageBodyPart.FileName*/);
                    sm.SavedFiles.Add(new SavedFiles());
                    ((SavedFiles)sm.SavedFiles[sm.SavedFiles.Count - 1]).FileName = temp;
                    ((SavedFiles)sm.SavedFiles[sm.SavedFiles.Count - 1]).Size = result.Length;
                }
            }
            catch (Exception ex)
            {
                sm.ExceptionType = 1;
                sm.SavedFiles.Add(new SavedFiles());
                ((SavedFiles)sm.SavedFiles[sm.SavedFiles.Count - 1]).FileName = "";
                ((SavedFiles)sm.SavedFiles[sm.SavedFiles.Count - 1]).Size = 0;
                return;
            }
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Saves the new mail attachements to the given path
        /// </summary>
        public void SaveAttachements()
        {

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR", false);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR", false);


            if (m_MailMessageFilters.Length == 0)
            {
                throw new Exception("Okunacak mail filtreleri sisteme kaydedilmemiştir.");
            }
            try
            {
                m_Connection = new ImapConnect();
                m_Connection.Hostname = m_Host;
                m_Connection.Port = Convert.ToInt32(m_Port);
                m_Connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("IMAP Server bağlantısı gerçekleştirilemedi!", ex);
            }
            try
            {
                ImapAuthenticate imapAuthanticate = new ImapAuthenticate(m_Connection, m_UserName, new Crypto().Decrypt(m_Password));

                try
                {
                    imapAuthanticate.Login();
                }
                catch (Exception exd)
                {
                    throw new Exception("Kullanıcı login sırasında hata oluştu. Lütfen kullanıcı adı ve parolayı tekrar gözden geçiriniz.", exd);
                }
                m_ImapMailbox = new ImapMailbox("inbox");
                m_ImapCommand = new ImapCommand(m_Connection);
                m_ImapCommand.Select(m_ImapMailbox.Mailbox);

                ImapMailboxMessage imapMailboxMessage = new ImapMailboxMessage();
                m_ImapCommand.FindUnseenMessageID(m_ImapMailbox);

                foreach (ImapMailboxMessage msg in m_ImapMailbox.Messages)
                {
                    //nevzat: bazen msg.from null geliyor, şimdilik bu mailleri atlayalım
                    if (msg == null || msg.From == null)
                    {
                        /*string d = msg.From + " - " + msg.Received + " - " + msg.HTML + " - " + msg.Errors;
                        throw new Exception(d);*/
                        continue;
                    }
                    for (int i = 0; i < m_MailMessageFilters.Length; i++)
                    {                           
                        if ((m_MailMessageFilters[i].From.Equals("") || msg.From.Address.Contains(m_MailMessageFilters[i].From)) &&
                            (m_MailMessageFilters[i].Subject.Equals("") || msg.Subject.Contains(m_MailMessageFilters[i].Subject))
                            )
                        {
                            SavedMails sm = new SavedMails();
                            sm.From = msg.From.Address;
                            m_SavedMails.Add(sm);
                            m_ImapCommand.FetchBodyStructure(msg);
                            int ind = 0;
                            foreach (ImapMessageBodyPart imapMessageBodyPart in msg.BodyParts)
                            {
                                m_ImapCommand.FetchBodyPart(msg, ind);
                                if (imapMessageBodyPart.Attachment && (m_MailMessageFilters[i].FileName.Equals("") || imapMessageBodyPart.FileName.Contains(m_MailMessageFilters[i].FileName)))
                                {
                                   createFile(imapMessageBodyPart, m_MailMessageFilters[i],sm);
                                }
                                ind++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_Connection = null;
                throw ex;
            }
            finally
            {
                if (m_Connection != null && m_Connection.ConnectionState == ConnectionState.Open)
                    m_Connection.Close();
            }
        }

        #endregion Public Methods
    }
}
