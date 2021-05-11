#region References
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Fintek.Common.IMAP
{
    #region Header
    /// <summary>
    /// Represents the comparer class used for sorting messages.
    /// </summary>
    #endregion
    public class ImapMailboxMessageComparer : IComparer<ImapMailboxMessage>
    {
        #region Enumerators
        /// <summary>
        /// Options for the direction of the sort.
        /// </summary>
        public enum SortMethod
        {
            /// <summary>
            /// Sort ascending.
            /// </summary>
            Ascending = 1, 
            /// <summary>
            /// Sort descending.
            /// </summary>
            Descending = -1
        }
        /// <summary>
        /// Options for the type of sort.
        /// </summary>
        public enum SortBy
        {
            /// <summary>
            /// Sort messages by the message ID.
            /// </summary>
            ID,
            /// <summary>
            /// Sort messages by the From address.
            /// </summary>
            From, 
            /// <summary>
            /// Sort messages by the Received date
            /// </summary>
            Date, 
            /// <summary>
            /// Sort messages by the Read flag.
            /// </summary>
            Read, 
            /// <summary>
            /// Sort messages by the Important (Flagged) flag.
            /// </summary>
            Important, 
            /// <summary>
            /// Sort messages by the Deleted flag.
            /// </summary>
            Deleted, 
            /// <summary>
            /// Sort messages by the Replied flag.
            /// </summary>
            Replied, 
            /// <summary>
            /// Sort messages by the message size.
            /// </summary>
            Size, 
            /// <summary>
            /// Sort messages by the message subject.
            /// </summary>
            Subject
        }
        #endregion

        #region Private Variables
        SortBy _sort;
        SortMethod _order;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an instance of the ImapMailboxMessageComparer class.
        /// </summary>
        /// <param name="Sort">The property to sort on.</param>
        /// <param name="Order">The direction of the sort.</param>
        public ImapMailboxMessageComparer(SortBy Sort, SortMethod Order)
        {
            _sort = Sort;
            _order = Order;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compares two ImapMailboxMessages.
        /// </summary>
        /// <param name="m1">A ImapMailboxMessage</param>
        /// <param name="m2">A ImapMailboxMessage to compare the previous value to.</param>
        /// <returns>Returns an integer value indicating how messages should be sorted.</returns>
        public int Compare(ImapMailboxMessage m1, ImapMailboxMessage m2)
        {
            int retval = 1;
            if (m1 == null || m2 == null)
                return 0;
            else
                switch (_sort)
                {
                    case SortBy.ID:
                        retval = m1.ID.CompareTo(m2.ID);
                        break;
                    case SortBy.From:
                        retval = m1.From.Address.CompareTo(m2.From.Address);
                        break;
                    case SortBy.Date:
                        retval = m1.Received.CompareTo(m2.Received);
                        break;
                    case SortBy.Read:
                        retval = m1.Flags.Seen.CompareTo(m2.Flags.Seen);
                        break;
                    case SortBy.Important:
                        retval = m1.Flags.Flagged.CompareTo(m2.Flags.Flagged);
                        break;
                    case SortBy.Deleted:
                        retval = m1.Flags.Deleted.CompareTo(m2.Flags.Deleted);
                        break;
                    case SortBy.Replied:
                        retval = m1.Flags.Answered.CompareTo(m2.Flags.Answered);
                        break;
                    case SortBy.Size:
                        retval = m1.Size.CompareTo(m2.Size);
                        break;
                    case SortBy.Subject:
                        retval = m1.Subject.CompareTo(m2.Subject);
                        break;
                }
            if (_order == SortMethod.Descending)
                retval *= -1;
            return retval;
        }
        #endregion
    }
}
