using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Phnyx.Web.Renderings
{
    [SuppressMessage("Microsoft.Naming", "CA1710", Justification = "Seen as a Rendering with child renderings - not a collecttion of renderings")]
    public class Repeater<T> : RenderingBase
    {
        private IEnumerable<T> _source;
        private Func<T, string> _formatter;

        public Repeater()
            : this(t => t.ToString())
        {
        }

        public Repeater(Func<T, string> formatter)
        {
            _formatter = formatter;
        }

        public void SetSource(IEnumerable<T> source)
        {
            _source = source;
        }

        public void SetFormatter(Func<T, string> formatter)
        {
            _formatter = formatter;
        }

        public override void Process(System.IO.TextWriter writer)
        {
            foreach (var item in _source)
            {
                writer.Write(_formatter(item));
            }
        }
    }
}
