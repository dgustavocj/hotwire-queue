using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestFramework
{
    //TODO: might be able to use specflow table parsers and "model binders"?

    public class ScenarioRow
    {
        public int SequenceNo { get; set; }
        public string Title { get; set; }
    }


    public class RestScenario : ScenarioRow
    {
        public Uri Uri { get; set; }
        public int Response { get; set; }
        public string ResponseTextToContain { get; set; }
    }



    public static class StoryParser
    {
        public static List<T> Parse<T>(string story) where T : ScenarioRow
        {
            var rawRows = GetRawRows(story);
            var headings = GetColumns(rawRows.ElementAt(0));
            var scenarioRows = rawRows.Skip(2);
            EnsureLastColumnOfHeadingIsHash(headings);
            CheckRowsEndWithRowNumberReportAnyErrors(scenarioRows);

            var scenarios = new List<T>();

            foreach (var row in scenarioRows)
            {
                T scenario = Activator.CreateInstance<T>();
                var type = scenario.GetType();
                PropertyInfo[] props = type.GetProperties();

                int col = 0;
                string[] columns = GetColumns(row).ToArray();
                foreach (var heading in headings)
                {
                    string columnText = columns[col++];

                    if (heading == "#")
                    {
                        // last column, set the sequence ( don't need to check column again, as it's already been checked with CheckRowsEndWithRowNumberReportAnyErrors()
                        scenario.SequenceNo = int.Parse(columnText);
                        continue;
                    }

                    var prop= props.FirstOrDefault(p => p.Name == heading);
                    if (prop==null) throw new ArgumentOutOfRangeException("heading", heading,"Could not find property [" + heading + "] in scenarioRow dto object '" + type.Name + "'");

                    
                    if(prop.PropertyType==typeof(string))
                    {
                        prop.SetValue(scenario, columnText, null);
                        continue;
                    }

                    if (prop.PropertyType == typeof(Uri))
                    {
                        try
                        {
                            prop.SetValue(scenario, new Uri(columnText, UriKind.Relative), null);
                            continue;
                        }
                        catch (Exception)
                        {
                            throw new ArgumentOutOfRangeException(heading, columnText, "Error parsing story column '" + heading + "' " + columnText + " is not a valid Uri.");
                        }

                    }


                    if (prop.PropertyType == typeof(int))
                    {
                        int number;
                        if (!int.TryParse(columnText, out number)) throw new ArgumentOutOfRangeException(heading, columnText, "Error parsing story column '" + heading + "' " + columnText + " is not an int.");
                        prop.SetValue(scenario, int.Parse(columnText),null );
                        continue;
                    }

                    if (prop.PropertyType == typeof(bool))
                    {
                        bool? result = columnText.ParseBool();
                        if (result ==null) throw new ArgumentOutOfRangeException(heading, columnText, "Error parsing story column '" + heading + "' " + columnText + " is not a bool.");
                        prop.SetValue(scenario,result.Value , null);
                        continue;
                    }

                    // extend here, more types...

                }
                scenarios.Add(scenario);
            }
            // call to list so that all the error checking happens first before users test code iterations
            var scenarioList = scenarios.OrderBy(s => s.SequenceNo).ToList();
            return scenarioList;
        }


        private static void EnsureLastColumnOfHeadingIsHash(IEnumerable<string> headings)
        {
            if (!headings.Last().Equals("#"))
                throw new ArgumentException(
                    "Parsing story text error: Story heading column does not end with '#', headings were:\n",
                    headings.ToVisualizerString());
        }

        private static IEnumerable<string> GetColumns(string titlerow)
        {
            IEnumerable<string> headings =
                titlerow.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(h => h.Trim());
            return headings;
        }

        private static IEnumerable<string> GetRawRows(string story)
        {
            IEnumerable<string> rawRows = story.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim(new char[] {'\n', '\r'})).Where(r => !String.IsNullOrEmpty(r));
            return rawRows;
        }

        private static void CheckRowsEndWithRowNumberReportAnyErrors(IEnumerable<string> scenarioRows)
        {
            int i = 1;
            foreach (var scenarioRow in scenarioRows)
            {
                if (!scenarioRow.EndsWith(i.ToString())) throw new ArgumentException("Parsing story text error: Scenario row should end with '" + i + "'");
                i++;
            }
        }
    }
}