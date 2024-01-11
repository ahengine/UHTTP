namespace UHTTP.Helpers
{
    public static class UrlUtility
    {
        /// <summary>
        /// Joins the urls by handling the existence or absence of the slash in the urls.
        /// </summary>
        public static string Join(string url, string additionalUrl)
        {
            if (url[url.Length - 1] == '/')
                url = url.Remove(url.Length - 1);

            if (additionalUrl[0] == '/')
                additionalUrl = additionalUrl.Substring(1);

            return $"{url}/{additionalUrl}";
        }
    }
}
