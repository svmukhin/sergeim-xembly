// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class ParserTests
{
    [TestMethod]
    public void TestParseSimpleAdd()
    {
        var xml = new Xembler(new Directives("ADD 'root';")).Xml();
        Assert.IsTrue(xml.Contains("<root"), "Should contain root element");
    }

    [TestMethod]
    public void TestParseMultipleDirectives()
    {
        var script = """
            ADD 'root';
            ADD 'child';
            SET 'value';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<root"), "Should contain root");
        Assert.IsTrue(xml.Contains("<child"), "Should contain child");
        Assert.IsTrue(xml.Contains("value"), "Should contain value");
    }

    [TestMethod]
    public void TestParseAttrDirective()
    {
        var script = """
            ADD 'root';
            ATTR 'id', '123';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("id=\"123\""), "Should contain attribute");
    }

    [TestMethod]
    public void TestParseWithComments()
    {
        var script = """
            # This is a comment
            ADD 'root';
            // Another comment
            SET 'value';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<root"), "Should contain root");
        Assert.IsTrue(xml.Contains("value"), "Should contain value");
    }

    [TestMethod]
    public void TestParseEscapedStrings()
    {
        var xml = new Xembler(new Directives("""ADD 'root'; SET 'line1\nline2';""")).Xml();
        Assert.IsTrue(xml.Contains("line1\nline2"), "Should contain escaped newline");
    }

    [TestMethod]
    public void TestParseWithoutSemicolons()
    {
        var script = """
            ADD 'root'
            SET 'value'
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<root"), "Should contain root");
        Assert.IsTrue(xml.Contains("value"), "Should contain value");
    }

    [TestMethod]
    public void TestParseComplex()
    {
        var script = """
            ADD 'orders';
            ADD 'order';
            ATTR 'id', '553';
            ADD 'amount';
            SET '$140.00';
            UP;
            ATTR 'date', '2014-05-19';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<orders"), "Should contain orders");
        Assert.IsTrue(xml.Contains("<order"), "Should contain order");
        Assert.IsTrue(xml.Contains("id=\"553\""), "Should contain id");
        Assert.IsTrue(xml.Contains("<amount"), "Should contain amount");
        Assert.IsTrue(xml.Contains("$140.00"), "Should contain amount value");
        Assert.IsTrue(xml.Contains("date=\"2014-05-19\""), "Should contain date");
    }

    [TestMethod]
    public void TestParseUpAndRemove()
    {
        var script = """
            ADD 'root';
            ADD 'temp';
            UP;
            ADD 'keep';
            """;
        var directives = new Directives(script);
        var xml = new Xembler(directives).Xml();
        Assert.IsTrue(xml.Contains("<keep"), "Should contain keep");
        Assert.IsTrue(xml.Contains("<temp"), "Should contain temp");
    }

    [TestMethod]
    public void TestParseXPathDirective()
    {
        var script = """
            ADD 'root';
            ADD 'a';
            UP;
            ADD 'b';
            UP;
            XPATH '//a';
            SET 'value';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<a>value</a>"), "Should set value in element 'a'");
    }

    [TestMethod]
    public void TestParseCDataDirective()
    {
        var script = """
            ADD 'root';
            CDATA '<script>alert("test");</script>';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<![CDATA["), "Should contain CDATA");
        Assert.IsTrue(xml.Contains("<script>alert(\"test\");</script>"), "Should contain CDATA content");
    }

    [TestMethod]
    public void TestParsePiDirective()
    {
        var script = """
            ADD 'root';
            PI 'xml-stylesheet', 'type="text/xsl" href="style.xsl"';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<?xml-stylesheet"), "Should contain processing instruction");
    }

    [TestMethod]
    public void TestParseStrictDirective()
    {
        var script = """
            ADD 'root';
            STRICT 1;
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<root"), "Should contain root");
    }

    [TestMethod]
    public void TestParseAddIfDirective()
    {
        var script = """
            ADD 'root';
            ADDIF 'item';
            UP;
            ADDIF 'item';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        var count = System.Text.RegularExpressions.Regex.Matches(xml, "<item").Count;
        Assert.AreEqual(1, count, "Should only add one item");
    }

    [TestMethod]
    public void TestParseXSetDirective()
    {
        var script = """
            ADD 'root';
            ADD 'price';
            SET '100';
            UP;
            ADD 'quantity';
            XSET 'number(//price) * 2';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("<quantity>200</quantity>"), "Should evaluate XPath expression");
    }

    [TestMethod]
    public void TestParseXAttrDirective()
    {
        var script = """
            ADD 'root';
            ADD 'item';
            SET '42';
            XATTR 'value', '.';
            """;
        var xml = new Xembler(new Directives(script)).Xml();
        Assert.IsTrue(xml.Contains("value=\"42\""), "Should set attribute from XPath");
    }

    [TestMethod]
    public void TestParseDoubleQuotes()
    {
        var xml = new Xembler(new Directives("""ADD "root"; SET "value";""")).Xml();
        Assert.IsTrue(xml.Contains("<root"), "Should accept double quotes");
        Assert.IsTrue(xml.Contains("value"), "Should contain value");
    }

    [TestMethod]
    public void TestParseEmptyScript()
    {
        var count = new Directives("  \n\t  ").Count;
        Assert.AreEqual(0, count, "Should parse empty script");
    }

    [TestMethod]
    public void TestParseThrowsOnUnknownDirective()
    {
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives("INVALID 'test';"));
        Assert.AreEqual(1, ex.Line);
        Assert.IsTrue(ex.Column > 0);
    }

    [TestMethod]
    public void TestParseThrowsOnUnterminatedString()
    {
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives("ADD 'unterminated"));
        Assert.AreEqual(1, ex.Line);
        Assert.IsTrue(ex.Column > 0);
    }
}
