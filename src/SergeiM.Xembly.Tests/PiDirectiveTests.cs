// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class PiDirectiveTests
{
    [TestMethod]
    public void TestAddProcessingInstruction()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new PiDirective("xml-stylesheet", "type=\"text/xsl\" href=\"style.xsl\"").Execute(new Cursor(document, [root]));
        Assert.AreEqual(1, root.ChildNodes.Count);
        var pi = root.ChildNodes[0] as XmlProcessingInstruction;
        Assert.IsNotNull(pi);
        Assert.AreEqual("xml-stylesheet", pi.Target);
        Assert.AreEqual("type=\"text/xsl\" href=\"style.xsl\"", pi.Data);
    }

    [TestMethod]
    public void TestPiOnMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        new PiDirective("target", "data").Execute(new Cursor(document, [child1, child2]));
        Assert.AreEqual(1, child1.ChildNodes.Count);
        Assert.AreEqual(1, child2.ChildNodes.Count);
        Assert.AreEqual(XmlNodeType.ProcessingInstruction, child1.ChildNodes[0]!.NodeType);
        Assert.AreEqual(XmlNodeType.ProcessingInstruction, child2.ChildNodes[0]!.NodeType);
    }

    [TestMethod]
    public void TestPiWithEmptyData()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new PiDirective("target", "").Execute(new Cursor(document, [root]));
        var pi = root.ChildNodes[0] as XmlProcessingInstruction;
        Assert.IsNotNull(pi);
        Assert.AreEqual("target", pi.Target);
        Assert.AreEqual("", pi.Data);
    }

    [TestMethod]
    public void TestPiPreservesExistingChildren()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child = document.CreateElement("child");
        document.AppendChild(root);
        root.AppendChild(child);
        new PiDirective("target", "data").Execute(new Cursor(document, [root]));
        Assert.AreEqual(2, root.ChildNodes.Count);
        Assert.AreEqual(child, root.ChildNodes[0]);
        Assert.AreEqual(XmlNodeType.ProcessingInstruction, root.ChildNodes[1]!.NodeType);
    }

    [TestMethod]
    public void TestMultiplePis()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        var cursor = new Cursor(document, new[] { root });
        var directive1 = new PiDirective("target1", "data1");
        var directive2 = new PiDirective("target2", "data2");
        directive1.Execute(cursor);
        directive2.Execute(cursor);
        Assert.AreEqual(2, root.ChildNodes.Count);
        var pi1 = root.ChildNodes[0] as XmlProcessingInstruction;
        var pi2 = root.ChildNodes[1] as XmlProcessingInstruction;
        Assert.AreEqual("target1", pi1?.Target);
        Assert.AreEqual("target2", pi2?.Target);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullTargetThrows()
    {
        _ = new PiDirective(null!, "data");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyTargetThrows()
    {
        _ = new PiDirective("", "data");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceTargetThrows()
    {
        _ = new PiDirective("   ", "data");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorWithNullDataThrows()
    {
        _ = new PiDirective("target", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new PiDirective("target", "data").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new PiDirective("target", "data").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new PiDirective("target", "data").ToString();
        Assert.AreEqual("PI 'target', 'data'", str);
    }
}
