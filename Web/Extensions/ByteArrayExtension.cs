using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phnyx
{
    public static class ByteArrayExtension
    {
        public static int IndexOfPattern(this byte[] buffer, byte[] pattern)
        {
            return buffer.IndexOfPattern(pattern, 0, buffer.Length);
        }

        public static int IndexOfPattern(this byte[] buffer, byte[] pattern, int offset, int count)
        {
            int endidx = offset + count - pattern.Length;
            if (endidx < offset)
                return -1;

            for (int i = offset; i <= endidx; i++)
            {
                bool match = true;
                if (buffer[i] == pattern[0])
                {
                    for (int j = 1; j < pattern.Length; j++)
                    {
                        if (buffer[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match) return i;
                }
            }
            return -1;
        }

        public static PatternMatch Match(this byte[] buffer, byte[] startPattern, byte[] endPattern)
        {
            return buffer.Match(startPattern, endPattern, 0, buffer.Length);
        }

        public static PatternMatch Match(this byte[] buffer, byte[] startPattern, byte[] endPattern, int offset, int count)
        {
            var begin = buffer.IndexOfPattern(startPattern, offset, count);
            if (begin > -1)
            {
                int restIdx = (begin + startPattern.Length), restLength = (offset + count) - restIdx;
                var end = buffer.IndexOfPattern(endPattern, restIdx, restLength);
                if (end > -1)
                {
                    return new PatternMatch(begin, (end - begin + endPattern.Length));
                }
                else
                {
                    return new PatternMatch(begin, startPattern.Length, true);
                }
            }

            return PatternMatch.NoMatch;
        }

        public static byte[] SubArray(this byte[] bytes, int startIndex)
        {
            return bytes.SubArray(startIndex, bytes.Length - startIndex);
        }

        public static byte[] SubArray(this byte[] bytes, int startIndex, int length)
        {
            var end = new byte[length];
            Array.Copy(bytes, startIndex, end, 0, length);
            return end;
        }

        public static string AsUTF8String(this byte[] self)
        {
            return Encoding.UTF8.GetString(self);
        }

        public struct PatternMatch
        {
            public static PatternMatch NoMatch = new PatternMatch(-1, 0);
            public PatternMatch(int begin, int length, bool isPartial = false)
            {
                this.Begin = begin;
                this.Length = length;
                this.IsPartial = isPartial;

            }
            public readonly int Begin;
            public readonly int Length;
            public readonly bool IsPartial;

            public bool IsMatch
            {
                get { return this != NoMatch && !IsPartial; }
            }

            #region Compare
            public static bool operator ==(PatternMatch p1, PatternMatch p2)
            {
                return (p1.Begin == p2.Begin) && p1.Length == p2.Length;
            }

            public static bool operator !=(PatternMatch p1, PatternMatch p2)
            {
                return !(p1 == p2);
            }

            public override bool Equals(object obj)
            {
                if (obj is PatternMatch)
                {
                    return (PatternMatch)obj == this;
                }

                return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion
        }
    }
}
