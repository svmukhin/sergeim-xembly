using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Directive.XPath;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class PushDirectiveTests
{
    [TestMethod]
    public void Execute_SavesCurrentCursorPosition()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><a><b/></a></root>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        new XPathDirective("//a").Execute(cursor);
        var nodes = cursor.Nodes;
        Assert.AreEqual(1, nodes.Count);
        Assert.AreEqual("a", nodes[0].Name);
        new PushDirective().Execute(cursor);
        nodes = cursor.Nodes;
        Assert.AreEqual(1, nodes.Count);
        Assert.AreEqual("a", nodes[0].Name);
    }

    [TestMethod]
    public void Execute_AllowsMultiplePushes()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><a><b/></a></root>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        new PushDirective().Execute(cursor);
        new XPathDirective("//a").Execute(cursor);
        new PushDirective().Execute(cursor);
        new XPathDirective("//b").Execute(cursor);
        new PushDirective().Execute(cursor);
        var nodes = cursor.Nodes;
        Assert.AreEqual(1, nodes.Count);
        Assert.AreEqual("b", nodes[0].Name);
    }


    [TestMethod]
    public void ToString_ReturnsCorrectFormat()
    {
        var result = new PushDirective().ToString();
        Assert.AreEqual("PUSH", result);
    }
}
