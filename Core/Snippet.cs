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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;

namespace WDHAN.Core
{
    /// <summary>
    /// The <c>Snippet</c> class.
    /// Contains all methods related to handling <c>{! snippet !}</c> calls.
    /// </summary>
    class Snippet
    {
        public static Dictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Parses through each <c>{! snippet !}</c> call in a page and evaluates them
        /// <para>See <see cref="Snippet.Render(string, JObject)"/></para>
        /// <seealso cref="IO.RenderPage(JObject, string, bool, bool, JObject)"/>
        /// </summary>
        /// <returns>
        /// Contents of a page with <c>Snippet</c>s evaluated
        /// </returns>
        /// <param name="Contents">A <c>string</c> containing the contents of the page being parsed</param>
        /// <param name="PageObject">A <c>JObject</c> representing the page being parsed</param>
        public static string Parse(string Contents, JObject PageObject)
        {
            if(Contents.Contains("{! snippet "))
            {
                List<string> SnippetCalls = new List<string>();
                StringBuilder Parser = new StringBuilder();
                int BraceCount = 0;
                foreach(var character in Contents)
                {
                    if(character.Equals('{') && BraceCount == 0)
                    {
                        BraceCount++;
                        Parser.Append(character);
                        continue;
                    }
                    if(character.Equals('}') && BraceCount == 1)
                    {
                        BraceCount = 0;
                        Parser.Append(character);
                        var ParserString = Parser.ToString();
                        if(ParserString.Contains("{! snippet ") || ParserString.Contains("{!snippet "))
                        {
                            SnippetCalls.Add(WebUtility.HtmlDecode(ParserString));
                        }
                        Parser = new StringBuilder();
                        continue;
                    }
                    if(BraceCount == 1)
                    {
                        Parser.Append(character);
                        continue;
                    }
                }
                foreach(var snippetCall in SnippetCalls)
                {
                    string[] CallArguments = GetArguments(snippetCall);
                    Variables = new Dictionary<string, string>(); // Reset variable data for each new call
                    List<string> Keys = GetKeys(CallArguments);
                    List<string> Values = GetValues(CallArguments, Keys.ToArray());

                    for(int i = 0; i < Keys.ToArray().Length; i++)
                    {
                        Variables.Add(Keys[i], Values[i]); // Merge `List`s into `Dictionary`
                    }

                    string SnippetPath = "./snippets/" + CallArguments[2]; // Trimming required for this to work
                    Contents = Contents.Replace(WebUtility.HtmlEncode(snippetCall), Snippet.Render(SnippetPath, PageObject));
                }
            }
            return Contents;
        }

        /// <summary>
        /// Renders <c>Snippet</c>s
        /// </summary>
        /// <returns>
        /// The evaluated <c>Snippet</c>
        /// </returns>
        /// <param name="SnippetPath">The path to the <c>Snippet</c> being rendered</param>
        /// <param name="PageObject">A <c>JObject</c> representing the page being parsed</param>
        static string Render(string SnippetPath, JObject PageObject)
        {
            // Get outside data
            JObject Global = JObject.Parse(IO.GetGlobal());
            Global.Merge(JObject.Parse(IO.GetCollections().ToString(Formatting.None)), new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            JObject CollectionObject = new JObject {};
            if(PageObject.ContainsKey("collection"))
            {
                // Can't use `Collection` parameter, as sometimes we don't want the output filed as an entry for said collection
                CollectionObject = JObject.Parse(File.ReadAllText("./_" + PageObject["collection"].ToString() + "/collection.json"));

            }
            JObject SnippetObject = JObject.Parse("{\n\t\"exists\": true\n}");
            
            if(Variables != null)
            {
                SnippetObject = JObject.Parse(JsonConvert.SerializeObject(Variables, Formatting.None));
            }
            var template = Template.ParseLiquid(File.ReadAllText(SnippetPath));
            var SnippetContents = template.Render(new { snippet = IO.ConvertFromJson(SnippetObject), page = IO.ConvertFromJson(PageObject), collection = IO.ConvertFromJson(CollectionObject), global = IO.ConvertFromJson(Global), wdhan = IO.ConvertFromJson(IO.GetWDHAN()) });
            SnippetContents = Snippet.Parse(SnippetContents, PageObject); // Parse for snippets
            return SnippetContents;
        }

        /// <summary>
        /// Gets the arguments given in the <c>Snippet</c> call, to be parsed through later
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing <c>Snippet</c> arguments
        /// </returns>
        static string[] GetArguments(string SnippetCall)
        {
            List<string> Arguments = new List<string>();
            StringBuilder CurrentArgument = new StringBuilder();
            foreach(var character in SnippetCall)
            {
                if(character.Equals(' '))
                {
                    Arguments.Add(CurrentArgument.ToString());
                    CurrentArgument.Clear();
                    continue;
                }
                else
                {
                    CurrentArgument.Append(character);
                    continue;
                }
            }
            return Arguments.ToArray();
        }

        /// <summary>
        /// Gets the keys (names of variables) given when the Snippet was called
        /// <para> See <see cref="Snippet.GetValues(string[], string[])"/> </para>
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing names of given arguments
        /// </returns>
        static List<string> GetKeys(string[] CallArguments)
        {
            List<string> Keys = new List<string>();
            StringBuilder CurrentKey = new StringBuilder();
            for(int i = 3; i < CallArguments.Length; i++)
            {
                foreach(var character in CallArguments[i])
                {
                    if(character.Equals('='))
                    {
                        Keys.Add(CurrentKey.ToString());
                        CurrentKey.Clear();
                        break;
                    }
                    else
                    {
                        CurrentKey.Append(character);
                        continue;
                    }
                }
            }
            return Keys;
        }

        /// <summary>
        /// Gets values of given arguments/variables when the <c>Snippet</c> was called
        /// <para> See <see cref="Snippet.GetKeys(string[])"/> </para>
        /// </summary>
        /// <returns>
        /// Array of <c>string</c>s containing values of given arguments
        /// </returns>
        static List<string> GetValues(string[] CallArguments, string[] Keys)
        {
            List<string> Values = new List<string>();
            StringBuilder CurrentValue = new StringBuilder();
            List<int> Bits = new List<int>();
            for(int i = 3; i < Keys.Length + 3; i++)
            {
                if(Bits.Contains(i))
                {
                    continue; // Skip if this bit of the arguments has been processed as a part of a quoted value
                }

                CurrentValue.Append(CallArguments[i]);
                CurrentValue.Replace(Keys[i - 3] + "=", ""); // Get value by removing the key

                // If value is in quotes, get all the pieces of the argument it's in
                if(CurrentValue[0].Equals('"'))
                {
                    for(int j = i + 1; j < CallArguments.Length; j++)
                    {
                        // If this argument did have a key, stop appending
                        if(CallArguments[j].Contains('='))
                        {
                            Bits.Add(j);
                            break;
                        }
                        else
                        {
                            CurrentValue.Append(" " + CallArguments[j]);
                            continue;
                        }
                    }
                }
                if(CurrentValue[0].Equals('"') && CurrentValue[CurrentValue.Length - 1].Equals('"'))
                {
                    CurrentValue.Remove(0, 1); // Remove the first quote mark
                    CurrentValue.Length--; // Move the pointer back, removing the last quote mark
                }

                Values.Add(CurrentValue.ToString());
                CurrentValue.Clear();
            }
            return Values;
        }
    }
}