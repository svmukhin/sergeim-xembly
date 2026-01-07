// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class SetDirectiveTests
{
    [TestMethod]
    public void TestSetTextOnSingleNode()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, [root]);
        new SetDirective("test value").Execute(cursor);
        Assert.AreEqual("test value", root.InnerText);
    }

    [TestMethod]
    public void TestSetTextOnMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        new SetDirective("same text").Execute(new Cursor(document, [child1, child2]));
        Assert.AreEqual("same text", child1.InnerText);
        Assert.AreEqual("same text", child2.InnerText);
    }

    [TestMethod]
    public void TestSetEmptyString()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        root.InnerText = "original";
        document.AppendChild(root);
        new SetDirective("").Execute(new Cursor(document, [root]));
        Assert.AreEqual("", root.InnerText);
    }

    [TestMethod]
    public void TestSetReplacesExistingText()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        root.InnerText = "old value";
        document.AppendChild(root);
        new SetDirective("new value").Execute(new Cursor(document, [root]));
        Assert.AreEqual("new value", root.InnerText);
    }

    [TestMethod]
    public void TestSetWithSpecialCharacters()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new SetDirective("<>&\"'").Execute(new Cursor(document, [root]));
        Assert.AreEqual("<>&\"'", root.InnerText);
        Assert.IsTrue(document.OuterXml.Contains("&lt;&gt;&amp;"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorWithNullValueThrows()
    {
        _ = new SetDirective(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new SetDirective("value").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        var document = new XmlDocument();
        var cursor = new Cursor(document, []);
        new SetDirective("value").Execute(cursor);
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new SetDirective("test").ToString();
        Assert.AreEqual("SET 'test'", str);
    }

    [TestMethod]
    public void TestCursorUnchangedAfterSet()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, [root]);
        new SetDirective("value").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(root, cursor.Nodes[0]);
    }
}
