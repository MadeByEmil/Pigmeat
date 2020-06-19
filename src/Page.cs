// Copyright (C) 2020 Emil Sayahi
/*
This file is part of Pigmeat.

    Pigmeat is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pigmeat is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pigmeat.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;

namespace Pigmeat.Core
{
    /// <summary>
    /// The <c>Page</c> class.
    /// Contains all methods related to handling <c>Page</c> objects.
    /// </summary>
    class Page
    {
        public string year { get; set; }
        public string short_year { get; set; }
        public string month { get; set; }
        public string i_month { get; set; }
        public string short_month { get; set; }
        public string long_month { get; set; }
        public string day { get; set; }
        public string i_day { get; set; }
        public string y_day { get; set; }
        public int w_year { get; set; }
        public string week { get; set; }
        public int w_day { get; set; }
        public string short_day { get; set; }
        public string long_day { get; set; }
        public string hour { get; set; }
        public string minute { get; set; }
        public string second { get; set; }
        public DateTime date { get; set; }
        public string dir { get; set; }
        public string name { get; set; }
        public string permalink { get; set; }
        public string url { get; set; }
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
            string PageFrontmatter = GetFrontmatter(PagePath);
            JObject FrontmatterObject = IO.GetYamlObject(PageFrontmatter);
            Page page = JsonConvert.DeserializeObject<Page>(FrontmatterObject.ToString(Formatting.None));

            page.name = Path.GetFileNameWithoutExtension(PagePath);
            page.dir = Path.GetDirectoryName(PagePath);
            page.content = string.Join(Environment.NewLine, File.ReadAllText(PagePath).Replace(PageFrontmatter, "").Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray());

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
            page.url = GetPermalink(PageObject); // Generate output path based on other variables
            PageObject.Merge(JObject.Parse(JsonConvert.SerializeObject(page, Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

            try
            {
                string DirectoryName = Path.GetDirectoryName(PagePath);
                if(!DirectoryName.Equals(".") && DirectoryName.Substring(0, 3).Equals("./_"))
                {
                    if(Path.GetExtension(PagePath).Equals(".md"))
                    {
                        page.content = IO.RenderPage(PageObject, DirectoryName.Substring(3), true, true);
                    }
                    else
                    {
                        page.content = IO.RenderPage(PageObject, DirectoryName.Substring(3), true, false);
                    }
                }
                else
                {
                    if(Path.GetExtension(PagePath).Equals(".md"))
                    {
                        page.content = IO.RenderPage(PageObject, "", true, true);
                    }
                    else
                    {
                        page.content = IO.RenderPage(PageObject, "", true, false);
                    }
                }
            }
            catch(ArgumentOutOfRangeException) // If not in root, but not in a collection
            {
                if(Path.GetExtension(PagePath).Equals(".md"))
                {
                    page.content = IO.RenderPage(PageObject, "", true, true);
                }
                else
                {
                    page.content = IO.RenderPage(PageObject, "", true, false);
                }
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
        /// <param name="PagePath">The path of the page being parsed</param>
        public static string GetFrontmatter(string PagePath)
        {
            string FrontMatter = "";
            foreach(var line in File.ReadAllLines(PagePath))
            {
                if(!line.Equals("---"))
                {
                    FrontMatter += line + "\n";
                }
                else
                {
                    break;
                }
            }
            return FrontMatter;
        }
        
        /// <summary>
        /// Parses the permalink using given metadata to generate an output path
        /// </summary>
        /// <returns>
        /// A <c>string</c> pointing to the page's output path
        /// </returns>
        /// <param name="PageObject">The <c>JObject</c> holding the page's metadata</param>
        /// <para> See <see cref="Page.GetPageObject(string)"/> </para>
        static string GetPermalink(JObject PageObject)
        {
            string Permalink = PageObject["permalink"].ToString();
            if(Permalink.Equals("date", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.month }}/{{ page.day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("pretty", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.month }}/{{ page.day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("ordinal", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/{{ page.y_day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("weekdate", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.year }}/W{{ page.week }}/{{ page.short_day }}/{{ page.title }}.html";
            }
            else if(Permalink.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                Permalink = "/{{ page.collection }}/{{ page.title }}.html";
            }
            var template = Template.ParseLiquid(Permalink);
            return template.Render(new { page = PageObject, global = IO.GetGlobal()});
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