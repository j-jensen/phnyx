using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Phnyx.Web.Renderings
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Seen as a Rendering with child renderings - not a collecttion of renderings")]
    public abstract class RenderingBase : IEnumerable<RenderingBase>
    {
        // Fields
        private List<RenderingBase> _subRenderings = new List<RenderingBase>();
        private NameValueCollection _attributes = new NameValueCollection();

        // Methods

        public virtual void Accept(VisitorBase visitor)
        {
            visitor.Visit(this);
            // then proceed to all children
            _subRenderings.ForEach(x => x.Accept(visitor));
        }

        public void Add(RenderingBase subRendering)
        {
            subRendering.Parent = this;
            _subRenderings.Add(subRendering);
        }

        /// <summary>
        /// Removes all sub-renderings
        /// </summary>
        public void Clear()
        {
            _subRenderings.Clear();
        }

        public virtual void Process(TextWriter writer)
        {
            this._subRenderings.ForEach(x => x.Process(writer));
        }

        // Properties
        public RenderingBase this[int index]
        {
            get { return _subRenderings[index]; }
            set { _subRenderings[index] = value; }
        }

        public NameValueCollection Attributes
        {
            get { return _attributes; }
        }

        public int Count
        {
            get { return _subRenderings == null ? 0 : _subRenderings.Count; }
        }

        public virtual RenderingBase Parent
        {
            get;
            protected set;
        }

        public static RenderingBase CreateRoot()
        {
            return new Root();
        }

        /// <summary>
        /// Finds the first Rendering in the Renderings sub-renderings (recursive)
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>First matching rendering</returns>
        public RenderingBase Find(Filter filter)
        {
            RenderingBase result = null;
            if (filter.Pass(this))
                result = this;
            else
            {
                foreach (var item in _subRenderings)
                {
                    result = item.Find(filter);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        #region IEnumerable<Rendering> Members

        public IEnumerator<RenderingBase> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}