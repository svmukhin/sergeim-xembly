// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.Advanced;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class StrictDirectiveTests
{
    [TestMethod]
    public void TestStrictPassesWithNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new StrictDirective().Execute(new Cursor(document, [root]));
    }

    [TestMethod]
    [ExpectedException(typeof(StrictException))]
    public void TestStrictThrowsWithEmptyCursor()
    {
        new StrictDirective().Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    public void TestStrictWithExactCount()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        var child1 = document.CreateElement("child1");
        var child2 = document.CreateElement("child2");
        document.AppendChild(root);
        root.AppendChild(child1);
        root.AppendChild(child2);
        new StrictDirective(2).Execute(new Cursor(document, [child1, child2]));
    }

    [TestMethod]
    [ExpectedException(typeof(StrictException))]
    public void TestStrictWithWrongCountThrows()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new StrictDirective(2).Execute(new Cursor(document, [root]));
    }

    [TestMethod]
    public void TestStrictWithZeroCountExpected()
    {
        new StrictDirective(0).Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    [ExpectedException(typeof(StrictException))]
    public void TestStrictExpectingZeroButHasNodes()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        new StrictDirective(0).Execute(new Cursor(document, [root]));
    }

    [TestMethod]
    public void TestStrictInWorkflow()
    {
        var xml = new Xembler(new Directives()
            .Add("root")
            .Strict(1)
            .Add("child")
            .Strict()
            .Set("value")).Xml();
        Assert.IsTrue(xml.Contains("<child>value</child>"));
    }

    [TestMethod]
    [ExpectedException(typeof(StrictException))]
    public void TestStrictFailsInWorkflow()
    {
        new Xembler(new Directives()
            .Add("root")
            .XPath("nonexistent")
            .Strict()).Xml();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new StrictDirective().Execute(null!);
    }

    [TestMethod]
    public void TestToStringNoCount()
    {
        var str = new StrictDirective().ToString();
        Assert.AreEqual("STRICT", str);
    }

    [TestMethod]
    public void TestToStringWithCount()
    {
        var str = new StrictDirective(3).ToString();
        Assert.AreEqual("STRICT 3", str);
    }

    [TestMethod]
    public void TestStrictExceptionMessage()
    {
        var document = new XmlDocument();
        var root = document.CreateElement("root");
        document.AppendChild(root);
        try
        {
            new StrictDirective(5).Execute(new Cursor(document, [root]));
            Assert.Fail("Should have thrown StrictException");
        }
        catch (StrictException ex)
        {
            Assert.IsTrue(ex.Message.Contains("Expected 5"));
            Assert.IsTrue(ex.Message.Contains("has 1"));
        }
    }
}
