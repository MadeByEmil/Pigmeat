using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WDHAN.Core
{
    /// <summary>
    /// The <c>Paginator</c> class.
    /// Contains all variables, and render method, for pagination
    /// </summary>
    public class Paginator
    {
        /// <value>Number of the current page</value>
        public int page { get; set; }
        /// <value>Number of posts per page</value>
        public int per_page { get; set; }
        /// <value>The posts available for the current page</value>
        public JObject[] posts { get; set; }
        /// <value>The total number of posts</value>
        public int total_posts { get; set; }
        /// <value>The total number of pages</value>
        public int total_pages { get; set; }
        /// <value>Number of the previous page, or <c>null</c> if no previous page exists.</value>
        public object previous_page { get; set; }
        /// <value>Number of the next page, or <c>null</c> if no next page exists.</value>
        public object next_page { get; set; }

        /// <summary>
        /// Outputs a paginated version of a page (requires <c>{{ page.paginate }}</c> and <c>{{ page.collection }}</c>)
        /// </summary>
        /// <param name="PagePath">The path to the page being parsed</param>
        public static void RenderPaginated(string PagePath)
        {
            bool isMarkdown = Path.GetExtension(PagePath).Equals(".md");
            JObject PageObject = Page.GetPageObject(PagePath);
            Paginator paginator = new Paginator();

            // Get collection's entries (collection specified in frontmatter) → total_posts
            JObject CollectionObject = JObject.Parse(File.ReadAllText("./_" + PageObject["collection"].ToString() + "/collection.json"));
            var Entries = JsonConvert.DeserializeObject<List<JObject>>(CollectionObject["entries"].ToString()).ToArray();
            paginator.total_posts = Entries.Length;

            // (total_posts ÷ paginate) + (total_posts % paginate) → total_pages
            paginator.per_page = int.Parse(PageObject["paginate"].ToString());
            paginator.total_pages = ((int) paginator.total_posts / paginator.per_page) + (paginator.total_posts % 2);
            if(paginator.total_pages == 0)
            {
                paginator.total_pages = 1;
            }

            // Store pages w/ calculated paginator values in List of JSON
            List<JObject> PaginatorArray = new List<JObject>();
            for(int i = 1; i <= paginator.total_pages; i++)
            {
                // Figure out paginator's page numbers
                object next_page, previous_page;
                if(i == paginator.total_pages)
                {
                    previous_page = paginator.total_pages - 1;
                    next_page = null;
                }
                else if(i == 1)
                {
                    previous_page = null;
                    next_page = 2;
                }
                else
                {
                    previous_page = i - 1;
                    next_page = i + 1;
                }
                paginator.page = i;
                paginator.next_page = next_page;
                paginator.previous_page = previous_page;

                // Assign posts to current page
                var CurrentPosts = new List<JObject>();
                var startIndex = ((i - 1) * paginator.per_page);
                for(int j = startIndex; j < (startIndex + paginator.per_page); j++)
                {
                    if(j >= paginator.total_posts) // Sometimes the total number of posts doesn't divide evenly with the amount per page
                    {
                        break;
                    }
                    else
                    {
                        CurrentPosts.Add(Entries[j]);
                    }
                }
                paginator.posts = CurrentPosts.ToArray();

                // If the amount per page is too large, empty extra pages will be generated
                if(paginator.posts.Length != 0)
                {
                    PaginatorArray.Add(JObject.Parse(JsonConvert.SerializeObject(paginator))); // Save the paginator values for current page

                }
            }
            foreach(var page in PaginatorArray)
            {
                var CurrentPageObject = PageObject;
                CurrentPageObject["content"] = Page.SplitFrontmatter(File.ReadAllLines(PagePath))[1]; // Reset page's contents to re-render
                CurrentPageObject["url"] = Page.GetPermalink(CurrentPageObject, page); // Update output path
                CurrentPageObject["content"] = IO.RenderPage(CurrentPageObject, "", true, isMarkdown, page);

                // Cannot do this outside of library, must write multiple pages in same scope
                Directory.CreateDirectory(Path.GetDirectoryName("./output/" + CurrentPageObject["url"].ToString()));
                File.WriteAllText("./output/" + CurrentPageObject["url"].ToString(), CurrentPageObject["content"].ToString());
            }
        }
    }
}