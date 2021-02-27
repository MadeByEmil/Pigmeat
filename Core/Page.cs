// Copyright (C) 2020 Emil Sayahi
/*
This file is part of WDHAN.

    WDHAN is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    WDHAN is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with WDHAN.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;

namespace WDHAN.Core
{
    /// <summary>
    /// The <c>Page</c> class.
    /// Contains all methods related to handling <c>Page</c> objects.
    /// </summary>
    public class Page
    {
        /// <value>Year from page's date</value>
        public string year { get; set; }
        /// <value>Year from page's date, without the century (00…99)</value>
        public string short_year { get; set; }
        /// <value>Month from the page's date</value>
        public string month { get; set; }
        /// <value>Alternate rendering of month, depending on <c>global</c> culture value</value>
        public string i_month { get; set; }
        /// <value>Abbreviated month name</value>
        public string short_month { get; set; }
        /// <value>Full month name</value>
        public string long_month { get; set; }
        /// <value>Day of the month (01…31)</value>
        public string day { get; set; }
        /// <value>Day of the month without leading zeroes (1…31)</value>
        public string i_day { get; set; }
        /// <value>Ordinal day of the year, with leading zeroes (001…366)</value>
        public string y_day { get; set; }
        /// <value>Business week year</value>
        public int w_year { get; set; }
        /// <value>Week number of the year (01…53)</value>
        public string week { get; set; }
        /// <value>Day of the week (1…7)</value>
        public int w_day { get; set; }
        /// <value>Abbreviated name of the day</value>
        public string short_day { get; set; }
        /// <value>Full name of the day</value>
        public string long_day { get; set; }
        /// <value>Zero-padded hour of the day (00…24)</value>
        public string hour { get; set; }
        /// <value>Minute of the hour from the page's date (00…59)</value>
        public string minute { get; set; }
        /// <value>Second of the minute from the page's date (00…59)</value>
        public string second { get; set; }
        /// <value>Page's date in the ISO 8601 format (YYYY-MM-DDTHH:MM:SS)</value>
        public DateTime date { get; set; }
        /// <value>Path to page relative to source directory</value>
        public string dir { get; set; }
        /// <value>Filename of page</value>
        public string name { get; set; }
        /// <value>Unrendered Liquid string which will become <c>page.url</c></value>
        public string permalink { get; set; }
        /// <value>Path to page relative to output directory/global URL</value>
        public string url { get; set; }
        /// <value>Page's content</value>
        public string content { get; set; }

        /// <summary>
        /// Parses a given page into a <c>JObject</c>
        /// </summary>
        /// <returns>
        /// The JSON representation of a page and its metadata
        /// </returns>
        /// <param name="PagePath">The path to the page being parsed</param>
        /// <para> See <see cref="IO.GetCollections"/> </para>
        public static JObject GetPageObject(string PagePath)
        {
            var SplitPage = Page.SplitFrontmatter(File.ReadAllLines(PagePath));
            string PageFrontmatter = SplitPage[0];

            JObject FrontmatterObject = IO.GetYamlObject(PageFrontmatter);
            Page page = JsonConvert.DeserializeObject<Page>(FrontmatterObject.ToString(Formatting.None));

            page.name = Path.GetFileNameWithoutExtension(PagePath);
            page.dir = Path.GetDirectoryName(PagePath);
            page.content = SplitPage[1];

            JObject GlobalObject = JObject.Parse(IO.GetGlobal());
            GlobalObject.Merge(JObject.Parse(IO.GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            CultureInfo Culture = new CultureInfo(GlobalObject["culture"].ToString()); // Get 'culture' from Global
            page.year = page.date.ToString("yyyy", Culture);
            page.short_year = page.date.ToString("y", Culture);
            page.month = page.date.ToString("MM", Culture);
            page.i_month = page.date.ToString("M", Culture);
            page.short_month = page.date.ToString("MMM", Culture);
            page.long_month = page.date.ToString("MMMM", Culture);
            page.day = page.date.ToString("dd", Culture);
            page.i_day = page.date.ToString("d", Culture);
            page.y_day = GetDayOfYear(page.date, Culture);
            page.week = Culture.Calendar.GetWeekOfYear(page.date, Culture.DateTimeFormat.CalendarWeekRule, Culture.DateTimeFormat.FirstDayOfWeek).ToString(Culture);
            page.w_day = (int) Culture.Calendar.GetDayOfWeek(page.date);
            page.w_year = ISOWeek.GetYear(page.date);
            page.short_day = Culture.DateTimeFormat.GetAbbreviatedDayName(Culture.Calendar.GetDayOfWeek(page.date));
            page.long_day = Culture.DateTimeFormat.GetDayName(Culture.Calendar.GetDayOfWeek(page.date));
            page.hour = page.date.ToString("HH", Culture);
            page.minute = page.date.ToString("mm", Culture);
            page.second = page.date.ToString("ss", Culture);

            JObject PageObject = JObject.Parse(JsonConvert.SerializeObject(page, Formatting.None));
            PageObject.Merge(JObject.Parse(FrontmatterObject.ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            
            // Get URL from collected page data, merge back into JObject
            page.url = GetPermalink(PageObject, new JObject {}); // Generate output path based on other variables
            PageObject.Merge(JObject.Parse(JsonConvert.SerializeObject(page, Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

            try
            {
                string DirectoryName = Path.GetDirectoryName(PagePath);
                bool isMarkdown = Path.GetExtension(PagePath).Equals(".md");
                if(!DirectoryName.Equals(".") && DirectoryName.Substring(0, 3).Equals("./_"))
                {
                    page.content = IO.RenderPage(PageObject, DirectoryName.Substring(3), true, isMarkdown, new JObject {});
                }
                else
                {
                    page.content = IO.RenderPage(PageObject, "", true, isMarkdown, new JObject {});
                }
            }
            catch(ArgumentOutOfRangeException) // If not in root, but not in a collection
            {
                bool isMarkdown = Path.GetExtension(PagePath).Equals(".md");
                page.content = IO.RenderPage(PageObject, "", true, isMarkdown, new JObject {});
            }

            // Merge content back into JObject
            PageObject.Merge(JObject.Parse(JsonConvert.SerializeObject(page, Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            return PageObject; // Return JObject of page
        }
        
        /// <summary>
        /// Gets the YAML of the frontmatter for a given page
        /// <para> See <see cref="IO.GetLayoutContents(string, bool)"/> </para>
        /// <seealso cref="Page.GetPageObject(string)"/>
        /// </summary>
        /// <returns>
        /// The YAML <c>string</c> for a given page
        /// </returns>
        /// <param name="PageLines">The file contents of the page being parsed</param>
        public static string[] SplitFrontmatter(string[] PageLines)
        {
            bool BeginFrontmatter = false;
            bool EndFrontmatter = false;
            StringBuilder Frontmatter = new StringBuilder();
            StringBuilder Contents = new StringBuilder();

            foreach(var line in PageLines)
            {
                var CurrentLine = line;
                if(!BeginFrontmatter && CurrentLine.Equals("---"))
                {
                    BeginFrontmatter = true;
                }
                else if(BeginFrontmatter && CurrentLine.Equals("---") && !EndFrontmatter)
                {
                    EndFrontmatter = true;
                }
                else if(BeginFrontmatter && !EndFrontmatter)
                {
                    Frontmatter.AppendLine(CurrentLine);
                }
                else
                {
                    Contents.AppendLine(CurrentLine);
                }
            }

            return new string[] { Frontmatter.ToString(), Contents.ToString() };
        }
        
        /// <summary>
        /// Parses the permalink using given metadata to generate an output path
        /// </summary>
        /// <returns>
        /// A <c>string</c> pointing to the page's output path
        /// </returns>
        /// <param name="PageObject">The <c>JObject</c> holding the page's metadata</param>
        /// <param name="PaginatorObject">The <c>JObject</c> representing the page's paginator values</param>
        /// <para> See <see cref="Page.GetPageObject(string)"/> </para>
        public static string GetPermalink(JObject PageObject, JObject PaginatorObject)
        {
            string Permalink = PageObject["permalink"].ToString();
            switch(Permalink)
            {
                case "date":
                    Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.month }}/{{ page.day }}/{{ page.title }}.html";
                    break;
                case "pretty":
                    Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.month }}/{{ page.day }}/{{ page.title }}.html";
                    break;
                case "ordinal":
                    Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.y_day }}/{{ page.title }}.html";
                    break;
                case "weekdate":
                    Permalink = "/{{ page.collection }}/{{ page.year }}/W{{ page.week }}/{{ page.short_day }}/{{ page.title }}.html";
                    break;
                case "none":
                    Permalink = "/{{ page.collection }}/{{ page.title }}.html";
                    break;
                default:
                    break;
            }
            var template = Template.ParseLiquid(Permalink);
            return template.Render(new { page = PageObject, global = IO.GetGlobal(), wdhan = IO.GetWDHAN(), paginator = PaginatorObject });
        }
        static string GetDayOfYear(DateTime Date, CultureInfo Culture)
        {
            if(Date.DayOfYear.ToString().Length == 1)
            {
                return "00" + Date.DayOfYear.ToString(Culture);
            }
            else if(Date.DayOfYear.ToString().Length == 2)
            {
                return "0" + Date.DayOfYear.ToString(Culture);
            }
            else
            {
                return Date.DayOfYear.ToString(Culture);
            }
        }
    }
}