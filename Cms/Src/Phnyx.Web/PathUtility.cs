using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Phnyx.Web
{
    public static class PathUtility
    {
        public static bool IsAppRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (path[0] != '~')
                return false;

            if ((path.Length != 1) && (path[1] != '\\'))
                return (path[1] == '/');

            return true;
        }

        public static bool IsRelativePath(string path)
        {
            return path.StartsWith("../");
        }

        public static string Resolve(string path, string applicationPath, string filePath)
        {
            var fullFilepath = IsAppRelativePath(filePath) ? filePath.Replace("~", applicationPath) : filePath;

            if (IsAppRelativePath(path))
            {
                return path.Replace("~", applicationPath);
            }
            if (IsRelativePath(path))
            {
                return WalkUpdirs(path, fullFilepath);
            }

            if (!IsHostRoot(path))
            {
                return GetDirectory(fullFilepath) + path;
            }

            return path;
        }

        public static string WalkUpdirs(string path, string fullFilepath)
        {
            string dir = RemoveTrailingSlashes(GetDirectory(fullFilepath));
            while (path.Length > 0 && path.StartsWith("../"))
            {
                path = path.Substring(path.IndexOf("../") + 3);
                dir = dir.Substring(0, dir.LastIndexOf("/"));
            }

            return String.Format("{0}/{1}", dir, path);
        }

        public static bool IsHostRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return path[0] == '/';
        }

        public static string GetDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Empty path has nno directory");
            }
            if ((path[0] != '/') && (path[0] != '~'))
            {
                throw new ArgumentException("Path must be rooted");
            }
            if (path.Length == 1)
            {
                return path;
            }
            int num = path.LastIndexOf('/');
            if (num < 0)
            {
                throw new ArgumentException("Path must be rooted");
            }
            return path.Substring(0, num + 1);
        }

        public static string RemoveTrailingSlashes(string applicationPath)
        {
            int end = applicationPath[applicationPath.Length - 1] == '/' ? applicationPath.Length - 1 : applicationPath.Length;

            return applicationPath.Substring(0, end);
        }
    }
}
