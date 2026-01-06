// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class NamespaceTests
{
    [TestMethod]
    public void Directives_Ns_DefaultNamespace()
    {
        var directives = new Directives()
            .Add("root")
            .Ns("", "http://example.com/ns");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example.com/ns", doc.DocumentElement!.GetAttribute("xmlns"));
    }

    [TestMethod]
    public void Directives_Ns_PrefixedNamespace()
    {
        var directives = new Directives()
            .Add("root")
            .Ns("ex", "http://example.com/ns");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example.com/ns", doc.DocumentElement!.GetAttribute("xmlns:ex"));
    }

    [TestMethod]
    public void Directives_Ns_MultiplePrefixes()
    {
        var directives = new Directives()
            .Add("root")
            .Ns("ns1", "http://example1.com")
            .Ns("ns2", "http://example2.com")
            .Add("child")
            .Ns("ns3", "http://example3.com");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example1.com", doc.DocumentElement!.GetAttribute("xmlns:ns1"));
        Assert.AreEqual("http://example2.com", doc.DocumentElement!.GetAttribute("xmlns:ns2"));        
        var child = doc.SelectSingleNode("//child") as XmlElement;
        Assert.AreEqual("http://example3.com", child!.GetAttribute("xmlns:ns3"));
    }

    [TestMethod]
    public void Parser_ParsesNs_DefaultNamespace()
    {
        var script = "ADD 'root'; NS 'http://example.com/ns';";
        var directives = new Directives(script);
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example.com/ns", doc.DocumentElement!.GetAttribute("xmlns"));
    }

    [TestMethod]
    public void Parser_ParsesNs_PrefixedNamespace()
    {
        var script = "ADD 'root'; NS 'ex', 'http://example.com/ns';";
        var directives = new Directives(script);
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example.com/ns", doc.DocumentElement!.GetAttribute("xmlns:ex"));
    }

    [TestMethod]
    public void Parser_ParsesNs_MultiplePrefixes()
    {
        var script = """
            ADD 'root';
            NS 'ns1', 'http://example1.com';
            NS 'ns2', 'http://example2.com';
            """;        
        var directives = new Directives(script);
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example1.com", doc.DocumentElement!.GetAttribute("xmlns:ns1"));
        Assert.AreEqual("http://example2.com", doc.DocumentElement!.GetAttribute("xmlns:ns2"));
    }

    [TestMethod]
    public void Integration_RealWorldNamespaceUsage()
    {
        var directives = new Directives()
            .Add("soap:Envelope")
            .Ns("soap", "http://schemas.xmlsoap.org/soap/envelope/")
            .Ns("xsi", "http://www.w3.org/2001/XMLSchema-instance")
            .Add("soap:Body")
            .Add("GetUserRequest")
            .Ns("", "http://example.com/api")
            .Add("userId")
            .Set("12345");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var root = doc.DocumentElement!;
        Assert.AreEqual("http://schemas.xmlsoap.org/soap/envelope/", root.GetAttribute("xmlns:soap"));
        Assert.AreEqual("http://www.w3.org/2001/XMLSchema-instance", root.GetAttribute("xmlns:xsi"));
    }

    [TestMethod]
    public void Integration_XmlNamespaceWithAttributes()
    {
        var directives = new Directives()
            .Add("books")
            .Ns("", "http://example.com/books")
            .Add("book")
            .Attr("id", "1")
            .Add("title")
            .Set("XML Namespaces");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        Assert.AreEqual("http://example.com/books", doc.DocumentElement!.GetAttribute("xmlns"));
        var nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("ns", "http://example.com/books");
        var book = doc.SelectSingleNode("//ns:book", nsmgr) as XmlElement;
        Assert.IsNotNull(book);
        Assert.AreEqual("1", book.GetAttribute("id"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Directives_Ns_ThrowsOnNullPrefix()
    {
        new Directives().Ns(null!, "http://example.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Directives_Ns_ThrowsOnNullUri()
    {
        new Directives().Ns("ex", null!);
    }
}
