// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Advanced;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class CDataDirectiveTests
{
    [TestMethod]
    public void TestSetCDataOnSingleNode()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new CDataDirective("test data").Execute(new Cursor(document, [root]));
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(XmlNodeType.CDATA, root.ChildNodes[0]!.NodeType);
        Assert.AreEqual("test data", root.ChildNodes[0]!.Value);
    }

    [TestMethod]
    public void TestCDataReplacesExistingContent()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        root.InnerText = "old text";
        document.AppendChild(root);
        new CDataDirective("new data").Execute(new Cursor(document, [root]));
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(XmlNodeType.CDATA, root.ChildNodes[0]!.NodeType);
        Assert.AreEqual("new data", root.ChildNodes[0]!.Value);
    }

    [TestMethod]
    public void TestCDataWithSpecialCharacters()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new CDataDirective("<>&\"'").Execute(new Cursor(document, [root]));
        var cdata = root.ChildNodes[0] as XmlCDataSection;
        Assert.IsNotNull(cdata);
        Assert.AreEqual("<>&\"'", cdata.Value);
        Assert.IsTrue(document.OuterXml.Contains("<![CDATA[<>&\"']]>"));
    }

    [TestMethod]
    public void TestCDataOnMultipleNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        new CDataDirective("same data").Execute(new Cursor(document, [child1, child2]));
        Assert.AreEqual(XmlNodeType.CDATA, child1.ChildNodes[0]!.NodeType);
        Assert.AreEqual(XmlNodeType.CDATA, child2.ChildNodes[0]!.NodeType);
        Assert.AreEqual("same data", child1.ChildNodes[0]!.Value);
        Assert.AreEqual("same data", child2.ChildNodes[0]!.Value);
    }

    [TestMethod]
    public void TestCDataWithEmptyString()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new CDataDirective("").Execute(new Cursor(document, [root]));
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(XmlNodeType.CDATA, root.ChildNodes[0]!.NodeType);
        Assert.AreEqual("", root.ChildNodes[0]!.Value);
    }

    [TestMethod]
    public void TestCDataClearsChildren()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        new CDataDirective("data").Execute(new Cursor(document, [root]));
        Assert.AreEqual(1, root.ChildNodes.Count);
        Assert.AreEqual(XmlNodeType.CDATA, root.ChildNodes[0]!.NodeType);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestConstructorWithNullValueThrows()
    {
        _ = new CDataDirective(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new CDataDirective("data").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new CDataDirective("data").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new CDataDirective("test").ToString();
        Assert.AreEqual("CDATA 'test'", str);
    }
}
