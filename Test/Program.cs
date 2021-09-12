using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog systemLog = new EventLog();
            systemLog.Log = "System";
            List<EventLogEntry> logs = new List<EventLogEntry>(systemLog.Entries.Count);
            SortedDictionary<DateTime, string> ee = new SortedDictionary<DateTime, string>();

            foreach (EventLogEntry entry in systemLog.Entries)
            {
                logs.Add(entry);
            }

            var exits = logs.Where(log => log.Source == "Microsoft-Windows-Kernel-General" && log.InstanceId == 13);
            var enters = logs.Where(log => log.Source == "Microsoft-Windows-Kernel-General" && log.InstanceId == 12);

            foreach (var log in exits)
            {
                Regex regex = new Regex(@"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})\.(\d*)Z", RegexOptions.Compiled);
                if (regex.IsMatch(log.Message))
                {
                    var matches = regex.Matches(log.Message);
                    var groups = matches[0].Groups;
                    var year = int.Parse(groups[1].Value);
                    var month = int.Parse(groups[2].Value);
                    var day = int.Parse(groups[3].Value);
                    var hour = int.Parse(groups[4].Value);
                    var minute = int.Parse(groups[5].Value);
                    var second = int.Parse(groups[6].Value);
                    var date = new DateTime(year, month, day, hour, minute, second);
                    Console.WriteLine($@"Exit: {date}");
                    ee.Add(date, "Exit");
                }
            }

            foreach (var log in enters)
            {
                Regex regex = new Regex(@"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})\.(\d*)Z", RegexOptions.Compiled);
                if (regex.IsMatch(log.Message))
                {
                    var matches = regex.Matches(log.Message);
                    var groups = matches[0].Groups;
                    var year = int.Parse(groups[1].Value);
                    var month = int.Parse(groups[2].Value);
                    var day = int.Parse(groups[3].Value);
                    var hour = int.Parse(groups[4].Value);
                    var minute = int.Parse(groups[5].Value);
                    var second = int.Parse(groups[6].Value);
                    var date = new DateTime(year, month, day, hour, minute, second);
                    Console.WriteLine($@"Enter: {date}");
                    ee.Add(date, "Enter");
                }
            }

            Console.WriteLine();
            
            var mas = ee.ToArray();
            for (int i = 0; i < mas.Length; i++)
            {
                Console.Write($@"{mas[i].Key}: {mas[i].Value}");
                if (i != 0 && mas[i].Value == "Exit")
                {
                    var d = mas[i].Key - mas[i - 1].Key;
                    Console.Write($@" Computer had been working for {d.Days} days {d.Hours} hours {d.Minutes} minutes {d.Seconds} seconds");
                }

                if (i == mas.Length - 1 && mas[i].Value == "Enter")
                {
                    var d = DateTime.UtcNow - mas[i].Key;
                    Console.Write($@" Computer has been working for {d.Days} days {d.Hours} hours {d.Minutes} minutes {d.Seconds} seconds");
                }

                Console.WriteLine();
            }
        }
    }
}