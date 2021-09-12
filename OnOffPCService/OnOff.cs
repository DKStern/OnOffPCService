using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OnOffPCService
{
    public static class OnOff
    {
        private static readonly EventLog _systemLog = new EventLog("System");

        private static List<EventLogEntry> _logs;
        
        private static readonly SortedDictionary<DateTime, Code> _entersExits = new SortedDictionary<DateTime, Code>();
        
        private static void Init()
        {
            if (File.Exists(@"C:\Логи\Логи включения и выключения ПК.log"))
            {
                using (var sr = new StreamReader(@"C:\Логи\Логи включения и выключения ПК.log", Encoding.Unicode))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        var lines = line?.Split(new[]
                        {
                            ": "
                        }, StringSplitOptions.RemoveEmptyEntries);

                        if (lines == null || lines.Length == 0)
                            continue;

                        var date = Convert.ToDateTime(lines[0]);
                        var code = lines[1];

                        switch (code)
                        {
                            case "Enter":
                                _entersExits.Add(date, Code.Enter);
                                break;
                            case "Exit":
                                _entersExits.Add(date, Code.Exit);
                                break;
                        }
                    }
                }
            }

            _logs = new List<EventLogEntry>(_systemLog.Entries.Count);

            foreach (EventLogEntry entry in _systemLog.Entries)
            {
                _logs.Add(entry);
            }
        }

        private static void GetEnterExit(Code code)
        {
            var list = _logs.Where(log => log.Source == "Microsoft-Windows-Kernel-General" && log.InstanceId == (int)code);

            foreach (var log in list)
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

                    _entersExits.Add(date, code);
                }
            }
        }

        private static void Write()
        {
            using (var sw = new StreamWriter(@"C:\Логи\Логи включения и выключения ПК.log", false, Encoding.Unicode))
            {
                foreach (var date in _entersExits)
                {
                    sw.WriteLine($@"{date.Key}: {date.Value}");
                }

                sw.Flush();
            }
        }

        public static void GetData()
        {
            Init();

            GetEnterExit(Code.Enter);
            GetEnterExit(Code.Exit);

            Write();
        }
    }
}