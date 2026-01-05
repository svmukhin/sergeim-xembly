// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class UpDirectiveTests
{
    [TestMethod]
    public void TestMoveUpFromSingleNode()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child = document.CreateElement("child");
        document.AppendChild(root);
        root.AppendChild(child);
        var cursor = new Cursor(document, [child]);
        new UpDirective().Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(root, cursor.Nodes[0]);
    }

    [TestMethod]
    public void TestMoveUpFromMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        var cursor = new Cursor(document, [child1, child2]);
        new UpDirective().Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(root, cursor.Nodes[0]);
    }

    [TestMethod]
    public void TestMoveUpDeduplicatesParents()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var parent1 = document.CreateElement("parent1");
        var parent2 = document.CreateElement("parent2");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        var child3 = document.CreateElement("child3");
        document.AppendChild(root);
        root.AppendChild(parent1);
        root.AppendChild(parent2);
        parent1.AppendChild(child1);
        parent1.AppendChild(child2);
        parent2.AppendChild(child3);
        var cursor = new Cursor(document, [child1, child2, child3]);
        new UpDirective().Execute(cursor);
        Assert.AreEqual(2, cursor.Count);
        Assert.IsTrue(cursor.Nodes.Contains(parent1));
        Assert.IsTrue(cursor.Nodes.Contains(parent2));
    }

    [TestMethod]
    public void TestMoveUpMultipleLevels()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var level1 = document.CreateElement("level1");
        var level2 = document.CreateElement("level2");
        document.AppendChild(root);
        root.AppendChild(level1);
        level1.AppendChild(level2);
        var cursor = new Cursor(document, [level2]);
        var directive = new UpDirective();
        directive.Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(level1, cursor.Nodes[0]);
        directive.Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(root, cursor.Nodes[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new UpDirective().Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new UpDirective().Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    public void TestMoveUpFromRootToDocument()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, [root]);
        new UpDirective().Execute(cursor);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(document, cursor.Nodes[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestMoveUpFromDocumentNodeThrows()
    {
        new UpDirective().Execute(new Cursor(new XmlDocument(), [(new XmlDocument())]));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new UpDirective().ToString();
        Assert.AreEqual("UP", str);
    }

    [TestMethod]
    public void TestMoveUpPreservesOrder()
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
        new UpDirective().Execute(cursor);
        Assert.AreEqual(2, cursor.Count);
        Assert.AreEqual(parent1, cursor.Nodes[0]);
        Assert.AreEqual(parent2, cursor.Nodes[1]);
    }
}
