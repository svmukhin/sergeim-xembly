// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly.Tests;

/// <summary>
/// Integration tests for complex real-world scenarios.
/// </summary>
[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public void TestBuildOrderDocument()
    {
        var directives = new Directives()
            .Add("orders")
            .Add("order")
            .Attr("id", "553")
            .Attr("date", "2014-05-19")
            .Add("customer")
            .Set("John Doe")
            .Up()
            .Add("amount")
            .Set("$140.00")
            .Up()
            .Add("items")
            .Add("item")
            .Attr("sku", "ABC123")
            .Set("Widget")
            .Up()
            .Add("item")
            .Attr("sku", "XYZ789")
            .Set("Gadget")
            .Up();
        var xml = new Xembler(directives).Xml();
        Assert.IsTrue(xml.Contains("<orders>"));
        Assert.IsTrue(xml.Contains("id=\"553\""));
        Assert.IsTrue(xml.Contains("date=\"2014-05-19\""));
        Assert.IsTrue(xml.Contains("<customer>John Doe</customer>"));
        Assert.IsTrue(xml.Contains("<amount>$140.00</amount>"));
        Assert.IsTrue(xml.Contains("sku=\"ABC123\""));
        Assert.IsTrue(xml.Contains("sku=\"XYZ789\""));
    }

    [TestMethod]
    public void TestModifyExistingDocument()
    {
        var doc = new XmlDocument();
        doc.LoadXml(@"
            <catalog>
                <book id='1'>
                    <title>Book One</title>
                    <price>29.99</price>
                </book>
                <book id='2'>
                    <title>Book Two</title>
                    <price>39.99</price>
                </book>
            </catalog>");
        var directives = new Directives()
            .XPath("//book[@id='1']/price")
            .Set("24.99")
            .XPath("//book[@id='2']/price")
            .Set("34.99");
        new Xembler(directives).Apply(doc);
        var book1Price = doc.SelectSingleNode("//book[@id='1']/price")?.InnerText;
        var book2Price = doc.SelectSingleNode("//book[@id='2']/price")?.InnerText;
        Assert.AreEqual("24.99", book1Price);
        Assert.AreEqual("34.99", book2Price);
    }

    [TestMethod]
    public void TestComplexXPathTransformation()
    {
        var doc = new XmlDocument();
        doc.LoadXml(@"
            <data>
                <item value='10'/>
                <item value='20'/>
                <item value='30'/>
            </data>");
        var directives = new Directives()
            .XPath("/data")
            .Add("summary")
            .XSet("sum(//item/@value)")
            .Up()
            .XPath("//item[@value='20']")
            .Attr("selected", "true");
        new Xembler(directives).Apply(doc);
        var summary = doc.SelectSingleNode("//summary")?.InnerText;
        Assert.AreEqual("60", summary, "Should calculate sum of all item values");
        var selectedItem = doc.SelectSingleNode("//item[@selected='true']");
        Assert.IsNotNull(selectedItem);
        Assert.AreEqual("20", selectedItem.Attributes?["value"]?.Value);
    }

    [TestMethod]
    public void TestScriptBasedTransformation()
    {
        var script = """
            ADD 'library';
            ADD 'book';
            ATTR 'isbn', '978-0-123456-78-9';
            ADD 'title';
            SET 'The Great Novel';
            UP;
            ADD 'author';
            SET 'Jane Smith';
            UP;
            ADD 'year';
            SET '2024';
            UP;
            UP;
            ADD 'book';
            ATTR 'isbn', '978-0-987654-32-1';
            ADD 'title';
            SET 'Another Story';
            UP;
            ADD 'author';
            SET 'John Doe';
            UP;
            ADD 'year';
            SET '2025';
            """;
        var directives = new Directives(script);
        var xml = new Xembler(directives).Xml();
        Assert.IsTrue(xml.Contains("<library>"));
        Assert.IsTrue(xml.Contains("isbn=\"978-0-123456-78-9\""));
        Assert.IsTrue(xml.Contains("<title>The Great Novel</title>"));
        Assert.IsTrue(xml.Contains("<author>Jane Smith</author>"));
        Assert.IsTrue(xml.Contains("isbn=\"978-0-987654-32-1\""));
    }

    [TestMethod]
    public void TestConditionalOperations()
    {
        var directives = new Directives()
            .Add("config")
            .AddIf("setting")
            .Set("value1")
            .Up()
            .AddIf("setting") // Should not add duplicate, stays on parent
            .Up()
            .AddIf("other")
            .Set("value3");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var settingNodes = doc.SelectNodes("//setting");
        Assert.AreEqual(1, settingNodes?.Count, "Should only have one 'setting' node");
        Assert.AreEqual("value1", settingNodes?[0]?.InnerText, "Should keep first value");
    }

    [TestMethod]
    public void TestComplexNavigationPattern()
    {
        var directives = new Directives()
            .Add("root")
            .Add("section")
            .Attr("id", "A")
            .Add("item")
            .Set("A1")
            .Up()
            .Up()
            .Add("section")
            .Attr("id", "B")
            .Add("item")
            .Set("B1")
            .Up()
            .Up()
            .XPath("//section[@id='A']")
            .Add("item")
            .Set("A2");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var sectionA = doc.SelectSingleNode("//section[@id='A']");
        Assert.IsNotNull(sectionA);
        var itemsInA = sectionA?.SelectNodes("item");
        Assert.AreEqual(2, itemsInA?.Count, "Section A should have 2 items");
    }

    [TestMethod]
    public void TestMixedContentTypes()
    {
        var directives = new Directives()
            .Add("document")
            .Pi("xml-stylesheet", "type='text/xsl' href='style.xsl'")
            .Add("script")
            .CData("function() { return 'test'; }")
            .Up()
            .Add("content")
            .Set("Regular text");
        var xml = new Xembler(directives).Xml();
        Assert.IsTrue(xml.Contains("<?xml-stylesheet"));
        Assert.IsTrue(xml.Contains("<![CDATA["));
        Assert.IsTrue(xml.Contains("function() { return 'test'; }"));
        Assert.IsTrue(xml.Contains("<content>Regular text</content>"));
    }

    [TestMethod]
    public void TestDynamicXPathCalculations()
    {
        var doc = new XmlDocument();
        doc.LoadXml(@"
            <store>
                <product price='10.00' quantity='5'/>
                <product price='20.00' quantity='3'/>
                <product price='15.00' quantity='4'/>
            </store>");
        var directives = new Directives()
            .XPath("/store")
            .Add("total-items")
            .XSet("sum(//product/@quantity)")
            .Up()
            .Add("average-price")
            .XSet("sum(//product/@price) div count(//product)");
        new Xembler(directives).Apply(doc);
        var totalItems = doc.SelectSingleNode("//total-items")?.InnerText;
        var avgPrice = doc.SelectSingleNode("//average-price")?.InnerText;
        Assert.AreEqual("12", totalItems);
        Assert.AreEqual("15", avgPrice);
    }

    [TestMethod]
    public void TestRemoveAndReplace()
    {
        var doc = new XmlDocument();
        doc.LoadXml(@"
            <data>
                <old>obsolete</old>
                <keep>important</keep>
            </data>");
        var removeDirectives = new Directives()
            .XPath("//old")
            .Remove();
        new Xembler(removeDirectives).Apply(doc);
        var addDirectives = new Directives()
            .XPath("/data")
            .Add("new")
            .Set("updated");
        new Xembler(addDirectives).Apply(doc);
        Assert.IsNull(doc.SelectSingleNode("//old"));
        Assert.IsNotNull(doc.SelectSingleNode("//keep"));
        Assert.AreEqual("updated", doc.SelectSingleNode("//new")?.InnerText);
    }

    [TestMethod]
    public void TestMultipleDocumentModifications()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var step1 = new Directives()
            .XPath("/root")
            .Add("step1")
            .Set("first");
        var step2 = new Directives()
            .XPath("/root")
            .Add("step2")
            .Set("second");
        var step3 = new Directives()
            .XPath("/root")
            .Add("step3")
            .Set("third");
        new Xembler(step1).Apply(doc);
        new Xembler(step2).Apply(doc);
        new Xembler(step3).Apply(doc);
        Assert.IsNotNull(doc.SelectSingleNode("//step1"));
        Assert.IsNotNull(doc.SelectSingleNode("//step2"));
        Assert.IsNotNull(doc.SelectSingleNode("//step3"));
        Assert.AreEqual(3, doc.DocumentElement?.ChildNodes.Count);
    }

    [TestMethod]
    public void TestStrictValidationInComplexScenario()
    {
        var directives = new Directives()
            .Add("root")
            .Add("item")
            .Up()
            .Add("item")
            .Up()
            .Add("item")
            .Up()
            .XPath("//item")
            .Strict(3)
            .Attr("validated", "true");
        var xml = new Xembler(directives).Xml();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var validatedItems = doc.SelectNodes("//item[@validated='true']");
        Assert.AreEqual(3, validatedItems?.Count);
    }

    [TestMethod]
    public void TestXAttrWithCalculation()
    {
        var doc = new XmlDocument();
        doc.LoadXml(@"
            <data>
                <record value='42'/>
            </data>");
        var directives = new Directives()
            .XPath("//record")
            .XAttr("doubled", "number(@value) * 2")
            .XAttr("original", "@value");
        new Xembler(directives).Apply(doc);
        var record = doc.SelectSingleNode("//record");
        Assert.AreEqual("84", record?.Attributes?["doubled"]?.Value);
        Assert.AreEqual("42", record?.Attributes?["original"]?.Value);
    }
}
