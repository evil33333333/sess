using Leaf.xNet;
using System;
using static Sess.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Sess
{
    class Program
    {
        public static List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
        public static List<String> accounts = new List<String>();
        public static List<String> proxies = Utility.ReturnProxies();
        static void Main(string[] args)
        {
            if (System.IO.File.Exists("accounts.txt"))
            {
                List<String> sessions = new List<string>();
                accounts = System.IO.File.ReadAllLines("accounts.txt").ToList();
                foreach (string account in accounts)
                {
                    threads.Add(
                        new System.Threading.Thread(() =>
                        {
                            string session = GetSessionId(account.Split(':')[0], account.Split(':')[1]);
                            if (!String.IsNullOrEmpty(session))
                            {
                                sessions.Add(session);
                            }
                        })
                    );
                }
                foreach (System.Threading.Thread thread in threads)
                {
                    thread.Start();
                    thread.Join();
                }
                while (true)
                {
                    if (accounts.Count == 0)
                    {
                        System.IO.File.WriteAllLines("sessions.txt", sessions);
                        Console.WriteLine("Parsed all sessions..");
                        System.Threading.Thread.Sleep(3000);
                        break;
                    }
                }
                Program.Main(args);
            }
            Console.Write("Username: "); string username = Console.ReadLine();
            Console.Write("Password: "); string password = Utility.ReadPassword('*');
            string sessionid = GetSessionId(username, password);
            if (!String.IsNullOrEmpty(sessionid))
            {
                Console.WriteLine($"Session ID: {sessionid}\n\n");
                System.Threading.Thread.Sleep(3000);
                Program.Main(args);
            }
            else
            {
                Console.WriteLine("err-> Please try again.");
                System.Threading.Thread.Sleep(3000);
                Program.Main(args);
            }
        }

        public static String GetSessionId(string username, string password, string proxy = null)
        {
            try
            {
                CookieStorage cookieStorage = new CookieStorage();
                HttpRequest httpRequest = new HttpRequest();
                if (proxy != null)
                {
                    httpRequest.Proxy = ProxyClient.Parse(ProxyType.HTTP, proxy);
                }
                httpRequest.AddHeader("User-Agent", "Instagram 85.0.0.21.100 Android (28/9; 380dpi; 1080x2147; OnePlus; HWEVA; OnePlus6T; qcom; en_US; 146536611)");
                httpRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                httpRequest.Cookies = cookieStorage;
                httpRequest.IgnoreProtocolErrors = true;
                StringContent stringContent = new StringContent($"username={username}&password={password}&device_id=android-{new Random().Next()}");
                stringContent.ContentType = "application/x-www-form-urlencoded";
                httpRequest.ConnectTimeout = 3000;
                HttpResponse response = httpRequest.Post("https://i.instagram.com/api/v1/accounts/login/", stringContent);
                System.Net.CookieCollection cookieCollection = cookieStorage.GetCookies(new Uri("https://i.instagram.com/api/v1/accounts/edit_profile/"));
                System.Collections.IEnumerator cookieEnum = cookieCollection.GetEnumerator();
                while (cookieEnum.MoveNext())
                {

                    if (cookieEnum.Current.ToString().Contains("sessionid="))
                    {
                        if (accounts.Count != 0) accounts.RemoveOccurenceOfString(username);
                        return cookieEnum.Current.ToString().Split('=')[1];
                    }
                }
                if (accounts.Count != 0) accounts.RemoveOccurenceOfString(username);
                return null;
            }
            catch
            {
                return GetSessionId(username, password, proxy);
            }
        }
    }
}
