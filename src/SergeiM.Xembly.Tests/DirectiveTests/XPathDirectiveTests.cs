// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class XPathDirectiveTests
{
    [TestMethod]
    public void TestSelectSingleNodeByName()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><child>text</child></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("child").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual("child", cursor.Nodes[0].Name);
        Assert.AreEqual("text", cursor.Nodes[0].InnerText);
    }

    [TestMethod]
    public void TestSelectMultipleNodes()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><item>1</item><item>2</item><item>3</item></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("item").Execute(cursor);
        Assert.AreEqual(3, cursor.Count);
        Assert.AreEqual("item", cursor.Nodes[0].Name);
        Assert.AreEqual("1", cursor.Nodes[0].InnerText);
        Assert.AreEqual("2", cursor.Nodes[1].InnerText);
        Assert.AreEqual("3", cursor.Nodes[2].InnerText);
    }

    [TestMethod]
    public void TestSelectByAttribute()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><item id='1'/><item id='2'/><item id='3'/></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("item[@id='2']").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual("2", (cursor.Nodes[0] as XmlElement)?.GetAttribute("id"));
    }

    [TestMethod]
    public void TestAbsoluteXPath()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><level1><level2>target</level2></level1></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("/root/level1/level2").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual("level2", cursor.Nodes[0].Name);
        Assert.AreEqual("target", cursor.Nodes[0].InnerText);
    }

    [TestMethod]
    public void TestDescendantAxis()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><a><item>1</item></a><b><item>2</item></b></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("//item").Execute(cursor);
        Assert.AreEqual(2, cursor.Count);
    }

    [TestMethod]
    public void TestNoMatchingNodes()
    {
        // Arrange
        var document = new XmlDocument();
        document.LoadXml("<root><child>text</child></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("nonexistent").Execute(cursor);
        Assert.AreEqual(0, cursor.Count);
        Assert.IsFalse(cursor.HasNodes);
    }

    [TestMethod]
    public void TestRelativeXPathFromMultipleNodes()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><a><item>1</item></a><b><item>2</item></b></root>");
        var a = document.SelectSingleNode("/root/a")!;
        var b = document.SelectSingleNode("/root/b")!;
        var cursor = new Cursor(document, [a, b]);
        new XPathDirective("item").Execute(cursor);
        Assert.AreEqual(2, cursor.Count);
        Assert.AreEqual("1", cursor.Nodes[0].InnerText);
        Assert.AreEqual("2", cursor.Nodes[1].InnerText);
    }

    [TestMethod]
    public void TestXPathWithPredicate()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><item>1</item><item>2</item><item>3</item></root>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("item[position()>1]").Execute(cursor);
        Assert.AreEqual(2, cursor.Count);
        Assert.AreEqual("2", cursor.Nodes[0].InnerText);
        Assert.AreEqual("3", cursor.Nodes[1].InnerText);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullExpressionThrows()
    {
        _ = new XPathDirective(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyExpressionThrows()
    {
        _ = new XPathDirective("");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceExpressionThrows()
    {
        _ = new XPathDirective("   ");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new XPathDirective("child").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new XPathDirective("child").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    [ExpectedException(typeof(XemblyException))]
    public void TestInvalidXPathThrows()
    {
        var document = new XmlDocument();
        document.LoadXml("<root/>");
        var cursor = new Cursor(document, [document.DocumentElement!]);
        new XPathDirective("//[invalid").Execute(cursor);
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new XPathDirective("//item[@id='1']").ToString();
        Assert.AreEqual("XPATH '//item[@id='1']'", str);
    }

    [TestMethod]
    public void TestXPathDeduplicatesResults()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><item>1</item></root>");
        var root = document.DocumentElement!;
        var cursor = new Cursor(document, [root, root]);
        new XPathDirective("item").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
    }
}
