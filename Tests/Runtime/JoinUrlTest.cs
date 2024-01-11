using NUnit.Framework;
using UHTTP.Helpers;

namespace UHTTP.Tests
{
    public class JoinUrlTest
    {
        private const string BaseUrl = "https://sample.com";

        private const string AdditionalUrl = "hello";

        private static string ExpectedJoined => $"{BaseUrl}/{AdditionalUrl}";

        [Test]
        public void WithNoSlash()
        {
            string actualJoined = UrlUtility.Join(BaseUrl, AdditionalUrl);
            Assert.AreEqual(ExpectedJoined, actualJoined);
        }

        [Test]
        public void WithSlashLeft()
        {
            string actualJoined = UrlUtility.Join(BaseUrl + "/", AdditionalUrl);
            Assert.AreEqual(ExpectedJoined, actualJoined);
        }

        [Test]
        public void WithSlashRight()
        {
            string actualJoined = UrlUtility.Join(BaseUrl, "/" + AdditionalUrl);
            Assert.AreEqual(ExpectedJoined, actualJoined);
        }

        [Test]
        public void WithSlashBoth()
        {
            string actualJoined = UrlUtility.Join(BaseUrl + "/", "/" + AdditionalUrl);
            Assert.AreEqual(ExpectedJoined, actualJoined);
        }
    }
}
