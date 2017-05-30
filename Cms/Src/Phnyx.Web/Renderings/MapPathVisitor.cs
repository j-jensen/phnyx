using Phnyx.Web.Renderings;
namespace Phnyx.Web.Renderings
{
    public class MapPathVisitor : VisitorBase
    {
        private string _applicationPath;
        private string _templateVirtualPath;

        public MapPathVisitor(string application)
            : this(application, "~/")
        {
        }

        public MapPathVisitor(string applicationPath, string templateVirtualPath)
        {
            this.Filter = Filter.AttributeName("href")
                                .Or(Filter.AttributeName("src"));
            _applicationPath = PathUtility.RemoveTrailingSlashes(applicationPath);
            _templateVirtualPath = templateVirtualPath;
        }

        public override void OnVisit(RenderingBase rendering)
        {
            FixPath(rendering, "href");
            FixPath(rendering, "src");
        }

        private void FixPath(RenderingBase rendering, string name)
        {
            string path = rendering.Attributes[name];
            if (string.IsNullOrEmpty(path))
                return;

            path = PathUtility.Resolve(path, _applicationPath, _templateVirtualPath);
            rendering.Attributes[name] = path;
        }
    }
}
