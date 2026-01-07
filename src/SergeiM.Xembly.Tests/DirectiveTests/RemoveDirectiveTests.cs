// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class RemoveDirectiveTests
{
    [TestMethod]
    public void TestRemoveSingleNode()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child = document.CreateElement("child");
        document.AppendChild(root);
        root.AppendChild(child);
        var cursor = new Cursor(document, [child]);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(0, root.ChildNodes.Count);
        Assert.AreEqual(0, cursor.Count);
        Assert.IsFalse(cursor.HasNodes);
    }

    [TestMethod]
    public void TestRemoveMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        var child3 = document.CreateElement("child3");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        root.AppendChild(child3);
        var cursor = new Cursor(document, [child1, child3]);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(child2, root.ChildNodes[0]);
        Assert.AreEqual(0, cursor.Count);
    }

    [TestMethod]
    public void TestRemoveFromDifferentParents()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var parent1 = document.CreateElement("parent1");
        var parent2 = document.CreateElement("parent2");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(parent1);
        root.AppendChild(parent2);
        parent1.AppendChild(child1);
        parent2.AppendChild(child2);
        var cursor = new Cursor(document, [child1, child2]);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(0, parent1.ChildNodes.Count);
        Assert.AreEqual(0, parent2.ChildNodes.Count);
        Assert.AreEqual(0, cursor.Count);
    }

    [TestMethod]
    public void TestRemoveLeavesCursorEmpty()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child = document.CreateElement("child");
        document.AppendChild(root);
        root.AppendChild(child);
        var cursor = new Cursor(document, [child]);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(0, cursor.Count);
        Assert.IsFalse(cursor.HasNodes);
    }

    [TestMethod]
    public void TestRemoveNodeWithChildren()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var parent = document.CreateElement("parent");
        var child = document.CreateElement("child");
        document.AppendChild(root);
        root.AppendChild(parent);
        parent.AppendChild(child);
        var cursor = new Cursor(document, [parent]);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(0, root.ChildNodes.Count);
        Assert.AreEqual(0, cursor.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new RemoveDirective().Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new RemoveDirective().Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestRemoveDocumentNodeThrows()
    {
        new RemoveDirective().Execute(new Cursor(new XmlDocument(), [(new XmlDocument())]));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new RemoveDirective().ToString();
        Assert.AreEqual("REMOVE", str);
    }

    [TestMethod]
    public void TestRemoveInLoop()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        var cursor = new Cursor(document, [root]);
        new AddDirective("temp").Execute(cursor);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(0, cursor.Count);
        Assert.AreEqual(2, root.ChildNodes.Count);
    }

    [TestMethod]
    public void TestRemovePreservesOtherSiblings()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        var child3 = document.CreateElement("child3");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        root.AppendChild(child3);
        var cursor = new Cursor(document, [child2]);
        new RemoveDirective().Execute(cursor);
        Assert.AreEqual(2, root.ChildNodes.Count);
        Assert.AreEqual(child1, root.ChildNodes[0]);
        Assert.AreEqual(child3, root.ChildNodes[1]);
    }
}
