using System;
using System.Collections.Generic;
using System.Linq;

namespace Sess
{
    static class Utility
    {
        public static string ReadPassword(char mask)
        {
            const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10 };

            var pass = new Stack<char>();
            char chr = (char)0;
            while ((chr = Console.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0) { }
                else
                {
                    pass.Push((char)chr);
                    Console.Write(mask);
                }
            }

            Console.WriteLine();
            return new string(pass.Reverse().ToArray());
        }

        public static List<String> ReturnProxies()
        {
            return new System.Net.WebClient().DownloadString("https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=1000&country=all&ssl=all&anonymity=all").Split('\n').ToList();
        }

        public static void RemoveOccurenceOfString(this List<String> list, string matcher)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i).Contains(matcher)) list.RemoveAt(i);
            }
        }
    }
}
