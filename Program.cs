using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml;

namespace CCG_TechnicalTest
{
    class Program
    {
        
        public static void Main()
        {
            OI oi = new OI();
            oi.GetDetails();
        }

        private class GroupedHeader
        {
            private string Header { get; set; }
            private List<int> Columns { get; set; }

            public GroupedHeader(string header, List<int> columns)
            {
                Header = header;
                Columns = columns;
            }

            public string GetHeader()
            {
                return Header;
            }

            public List<int> GetColumns()
            {
                return Columns;
            }
        }

        public class OI
        {
            public void GetDetails()
            {
                Console.WriteLine("Would you like to: -" +
                    "\n1. Convert from .csv to XML" +
                    "\n2. Convert from .csv to JSON" +
                    "\n3. Convert from XML to .csv" +
                    "\n4. Convert from JSON to .csv" +
                    "\n5. Convert from XML to JSON" +
                    "\n6. Convert from JSON to XML");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter filepath to be converted to XML");
                        string path = Console.ReadLine();
                        CsvToXml(path);
                        break;
                    case "2":
                        Console.WriteLine("Enter filepath to be converted to JSON");
                        string path2 = Console.ReadLine();
                        CsvToJson(path2);
                        break;
                    default:
                        CommingSoon();
                        break;
                }
            }
            private void CsvToXml(string path)
            {
                List<string> raw = GetDataFromCsv(path);
                string[] headings = raw[0].Split(',');
                List<GroupedHeader> grouped = HeaderGroups(headings);
                string outPath = path.Split('.')[0] + ".xml";
                XmlWriterSettings xws = new XmlWriterSettings { Indent = true };
                using (XmlWriter xw = XmlWriter.Create(outPath, xws))
                {
                    xw.WriteStartElement(outPath.Split('\\').Last());
                    for (int i = 1; i < raw.Count; i++)
                    {
                        foreach (GroupedHeader gh in grouped)
                        {
                            if (gh.GetColumns().Count == 1)
                            {
                                xw.WriteElementString(gh.GetHeader(), raw[i].Split(',')[gh.GetColumns()[0]]);
                            }
                            else
                            {
                                xw.WriteStartElement(gh.GetHeader());
                                foreach (int loc in gh.GetColumns())
                                {
                                    xw.WriteElementString(headings[loc], raw[i].Split(',')[loc]);
                                }
                                xw.WriteEndElement();
                            }
                        }
                    }
                    xw.Flush();
                }
                Console.WriteLine("File " + outPath + " created.");
                ContinueOrQuit();
            }

            private void CsvToJson(string path)
            {
                List<string> raw = GetDataFromCsv(path);
                string[] headings = raw[0].Split(',');
                List<GroupedHeader> grouped = HeaderGroups(headings);
                string outPath = path.Split('.')[0] + ".json";
                string jText = "[";
                for(int i = 1; i < raw.Count; i++)
                {
                    jText += "{";
                    foreach(GroupedHeader gh in grouped)
                    {
                        if(gh.GetColumns().Count == 1)
                        {
                            jText += "\"" + gh.GetHeader() + "\":\"" + raw[i].Split(',')[gh.GetColumns()[0]] + "\",";
                        }
                        else
                        {
                            jText += "\"" + gh.GetHeader() + "\":{";
                            foreach(int loc in gh.GetColumns())
                            {
                                jText += "\"" + headings[loc] + "\":\"" + raw[i].Split(',')[loc] + "\",";
                            }
                            jText = jText.TrimEnd(',');
                            jText += "},";
                        }
                    }
                    jText = jText.TrimEnd(',');
                    jText += "},";
                }
                jText = jText.TrimEnd(',');
                jText += "]";
                File.WriteAllText(outPath, jText);
                Console.WriteLine("File " + outPath + " created");
                ContinueOrQuit();
            }

            private List<string> GetDataFromCsv(string path)
            {
                List<string> raw = new List<string>();
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        raw.Add(line);
                    }
                }
                return raw;
            }

            private List<GroupedHeader> HeaderGroups(string[] headings)
            {
                List<GroupedHeader> grouped = new List<GroupedHeader>();
                foreach (string x in headings)
                {
                    if (x.Contains("_") == false)
                    {
                        List<int> current = new List<int>
                        {
                            Array.IndexOf(headings, x)
                        };
                        GroupedHeader nextHeader = new GroupedHeader(x, current);
                        grouped.Add(nextHeader);
                    }
                    else
                    {
                        string left = x.Split('_')[0];
                        try
                        {
                            GroupedHeader found = grouped.Where(p => p.GetHeader() == left).First();
                            found.GetColumns().Add(Array.IndexOf(headings, x));
                        }
                        catch
                        {
                            List<int> current = new List<int>
                            {
                                Array.IndexOf(headings, x)
                            };
                            GroupedHeader nextHeader = new GroupedHeader(left, current);
                            grouped.Add(nextHeader);
                        }
                    }
                }
                return grouped;
            }

            private void ContinueOrQuit()
            {
                Console.WriteLine("Convert another file? y/n");
                string answer = Console.ReadLine();
                switch (answer)
                {
                    case "y":
                        OI oi = new OI();
                        oi.GetDetails();
                        break;
                    case "n":
                        Environment.Exit(0);
                        break;
                    default:
                        ContinueOrQuit();
                        break;
                }
            }

            private void CommingSoon()
            {
                Console.WriteLine("Comming soon! Available options at this time are - 1 & 2.");
                ContinueOrQuit();
            }
        }

    }
}
