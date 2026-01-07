// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class PushPopTests
{
    [TestMethod]
    public void PushPop_BasicUsage_RestoresPosition()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var directives = new Directives()
            .Push()
            .Add("books")
            .Add("book")
            .Attr("isbn", "123")
            .Pop();
        new Xembler(directives).Apply(doc);
        Assert.AreEqual("root", doc.DocumentElement!.Name);
        Assert.IsNotNull(doc.SelectSingleNode("//root/books/book[@isbn='123']"));
    }

    [TestMethod]
    public void PushPop_NestedOperations_WorksCorrectly()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var directives = new Directives()
            .Push()
            .Add("section1")
            .Push()
            .Add("item1")
            .Set("value1")
            .Pop()
            .Add("item2")
            .Set("value2")
            .Pop()
            .Add("section2")
            .Add("item3")
            .Set("value3");
        new Xembler(directives).Apply(doc);
        Assert.IsNotNull(doc.SelectSingleNode("//root/section1/item1[text()='value1']"));
        Assert.IsNotNull(doc.SelectSingleNode("//root/section1/item2[text()='value2']"));
        Assert.IsNotNull(doc.SelectSingleNode("//root/section2/item3[text()='value3']"));
    }

    [TestMethod]
    public void PushPop_WithXPath_NavigatesAndReturns()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><target id='1'/><target id='2'/></root>");
        var directives = new Directives()
            .XPath("//target[@id='1']")
            .Push()
            .Add("child1")
            .Pop()
            .XPath("//target[@id='2']")
            .Add("child2");
        new Xembler(directives).Apply(doc);
        Assert.IsNotNull(doc.SelectSingleNode("//target[@id='1']/child1"));
        Assert.IsNotNull(doc.SelectSingleNode("//target[@id='2']/child2"));
    }

    [TestMethod]
    public void PushPop_MultipleStackLevels_MaintainsLIFO()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var directives = new Directives()
            .Add("level1")
            .Push()
            .Add("level2")
            .Push()
            .Add("level3")
            .Push()
            .Add("level4")
            .Attr("value", "deepest")
            .Pop()
            .Attr("value", "three")
            .Pop()
            .Attr("value", "two")
            .Pop()
            .Attr("value", "one");
        new Xembler(directives).Apply(doc);
        Assert.AreEqual("one", doc.SelectSingleNode("//level1")!.Attributes!["value"]!.Value);
        Assert.AreEqual("two", doc.SelectSingleNode("//level2")!.Attributes!["value"]!.Value);
        Assert.AreEqual("three", doc.SelectSingleNode("//level3")!.Attributes!["value"]!.Value);
        Assert.AreEqual("deepest", doc.SelectSingleNode("//level4")!.Attributes!["value"]!.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void Pop_WithEmptyStack_ThrowsException()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var directives = new Directives()
            .Pop();
        new Xembler(directives).Apply(doc);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void Pop_MoreTimesThanPush_ThrowsException()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var directives = new Directives()
            .Push()
            .Add("item")
            .Pop()
            .Pop();
        new Xembler(directives).Apply(doc);
    }

    [TestMethod]
    public void PushPop_FromScript_ParsesAndExecutes()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var script = @"
            PUSH;
            ADD 'container';
            ADD 'item';
            SET 'test value';
            POP;
            ADD 'sibling';
        ";
        var directives = new Directives(script);
        new Xembler(directives).Apply(doc);
        Assert.IsNotNull(doc.SelectSingleNode("//root/container/item[text()='test value']"));
        Assert.IsNotNull(doc.SelectSingleNode("//root/sibling"));
    }

    [TestMethod]
    public void PushPop_ComplexNestedScript_WorksCorrectly()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<catalog/>");
        var script = @"
            ADD 'books';
            PUSH;
                ADD 'book';
                ATTR 'id', '1';
                PUSH;
                    ADD 'title';
                    SET 'Book One';
                POP;
                PUSH;
                    ADD 'author';
                    SET 'Author One';
                POP;
            POP;
            PUSH;
                ADD 'book';
                ATTR 'id', '2';
                ADD 'title';
                SET 'Book Two';
            POP;
        ";
        var directives = new Directives(script);
        new Xembler(directives).Apply(doc);
        Assert.IsNotNull(doc.SelectSingleNode("//books/book[@id='1']/title[text()='Book One']"));
        Assert.IsNotNull(doc.SelectSingleNode("//books/book[@id='1']/author[text()='Author One']"));
        Assert.IsNotNull(doc.SelectSingleNode("//books/book[@id='2']/title[text()='Book Two']"));
    }

    [TestMethod]
    public void PushPop_WithMultipleNodes_PreservesAllNodes()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root><item/><item/><item/></root>");
        var directives = new Directives()
            .XPath("//item")
            .Push()
            .Add("child")
            .Pop()
            .Attr("processed", "true");
        new Xembler(directives).Apply(doc);
        Assert.AreEqual(3, doc.SelectNodes("//item[@processed='true']")!.Count);
        Assert.AreEqual(3, doc.SelectNodes("//item/child")!.Count);
    }
}
