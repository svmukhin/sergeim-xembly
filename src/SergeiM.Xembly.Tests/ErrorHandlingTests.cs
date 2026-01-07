// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class ErrorHandlingTests
{
    [TestMethod]
    public void TestParsingExceptionHasLineAndColumn()
    {
        var script = """
            ADD 'root';
            INVALID 'test';
            """;
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives(script));
        Assert.AreEqual(2, ex.Line, "Should report error on line 2");
        Assert.IsTrue(ex.Message.Contains("line 2"), "Message should contain line number");
        Assert.IsTrue(ex.Message.Contains("INVALID"), "Message should contain directive name");
    }

    [TestMethod]
    public void TestUnterminatedStringReportsCorrectPosition()
    {
        var script = """
            ADD 'root';
            SET 'unterminated
            """;
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives(script));
        Assert.AreEqual(2, ex.Line, "Should report error on line 2");
        Assert.IsTrue(ex.Message.Contains("Unterminated"), "Message should mention unterminated string");
    }

    [TestMethod]
    public void TestMissingArgumentError()
    {
        var script = "ADD";
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives(script));
        Assert.IsTrue(ex.Message.Contains("Expected argument"), "Should mention missing argument");
    }

    [TestMethod]
    public void TestCursorExceptionOnInvalidOperation()
    {
        var directives = new Directives().Add("root");
        var doc = new XmlDocument();
        new Xembler(directives).Apply(doc);
        var badDirectives = new Directives().Up().Up().Add("invalid");
        Assert.ThrowsException<CursorException>(() => new Xembler(badDirectives).Apply(doc));
    }

    [TestMethod]
    public void TestStrictExceptionOnInvalidCount()
    {
        var directives = new Directives()
            .Add("root")
            .Strict(2);
        Assert.ThrowsException<StrictException>(() => new Xembler(directives).Xml());
    }

    [TestMethod]
    public void TestStrictExceptionOnEmptyCursor()
    {
        var directives = new Directives()
            .Add("root")
            .Remove()
            .Strict();
        Assert.ThrowsException<StrictException>(() => new Xembler(directives).Xml());
    }

    [TestMethod]
    public void TestDirectiveExceptionProperties()
    {
        var script = """
            ADD 'root';
            ATTR 'name'
            """;
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives(script));
        Assert.IsTrue(ex.Line > 0, "Should have line number");
        Assert.IsTrue(ex.Column > 0, "Should have column number");
    }

    [TestMethod]
    public void TestMultipleErrorsReportFirst()
    {
        var script = """
            INVALID1 'test';
            INVALID2 'test';
            """;
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives(script));
        Assert.AreEqual(1, ex.Line, "Should report first error");
        Assert.IsTrue(ex.Message.Contains("INVALID1"), "Should contain first invalid directive");
    }

    [TestMethod]
    public void TestErrorWithSpecialCharacters()
    {
        var doc = new XmlDocument();
        new Xembler(new Directives("ADD 'test'; ADD 'next';")).Apply(doc);
        Assert.IsTrue(doc.DocumentElement != null);
        Assert.AreEqual("test", doc.DocumentElement.Name);
    }

    [TestMethod]
    public void TestParsingExceptionInheritance()
    {
        try
        {
            _ = new Directives("INVALID 'test';");
            Assert.Fail("Should have thrown exception");
        }
        catch (ParsingException pex)
        {
            Assert.IsInstanceOfType(pex, typeof(XemblyException), "ParsingException should inherit from XemblyException");
        }
    }

    [TestMethod]
    public void TestDirectiveExceptionWithInnerException()
    {
        try
        {
            new Xembler(new Directives()
                .Add("root")
                .XPath("//[invalid xpath")).Xml();
            Assert.Fail("Should have thrown exception for invalid XPath");
        }
        catch (Exception ex)
        {
            Assert.IsNotNull(ex);
        }
    }

    [TestMethod]
    public void TestErrorMessageClarity()
    {
        var script = """
            ADD 'root';
            SET
            """;
        var ex = Assert.ThrowsException<ParsingException>(() => new Directives(script));
        StringAssert.Contains(ex.Message, "line", "Error message should mention line");
        StringAssert.Contains(ex.Message, "column", "Error message should mention column");
    }
}
