// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class XSetDirectiveTests
{
    [TestMethod]
    public void TestSetFromStringExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("'hello'").Execute(new Cursor(document, [target]));
        Assert.AreEqual("hello", target.InnerText);
    }

    [TestMethod]
    public void TestSetFromNumberExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("42").Execute(new Cursor(document, [target]));
        Assert.AreEqual("42", target.InnerText);
    }

    [TestMethod]
    public void TestSetFromBooleanExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("true()").Execute(new Cursor(document, [target]));
        Assert.AreEqual("true", target.InnerText);
    }

    [TestMethod]
    public void TestSetFromNodeValue()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><source>value from source</source><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("/root/source").Execute(new Cursor(document, [target]));
        Assert.AreEqual("value from source", target.InnerText);
    }

    [TestMethod]
    public void TestSetFromAttributeValue()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><source id='123'/><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("/root/source/@id").Execute(new Cursor(document, [target]));
        Assert.AreEqual("123", target.InnerText);
    }

    [TestMethod]
    public void TestSetWithArithmeticExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("5 + 3").Execute(new Cursor(document, [target]));
        Assert.AreEqual("8", target.InnerText);
    }

    [TestMethod]
    public void TestSetWithConcatFunction()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><first>Hello</first><second>World</second><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("concat(/root/first, ' ', /root/second)").Execute(new Cursor(document, [target]));
        Assert.AreEqual("Hello World", target.InnerText);
    }

    [TestMethod]
    public void TestSetOnMultipleNodes()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><item/><item/><item/></root>");
        var items = document.SelectNodes("/root/item")!;
        new XSetDirective("'same'").Execute(new Cursor(document, items.Cast<XmlNode>()));
        foreach (XmlNode item in items)
        {
            Assert.AreEqual("same", item.InnerText);
        }
    }

    [TestMethod]
    public void TestSetWithEmptyResult()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target>old</target></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("/root/nonexistent").Execute(new Cursor(document, [target]));
        Assert.AreEqual("", target.InnerText);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullExpressionThrows()
    {
        _ = new XSetDirective(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyExpressionThrows()
    {
        _ = new XSetDirective("");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceExpressionThrows()
    {
        _ = new XSetDirective("   ");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new XSetDirective("'value'").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new XSetDirective("'value'").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    [ExpectedException(typeof(XemblyException))]
    public void TestInvalidXPathThrows()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target")!;
        new XSetDirective("//[invalid").Execute(new Cursor(document, [target]));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new XSetDirective("concat('a', 'b')").ToString();
        Assert.AreEqual("XSET 'concat('a', 'b')'", str);
    }
}
