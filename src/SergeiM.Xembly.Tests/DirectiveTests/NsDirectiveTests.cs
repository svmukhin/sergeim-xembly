// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Advanced;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class NsDirectiveTests
{
    [TestMethod]
    public void Execute_SetsDefaultNamespace()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        var directive = new NsDirective("", "http://example.com/ns");
        directive.Execute(cursor);
        var xmlns = doc.DocumentElement!.GetAttribute("xmlns");
        Assert.AreEqual("http://example.com/ns", xmlns);
    }

    [TestMethod]
    public void Execute_SetsPrefixedNamespace()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        var directive = new NsDirective("ex", "http://example.com/ns");
        directive.Execute(cursor);
        var xmlns = doc.DocumentElement!.GetAttribute("xmlns:ex");
        Assert.AreEqual("http://example.com/ns", xmlns);
    }

    [TestMethod]
    public void Execute_SetsNamespaceOnMultipleNodes()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><child1/><child2/></root>");
        var cursor = new Cursor(doc, [
            doc.SelectSingleNode("//child1")!,
            doc.SelectSingleNode("//child2")!
        ]);
        var directive = new NsDirective("ex", "http://example.com/ns");
        directive.Execute(cursor);
        var child1 = doc.SelectSingleNode("//child1") as XmlElement;
        var child2 = doc.SelectSingleNode("//child2") as XmlElement;
        Assert.AreEqual("http://example.com/ns", child1!.GetAttribute("xmlns:ex"));
        Assert.AreEqual("http://example.com/ns", child2!.GetAttribute("xmlns:ex"));
    }

    [TestMethod]
    public void Execute_ThrowsOnEmptyCursor()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var cursor = new Cursor(doc, []);
        var directive = new NsDirective("ex", "http://example.com/ns");
        Assert.ThrowsException<CursorException>(() => directive.Execute(cursor));
    }

    [TestMethod]
    [ExpectedException(typeof(XemblyException))]
    public void Execute_ThrowsOnNonElementNode()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root>text</root>");
        var textNode = doc.DocumentElement!.FirstChild!;
        var cursor = new Cursor(doc, [textNode]);
        new NsDirective("ex", "http://example.com/ns").Execute(cursor);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsOnNullPrefix()
    {
        new NsDirective(null!, "http://example.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsOnNullUri()
    {
        new NsDirective("ex", null!);
    }

    [TestMethod]
    public void ToString_ReturnsCorrectFormat_DefaultNamespace()
    {
        var directive = new NsDirective("", "http://example.com/ns");
        Assert.AreEqual("NS('http://example.com/ns')", directive.ToString());
    }

    [TestMethod]
    public void ToString_ReturnsCorrectFormat_PrefixedNamespace()
    {
        var directive = new NsDirective("ex", "http://example.com/ns");
        Assert.AreEqual("NS('ex', 'http://example.com/ns')", directive.ToString());
    }

    [TestMethod]
    public void Execute_OverridesExistingNamespace()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root xmlns:ex='http://old.com'/>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        var directive = new NsDirective("ex", "http://new.com");
        directive.Execute(cursor);
        var xmlns = doc.DocumentElement!.GetAttribute("xmlns:ex");
        Assert.AreEqual("http://new.com", xmlns);
    }

    [TestMethod]
    public void Execute_WorksWithMultiplePrefixes()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        new NsDirective("ex1", "http://example1.com").Execute(cursor);
        new NsDirective("ex2", "http://example2.com").Execute(cursor);
        Assert.AreEqual("http://example1.com", doc.DocumentElement!.GetAttribute("xmlns:ex1"));
        Assert.AreEqual("http://example2.com", doc.DocumentElement!.GetAttribute("xmlns:ex2"));
    }
}
