namespace UHTTP.Helpers
{
    public static class UrlUtility
    {
        /// <summary>
        /// Joins the urls by handling the existence or absence of the slash in the urls.
        /// </summary>
        public static string Join(string url, string additionalUrl)
        {
            string slash = url[url.Length - 1] == '/' ? string.Empty :
                additionalUrl[0] == '/' ? string.Empty : "/";

            return $"{url}{slash}{additionalUrl}";
        }
    }
}
