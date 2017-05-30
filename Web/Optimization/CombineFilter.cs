using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Phnyx.Web.Optimization
{
    public class ScriptCombineFilter : CombineFilter
    {
        private static byte[] INSERT_PATTERN = Encoding.UTF8.GetBytes("</body>");
        private static byte[] START_PATTERN = Encoding.UTF8.GetBytes("<script");
        private static byte[] END_PATTERN = Encoding.UTF8.GetBytes("</script>");

        public ScriptCombineFilter(Stream output) : base(output) { }

        protected override byte[] InsertPattern { get { return INSERT_PATTERN; } }
        protected override byte[] StartPattern { get { return START_PATTERN; } }
        protected override byte[] EndPattern { get { return END_PATTERN; } }
        protected override string BundleFormat { get { return "<script src=\"{0}?v={1}\"></script>"; } }
    }

    public abstract class CombineFilter : MemoryStream
    {
        protected virtual byte[] InsertPattern { get; private set; }
        protected virtual byte[] StartPattern { get; private set; }
        protected virtual byte[] EndPattern { get; private set; }
        protected virtual string BundleFormat { get; private set; }

        private static byte[] BUNDLE_PATTERN = Encoding.UTF8.GetBytes("data-bundle=\"");
        private static string VERSION = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", "");

        private Stream output;
        private List<byte> scriptBlocks;
        private Action<byte[]> OnScriptHandler;
        private bool scriptInserted = false;

        public CombineFilter(Stream output)
        {
            this.output = output;
            this.scriptBlocks = new List<byte>();
            OnScriptHandler = (s) =>
            {
                FoundResources++;
                var m = s.Match(BUNDLE_PATTERN, new byte[] { (byte)'"' });
                if (m.IsMatch)
                {
                    string bundle = Encoding.UTF8.GetString(s.SubArray(m.Begin + BUNDLE_PATTERN.Length, m.Length - BUNDLE_PATTERN.Length - 1));
                    if (!this.Bundles.Contains(bundle))
                    {
                        this.Bundles.Add(bundle);
                        scriptBlocks.AddRange(Encoding.UTF8.GetBytes(string.Format(BundleFormat, bundle, VERSION)));
                    }
                }
                else
                {
                    scriptBlocks.AddRange(s);
                }
            };
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var posBodyend = buffer.IndexOfPattern(InsertPattern, offset, count);
            if (!scriptInserted && posBodyend > -1)
            {
                ProcessBuffer(buffer, offset, (posBodyend - offset), output.Write, OnScriptHandler);

                var script = scriptBlocks.ToArray();
                output.Write(script, 0, script.Length);
                scriptInserted = true;

                int restCount = (count - (posBodyend - offset));
                if (restCount > 0)
                    output.Write(buffer, posBodyend, restCount);
            }
            else
            {
                ProcessBuffer(buffer, offset, count, output.Write, OnScriptHandler);
            }
        }

        private void ProcessBuffer(byte[] buffer, int offset, int count, Action<byte[], int, int> write, Action<byte[]> onScript)
        {
            var m = buffer.Match(StartPattern, EndPattern, offset, count);
            if (m.IsMatch)
            {
                if (m.Begin > 0)
                    write(buffer, offset, (m.Begin - offset));

                onScript(buffer.SubArray(m.Begin, m.Length));

                int restOffset = (m.Begin + m.Length),
                    restCount = (offset + count) - restOffset;
                ProcessBuffer(buffer, restOffset, restCount, write, onScript);
            }
            else
            {
                write(buffer, offset, count);
            }
        }

        public List<string> Bundles = new List<string>();
        public int FoundResources;
    }
}
