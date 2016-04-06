using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;

namespace ImapMailReading
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate("kuldeep6.in@gmail.com", "password");
                var query = SearchQuery.NotSeen;

                // Get the first personal namespace and list the toplevel folders under it.
                var personal = client.GetFolder(client.PersonalNamespaces[0]);
                foreach (var folder in personal.GetSubfolders(false))
                {
                    folder.Open(FolderAccess.ReadWrite);
                    foreach (var uid in folder.Search(query))
                    {
                        var message = folder.GetMessage(uid);
                        Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
                        folder.AddFlags(uid, MessageFlags.Seen, false);
                    }
                    Console.WriteLine("[folder] {0}", folder.Name);
                }
                

                // The Inbox folder is always available on all IMAP servers...
                // let's search for all messages received after Jan 12, 2013 with "MailKit" in the subject...
               
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                foreach (var uid in inbox.Search(query))
                {
                    var message = inbox.GetMessage(uid);
                    Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
                    
                }

                client.Disconnect(true);
            }
        }
    }
}
