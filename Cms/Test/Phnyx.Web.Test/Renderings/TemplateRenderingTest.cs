using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Phnyx.Web.Renderings;

namespace Phnyx.Web.Renderings.Test
{
    [TestClass, DeploymentItem(@".\Web\Layout\vcard_template.htm")]
    public class TemplateRenderingTest
    {
        private static string TEMPLATE_PATH = System.IO.Path.GetFullPath("vcard_template.htm");
        [TestMethod]
        public void ShouldParseTemplateAndSelectElementWithID_template_AsRoot()
        {
            var sut = new Template(File.OpenText(TEMPLATE_PATH), Filter.ElementId("template"));

            Assert.AreEqual("template", sut[0].Attributes["id"]);
        }

        [TestMethod]
        public void ShouldBeAbleToBindData()
        {
            var sut = new Template(File.OpenText(TEMPLATE_PATH), Filter.ElementId("template"));
            var data = new VCard { Name = "Jesper Jensen" };

            sut.Bind(data);

            Assert.AreEqual(data.Name, ((Literal)sut[0][1][1][1][0]).Content);
        }

        [TestMethod]
        public void ShouldReturnFullDocumentIfNoFilter()
        {
            var sut = new Template(File.OpenText(TEMPLATE_PATH), null);

            Assert.IsNotNull(sut[0]);
        }

        [TestMethod]
        public void ShouldIgnoreDataWithoutDestinationOnBind()
        {
            var html = "<head><title>Old title</title></head>";
            StringReader reader = new StringReader(html);
            StringWriter writer = new StringWriter();

            var sut = new Template(reader, Filter.ElementName("head"));
            sut.Bind(new { UnknownDataProperty = "Unknown data" });
            sut.Process(writer);

            Assert.AreEqual<string>(html, writer.ToString());
        }

        [TestMethod]
        public void ShouldInsertDataOnTagmatch()
        {
            StringReader reader = new StringReader("<head><title>Old title</title></head>");
            StringWriter writer = new StringWriter();

            var sut = new Template(reader, Filter.ElementName("head"));
            sut.Bind(new { Title = "New Title" });
            sut.Process(writer);

            Assert.AreEqual<string>("<head><title>New Title</title></head>", writer.ToString());
        }

        [TestMethod]
        public void ShouldInsertDataOnMatchingAttributeIfNoMatchingName()
        {
            StringReader reader = new StringReader("<head><title id=\"Id\">Old title</title></head>");
            StringWriter writer = new StringWriter();

            var sut = new Template(reader, Filter.ElementName("head"));
            sut.Bind(new { Id = "New Title" });
            sut.Process(writer);

            Assert.AreEqual<string>("<head><title id=\"Id\">New Title</title></head>", writer.ToString());
        }

        [TestMethod]
        public void InvokeVisitorShouldReflectOnResult()
        {
            StringReader reader = new StringReader("<head><title id=\"id\">Old title</title></head>");
            StringWriter writer = new StringWriter();

            var visitor = new InnerTextVisitor("New Title");
            visitor.Filter = Filter.ElementId("id");
            var sut = new Template(reader, Filter.ElementName("head"));

            sut.Accept(visitor);

            sut.Process(writer);
            Assert.AreEqual<string>("<head><title id=\"id\">New Title</title></head>", writer.ToString());
        }
    }

    internal class VCard
    {
        [Markup("Name")]
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
