// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Directive.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class PopDirectiveTests
{

    [TestMethod]
    public void Execute_RestoresSavedCursorPosition()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><a><b/></a></root>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        new PushDirective().Execute(cursor);
        new XPathDirective("//a").Execute(cursor);
        var nodesBeforePop = cursor.Nodes;
        Assert.AreEqual("a", nodesBeforePop[0].Name);
        new PopDirective().Execute(cursor);
        var nodes = cursor.Nodes;
        Assert.AreEqual(1, nodes.Count);
        Assert.AreEqual("root", nodes[0].Name);
    }

    [TestMethod]
    public void Execute_WithEmptyStack_ThrowsCursorException()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        var ex = Assert.ThrowsException<CursorException>(() => new PopDirective().Execute(cursor));
        Assert.IsTrue(ex.Message.Contains("POP directive failed"));
    }

    [TestMethod]
    public void Execute_MultiplePushPop_WorksInLIFOOrder()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><a><b/></a></root>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        new PushDirective().Execute(cursor);
        new XPathDirective("//a").Execute(cursor);
        new PushDirective().Execute(cursor);
        new XPathDirective("//b").Execute(cursor);
        new PushDirective().Execute(cursor);
        new PopDirective().Execute(cursor);
        var nodes = cursor.Nodes;
        Assert.AreEqual("b", nodes[0].Name);
        new PopDirective().Execute(cursor);
        nodes = cursor.Nodes;
        Assert.AreEqual("a", nodes[0].Name);
        new PopDirective().Execute(cursor);
        nodes = cursor.Nodes;
        Assert.AreEqual("root", nodes[0].Name);
    }

    [TestMethod]
    public void Execute_AfterModifications_RestoresCorrectPosition()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><a/></root>");
        var cursor = new Cursor(doc, [doc.DocumentElement!]);
        new PushDirective().Execute(cursor);
        new XPathDirective("//a").Execute(cursor);
        new AddDirective("child").Execute(cursor);
        new PopDirective().Execute(cursor);
        var nodes = cursor.Nodes;
        Assert.AreEqual("root", nodes[0].Name);
        Assert.IsNotNull(doc.SelectSingleNode("//a/child"));
    }

    [TestMethod]
    public void ToString_ReturnsCorrectFormat()
    {
        var result = new PopDirective().ToString();
        Assert.AreEqual("POP", result);
    }
}
