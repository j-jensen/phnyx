using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phnyx.Web
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class MarkupAttribute : Attribute
    {
        private string _id;

        public MarkupAttribute(string id)
        {
            this._id = id;
        }

        public string Id { get { return _id; } }
        
    }
}
