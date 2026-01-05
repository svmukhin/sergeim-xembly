// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class AddDirectiveTests
{
    [TestMethod]
    public void TestAddSingleNode()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, [root]);
        new AddDirective("child").Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual("child", cursor.Nodes[0].Name);
        Assert.AreEqual(root, cursor.Nodes[0].ParentNode);
    }

    [TestMethod]
    public void TestAddToMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        root.AppendChild(child1);
        root.AppendChild(child2);
        var cursor = new Cursor(document, [child1, child2]);
        new AddDirective("item").Execute(cursor);
        Assert.AreEqual(2, cursor.Count);
        Assert.AreEqual("item", cursor.Nodes[0].Name);
        Assert.AreEqual("item", cursor.Nodes[1].Name);
        Assert.AreEqual(1, child1.ChildNodes.Count);
        Assert.AreEqual(1, child2.ChildNodes.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestNullNameThrows()
    {
        _ = new AddDirective(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestEmptyNameThrows()
    {
        _ = new AddDirective("");
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new AddDirective("node").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    public void TestToString()
    {
        Assert.AreEqual("ADD 'test'", new AddDirective("test").ToString());
    }
}
