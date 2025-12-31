using System.Xml;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class XemblerTests
{
    [TestMethod]
    public void TestSimpleAddAndSet()
    {
        var xml = new Xembler(new Directives()
            .Add("root")
            .Add("order")
            .Set("$140.00")).Xml();
        Assert.IsTrue(xml.Contains("<root>"));
        Assert.IsTrue(xml.Contains("<order>$140.00</order>"));
        Assert.IsTrue(xml.Contains("</root>"));
    }

    [TestMethod]
    public void TestAddWithAttribute()
    {
        var xml = new Xembler(new Directives()
            .Add("root")
            .Add("order")
            .Attr("id", "553")
            .Set("$140.00")).Xml();
        Assert.IsTrue(xml.Contains("<order id=\"553\">"));
        Assert.IsTrue(xml.Contains("$140.00"));
    }

    [TestMethod]
    public void TestMultipleNodes()
    {
        var items = new Xembler(new Directives()
            .Add("root")
            .Add("item")
            .Set("First")
            .Up()
            .Add("item")
            .Set("Second")).Document().SelectNodes("//item");
        Assert.IsNotNull(items);
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("First", items[0]?.InnerText);
        Assert.AreEqual("Second", items[1]?.InnerText);
    }

    [TestMethod]
    public void TestApplyToExistingDocument()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("existing");
        document.AppendChild(root);
        new Xembler(new Directives()
            .Add("new")
            .Set("value")).Apply(document);
        var node = document.SelectSingleNode("//existing/new");
        Assert.IsNotNull(node);
        Assert.AreEqual("value", node.InnerText);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestNullDirectivesThrows()
    {
        _ = new Xembler(null!);
    }
}
