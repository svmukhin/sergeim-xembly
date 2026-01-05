// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Directive.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests.DirectiveTests;

[TestClass]
public class XAttrDirectiveTests
{
    [TestMethod]
    public void TestSetAttributeFromStringExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("name", "'hello'").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("hello", target!.GetAttribute("name"));
    }

    [TestMethod]
    public void TestSetAttributeFromNumberExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("count", "42").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("42", target!.GetAttribute("count"));
    }

    [TestMethod]
    public void TestSetAttributeFromBooleanExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        var cursor = new Cursor(document, new[] { target! });
        var directive = new XAttrDirective("enabled", "false()");
        directive.Execute(cursor);
        Assert.AreEqual("false", target!.GetAttribute("enabled"));
    }

    [TestMethod]
    public void TestSetAttributeFromNodeValue()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><source>value</source><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("data", "/root/source").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("value", target!.GetAttribute("data"));
    }

    [TestMethod]
    public void TestSetAttributeFromAnotherAttribute()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><source id='123'/><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("ref", "/root/source/@id").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("123", target!.GetAttribute("ref"));
    }

    [TestMethod]
    public void TestSetAttributeWithArithmeticExpression()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("result", "10 * 5").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("50", target!.GetAttribute("result"));
    }

    [TestMethod]
    public void TestSetAttributeWithConcatFunction()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><first>A</first><second>B</second><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("combined", "concat(/root/first, '-', /root/second)").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("A-B", target!.GetAttribute("combined"));
    }

    [TestMethod]
    public void TestUpdateExistingAttribute()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target id='old'/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("id", "'new'").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("new", target!.GetAttribute("id"));
    }

    [TestMethod]
    public void TestSetAttributeOnMultipleNodes()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><item/><item/><item/></root>");
        var items = document.SelectNodes("/root/item")!;
        new XAttrDirective("status", "'active'").Execute(new Cursor(document, items.Cast<XmlNode>()));
        foreach (XmlElement item in items)
        {
            Assert.AreEqual("active", item.GetAttribute("status"));
        }
    }

    [TestMethod]
    public void TestSetAttributeWithEmptyResult()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("empty", "/root/nonexistent").Execute(new Cursor(document, [target!]));
        Assert.AreEqual("", target!.GetAttribute("empty"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullNameThrows()
    {
        _ = new XAttrDirective(null!, "'value'");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyNameThrows()
    {
        _ = new XAttrDirective("", "'value'");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceNameThrows()
    {
        _ = new XAttrDirective("   ", "'value'");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithNullExpressionThrows()
    {
        _ = new XAttrDirective("name", null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithEmptyExpressionThrows()
    {
        _ = new XAttrDirective("name", "");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestConstructorWithWhitespaceExpressionThrows()
    {
        _ = new XAttrDirective("name", "   ");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestExecuteWithNullCursorThrows()
    {
        new XAttrDirective("name", "'value'").Execute(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(CursorException))]
    public void TestExecuteWithEmptyCursorThrows()
    {
        new XAttrDirective("name", "'value'").Execute(new Cursor(new XmlDocument(), []));
    }

    [TestMethod]
    [ExpectedException(typeof(XemblyException))]
    public void TestExecuteOnNonElementNodeThrows()
    {
        var document = new XmlDocument();
        var textNode = document.CreateTextNode("text");
        new XAttrDirective("name", "'value'").Execute(new Cursor(document, [textNode]));
    }

    [TestMethod]
    [ExpectedException(typeof(XemblyException))]
    public void TestInvalidXPathThrows()
    {
        var document = new XmlDocument();
        document.LoadXml("<root><target/></root>");
        var target = document.SelectSingleNode("/root/target") as XmlElement;
        new XAttrDirective("name", "//[invalid").Execute(new Cursor(document, [target!]));
    }

    [TestMethod]
    public void TestToString()
    {
        var str = new XAttrDirective("id", "concat('a', 'b')").ToString();
        Assert.AreEqual("XATTR 'id', 'concat('a', 'b')'", str);
    }
}
