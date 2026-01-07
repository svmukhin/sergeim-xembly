// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class CursorTests
{
    [TestMethod]
    public void TestConstructorWithDocument()
    {
        var document = new XmlDocument();
        var cursor = new Cursor(document);
        Assert.IsNotNull(cursor);
        Assert.AreEqual(document, cursor.Document);
        Assert.AreEqual(1, cursor.Count);
        Assert.IsTrue(cursor.HasNodes);
        Assert.AreEqual(document, cursor.Nodes[0]);
    }

    [TestMethod]
    public void TestConstructorWithNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        var cursor = new Cursor(document, [child1, child2]);
        Assert.AreEqual(2, cursor.Count);
        Assert.IsTrue(cursor.HasNodes);
        Assert.AreEqual(child1, cursor.Nodes[0]);
        Assert.AreEqual(child2, cursor.Nodes[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorWithNullDocumentThrows()
    {
        _ = new Cursor(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorWithNullNodesThrows()
    {
        _ = new Cursor(new XmlDocument(), null!);
    }

    [TestMethod]
    public void TestAddNode()
    {
        var document = new XmlDocument();
        var cursor = new Cursor(document, Array.Empty<XmlNode>());
        var node = document.CreateElement("test");
        cursor.Add(node);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(node, cursor.Nodes[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAddNullNodeThrows()
    {
        new Cursor(new XmlDocument()).Add(null!);
    }

    [TestMethod]
    public void TestSetNodes()
    {
        var document = new XmlDocument();
        document.AppendChild(document.CreateElement("root"));
        var cursor = new Cursor(document);
        var newNode1 = document.CreateElement("new1");
        var newNode2 = document.CreateElement("new2");
        cursor.Set([newNode1, newNode2]);
        Assert.AreEqual(2, cursor.Count);
        Assert.AreEqual(newNode1, cursor.Nodes[0]);
        Assert.AreEqual(newNode2, cursor.Nodes[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestSetNullNodesThrows()
    {
        new Cursor(new XmlDocument()).Set(null!);
    }

    [TestMethod]
    public void TestSetClearsExistingNodes()
    {
        var document = new XmlDocument();
        var node1 = document.CreateElement("node1");
        var cursor = new Cursor(document, [node1]);
        var newNode = document.CreateElement("new");
        cursor.Set([newNode]);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(newNode, cursor.Nodes[0]);
    }

    [TestMethod]
    public void TestHasNodesWhenEmpty()
    {
        var cursor = new Cursor(new XmlDocument(), []);
        Assert.IsFalse(cursor.HasNodes);
        Assert.AreEqual(0, cursor.Count);
    }

    [TestMethod]
    public void TestNodesAreReadOnly()
    {
        var nodes = new Cursor(new XmlDocument()).Nodes;
        Assert.IsInstanceOfType(nodes, typeof(IReadOnlyList<XmlNode>));
    }
}
