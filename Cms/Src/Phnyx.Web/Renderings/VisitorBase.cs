using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings
{
    public abstract class VisitorBase
    {
        private Filter _filter = Filter.AlwaysTrue();
        private IList<int> _inventedHere = new List<int>();

        public void Visit(RenderingBase rendering)
        {
            if (!InventedHere(rendering) && _filter.Pass(rendering))
                OnVisit(rendering);
        }

        public abstract void OnVisit(RenderingBase rendering);

        public Filter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        // TODO: This is not good
        // Visitor should not visit renderings created by that visitor - it would result in stack-overflow.
        // Visitors altering the dom is recommended to register all new renderings here 
        protected void Register(RenderingBase rendering)
        {
            _inventedHere.Add(rendering.GetHashCode());
        }

        private bool InventedHere(RenderingBase rendering)
        {
            return _inventedHere.Contains(rendering.GetHashCode());
        }
    }
}
