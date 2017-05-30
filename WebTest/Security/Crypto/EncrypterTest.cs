using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Phnyx.Web.Security.Crypto;

namespace WebTest.Security.Crypto
{
    [TestClass]
    public class EncrypterTest
    {
        [TestMethod]
        public void EncryptShouldNotResultInHttpSymbols()
        {
            var serialized = Encrypter.Encrypt("æøuounknkjh");
            var url_encoded = HttpUtility.UrlEncode(serialized);

            Assert.AreEqual(serialized, url_encoded);
        }
    }
}
