using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phnyx.Web.Security;

namespace WebTest.Security
{
    [TestClass]
    public class LoginTest
    {
        [TestMethod]
        public void StringifyParse_Basic()
        {
            var sut = new AuthenticationTicket { Email="jesper.jensen@dgi.dk", Id="1a2f1d3c", Provider="Email", Roles=new string[]{"Admin", "Editor"} };

            var str = sut.Stringify();
            Console.WriteLine(str);
            var rehydrated = AuthenticationTicket.Parse(str);

            Assert.AreEqual(sut.Email, rehydrated.Email);
            Assert.AreEqual(sut.Id, rehydrated.Id);
            Assert.AreEqual(sut.Provider, rehydrated.Provider);
            Assert.AreEqual(sut.Roles[0], rehydrated.Roles[0]);
            Assert.AreEqual(sut.Roles[1], rehydrated.Roles[1]);
        }
    }
}
