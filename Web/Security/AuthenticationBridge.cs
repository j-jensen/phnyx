using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phnyx.Web.Security
{
    public abstract class AuthenticationBridge
    {
        /// <summary>
        /// Should authenticate provider/id, and return a valid user ticket.
        /// </summary>
        /// <param name="provider">Name of provider</param>
        /// <param name="id">Providerspecific id</param>
        /// <returns></returns>
        public abstract AuthenticationResult Authenticate(string provider, object id);

        /// <summary>
        /// Should create a useraccount and authenticate provider/id, and return a valid user ticket.
        /// </summary>
        /// <param name="provider">Name of provider</param>
        /// <param name="id">Providerspecific id</param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public abstract AuthenticationResult CreateAndAuthenticate(string provider, object id, string email, string name, string firstName, string lastName, Gender gender);
    }

    public class AuthenticationResult
    {
        public AuthenticationResult(ConnectResult result, AuthenticationTicket ticket=null)
        {
            this.Result = result;
            this.Ticket = ticket;
        }
        public readonly AuthenticationTicket Ticket;
        public readonly ConnectResult Result;
    }
}
