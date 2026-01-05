// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class AttrDirectiveTests
{
    [TestMethod]
    public void TestSetAttributeOnSingleNode()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new AttrDirective("id", "123").Execute(new Cursor(document, [root]));
        Assert.AreEqual("123", root.GetAttribute("id"));
    }

    [TestMethod]
    public void TestSetAttributeOnMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        new AttrDirective("status", "active").Execute(new Cursor(document, [child1, child2]));
        Assert.AreEqual("active", child1.GetAttribute("status"));
        Assert.AreEqual("active", child2.GetAttribute("status"));
    }

    [TestMethod]
    public void TestUpdateExistingAttribute()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        root.SetAttribute("id", "old");
        document.AppendChild(root);
        new AttrDirective("id", "new").Execute(new Cursor(document, [root]));
        Assert.AreEqual("new", root.GetAttribute("id"));
    }

    [TestMethod]
    public void TestSetEmptyAttributeValue()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new AttrDirective("empty", "").Execute(new Cursor(document, [root]));
        Assert.AreEqual("", root.GetAttribute("empty"));
        Assert.IsTrue(root.HasAttribute("empty"));
    }

    [TestMethod]
    public void TestSetAttributeWithSpecialCharacters()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new AttrDirective("data", "<>&\"'").Execute(new Cursor(document, [root]));
        Assert.AreEqual("<>&\"'", root.GetAttribute("data"));
        Assert.IsTrue(document.OuterXml.Contains("&lt;&gt;&amp;"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullNameThrows()
    {
        _ = new AttrDirective(null!, "value");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyNameThrows()
    {
        _ = new AttrDirective("", "value");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceNameThrows()
    {
        _ = new AttrDirective("   ", "value");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorWithNullValueThrows()
    {
        _ = new AttrDirective("name", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new AttrDirective("name", "value").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new AttrDirective("name", "value").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    [ExpectedException(typeof(XemblyException))]
    public void TestExecuteOnNonElementNodeThrows()
    {
        var document = new XmlDocument();
        var textNode = document.CreateTextNode("text");
        new AttrDirective("name", "value").Execute(new Cursor(document, [textNode]));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new AttrDirective("id", "123").ToString();
        Assert.AreEqual("ATTR 'id', '123'", str);
    }

    [TestMethod]
    public void TestCursorUnchangedAfterAttr()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, [root]);
        new AttrDirective("id", "123").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(root, cursor.Nodes[0]);
    }
}
