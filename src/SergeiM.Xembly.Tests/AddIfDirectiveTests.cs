// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class AddIfDirectiveTests
{
    [TestMethod]
    public void TestAddIfNodeDoesNotExist()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, new[] { root });
        new AddIfDirective("child").Execute(cursor);
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual("child", root.ChildNodes[0]!.Name);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual("child", cursor.Nodes[0].Name);
    }

    [TestMethod]
    public void TestAddIfNodeAlreadyExists()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var existing = document.CreateElement("child");
        document.AppendChild(root);
        root.AppendChild(existing);
        var cursor = new Cursor(document, [root]);
        new AddIfDirective("child").Execute(cursor);
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(existing, root.ChildNodes[0]);
        Assert.AreEqual(1, cursor.Count);
        Assert.AreEqual(existing, cursor.Nodes[0]);
    }

    [TestMethod]
    public void TestAddIfOnMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var parent1 = document.CreateElement("parent1");
        var parent2 = document.CreateElement("parent2");
        document.AppendChild(root);
        root.AppendChild(parent1);
        root.AppendChild(parent2);
        var cursor = new Cursor(document, new XmlNode[] { parent1, parent2 });
        new AddIfDirective("item").Execute(cursor);
        Assert.AreEqual(1, parent1.ChildNodes.Count);
        Assert.AreEqual(1, parent2.ChildNodes.Count);
        Assert.AreEqual(2, cursor.Count);
    }

    [TestMethod]
    public void TestAddIfMixedExistingAndNew()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var parent1 = document.CreateElement("parent1");
        var parent2 = document.CreateElement("parent2");
        var existing = document.CreateElement("item");
        document.AppendChild(root);
        root.AppendChild(parent1);
        root.AppendChild(parent2);
        parent1.AppendChild(existing);
        var cursor = new Cursor(document, [parent1, parent2]);
        new AddIfDirective("item").Execute(cursor);
        Assert.AreEqual(1, parent1.ChildNodes.Count); // Still 1 (existing)
        Assert.AreEqual(1, parent2.ChildNodes.Count); // New one added
        Assert.AreEqual(2, cursor.Count);
        Assert.AreEqual(existing, cursor.Nodes[0]); // First is existing
    }

    [TestMethod]
    public void TestAddIfMultipleTimes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, [root]);
        var directive = new AddIfDirective("child");
        directive.Execute(cursor);
        var firstChild = cursor.Nodes[0];
        cursor.Set([root]);
        directive.Execute(cursor);
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(firstChild, cursor.Nodes[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullNameThrows()
    {
        _ = new AddIfDirective(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyNameThrows()
    {
        _ = new AddIfDirective("");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceNameThrows()
    {
        _ = new AddIfDirective("   ");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new AddIfDirective("child").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new AddIfDirective("child").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new AddIfDirective("test").ToString();
        Assert.AreEqual("ADDIF 'test'", str);
    }
}
