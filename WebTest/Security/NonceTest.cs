using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phnyx.Web.Security;
using FakeItEasy;
using System.Web;
using System.Text;

namespace WebTest.Security
{
    [TestClass]
    public class NonceTest
    {
        [TestInitialize]
        public void Init()
        {
            Nonce.GET_NOW = () => new DateTime(635028768000000000); //30-04-2013 00:00:00
            Nonce.GET_CLIENT_IP = () => "198.168.1.23";
        }

        [TestMethod]
        public void HasProperties()
        {
            string ip = "198.168.1.23", provider = "xProvider", url = "http://www.3wp.dk/";
            var now = DateTime.Now;

            var sut = new Nonce(ip, url, now, provider);

            Assert.AreEqual(ip, sut.IP);
            Assert.AreEqual(now, sut.Created);
            Assert.AreEqual(provider, sut.Provider);
        }

        [TestMethod]
        public void Validate_Exact()
        {
            string ip = "198.168.1.23", provider = "xProvider", url = "http://www.3wp.dk/";

            var encrypted = new Nonce(ip, url, Nonce.GET_NOW().AddSeconds(-120), provider).ToString();

            string provider_actual;
            Assert.IsTrue(Nonce.Validate(encrypted, out provider_actual));
            Assert.AreEqual(provider, provider_actual);
        }

        [TestMethod]
        public void Validate_New()
        {
            string ip = "198.168.1.23", provider = "xProvider", url = "http://www.3wp.dk/";

            var encrypted = new Nonce(ip, url, Nonce.GET_NOW().AddSeconds(-3), provider).ToString();

            string provider_actual;
            Assert.IsTrue(Nonce.Validate(encrypted, out provider_actual));
            Assert.AreEqual(provider, provider_actual);
        }

        [TestMethod]
        public void Validate_Old()
        {
            string ip = "198.168.1.23", provider = "xProvider", url = "http://www.3wp.dk/";

            var encrypted = new Nonce(ip, url, Nonce.GET_NOW().AddSeconds(-200), provider).ToString();

            string provider_actual;
            Assert.IsFalse(Nonce.Validate(encrypted, out provider_actual));
        }

        [TestMethod]
        public void Validate_WrongIP_False()
        {
            string ip = "198.168.1.111", provider = "xProvider", url = "http://www.3wp.dk/";

            var encrypted = new Nonce(ip, url, Nonce.GET_NOW().AddSeconds(-20), provider).ToString();

            string provider_actual;
            Assert.IsFalse(Nonce.Validate(encrypted, out provider_actual));
        }
    }
}
