// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using System.Diagnostics;

namespace SergeiM.Xembly.Tests;

/// <summary>
/// Performance and stress tests for the Xembly library.
/// </summary>
[TestClass]
public class PerformanceTests
{
    [TestMethod]
    public void TestLargeDocumentCreation()
    {
        var directives = new Directives().Add("root");
        for (int i = 0; i < 1000; i++)
        {
            directives
                .Add("item")
                .Attr("id", i.ToString())
                .Set($"Value {i}")
                .Up();
        }
        var stopwatch = Stopwatch.StartNew();
        var xml = new Xembler(directives).Xml();
        stopwatch.Stop();
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000,
            $"Should create 1000 elements in under 1 second, took {stopwatch.ElapsedMilliseconds}ms");
        Assert.IsTrue(xml.Contains("item id=\"999\""));
    }

    [TestMethod]
    public void TestManyDirectiveOperations()
    {
        var directives = new Directives().Add("root");
        for (int i = 0; i < 100; i++)
        {
            directives
                .Add("level1")
                .Add("level2")
                .Set("value")
                .Up()
                .Up();
        }
        var stopwatch = Stopwatch.StartNew();
        var xml = new Xembler(directives).Xml();
        stopwatch.Stop();
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var level2Nodes = doc.SelectNodes("//level2");
        Assert.AreEqual(100, level2Nodes?.Count);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 500,
            $"Should handle 100 nested structures quickly, took {stopwatch.ElapsedMilliseconds}ms");
    }

    [TestMethod]
    public void TestXPathPerformance()
    {
        var doc = new XmlDocument();
        var root = doc.CreateElement("root");
        doc.AppendChild(root);
        for (int i = 0; i < 100; i++)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", i.ToString());
            item.InnerText = $"Value {i}";
            root.AppendChild(item);
        }
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            var directives = new Directives()
                .XPath($"//item[@id='{i}']")
                .Attr("processed", "true");

            new Xembler(directives).Apply(doc);
        }
        stopwatch.Stop();
        var processedCount = doc.SelectNodes("//item[@processed='true']")?.Count;
        Assert.AreEqual(100, processedCount);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000,
            $"Should process 100 XPath queries quickly, took {stopwatch.ElapsedMilliseconds}ms");
    }

    [TestMethod]
    public void TestScriptParsingPerformance()
    {
        var scriptBuilder = new System.Text.StringBuilder();
        scriptBuilder.AppendLine("ADD 'root';");
        for (int i = 0; i < 500; i++)
        {
            scriptBuilder.AppendLine($"ADD 'item';");
            scriptBuilder.AppendLine($"ATTR 'id', '{i}';");
            scriptBuilder.AppendLine($"SET 'Value {i}';");
            scriptBuilder.AppendLine("UP;");
        }
        var script = scriptBuilder.ToString();
        var stopwatch = Stopwatch.StartNew();
        var directives = new Directives(script);
        var xml = new Xembler(directives).Xml();
        stopwatch.Stop();
        Assert.IsTrue(xml.Contains("item id=\"499\""));
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 500,
            $"Should parse and execute large script quickly, took {stopwatch.ElapsedMilliseconds}ms");
    }

    [TestMethod]
    public void TestRepeatedDocumentModification()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<root/>");
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            var directives = new Directives()
                .XPath("/root")
                .Add("item")
                .Attr("index", i.ToString())
                .Set($"Item {i}");
            new Xembler(directives).Apply(doc);
        }
        stopwatch.Stop();
        Assert.AreEqual(100, doc.DocumentElement?.ChildNodes.Count);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000,
            $"Should apply 100 modifications quickly, took {stopwatch.ElapsedMilliseconds}ms");
    }

    [TestMethod]
    public void TestDeepNesting()
    {
        var directives = new Directives().Add("root");
        for (int i = 0; i < 20; i++)
        {
            directives.Add($"level{i}");
        }
        directives.Set("deep value");
        var stopwatch = Stopwatch.StartNew();
        var xml = new Xembler(directives).Xml();
        stopwatch.Stop();
        Assert.IsTrue(xml.Contains("<level19>"));
        Assert.IsTrue(xml.Contains("deep value"));
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100,
            $"Should handle deep nesting efficiently, took {stopwatch.ElapsedMilliseconds}ms");
    }

    [TestMethod]
    public void TestMemoryEfficiency()
    {
        var documents = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            var directives = new Directives()
                .Add("doc")
                .Add("content")
                .Set($"Document {i} with some content to use memory");
            documents.Add(new Xembler(directives).Xml());
        }
        Assert.AreEqual(100, documents.Count);
        Assert.IsTrue(documents[99].Contains("Document 99"));
    }

    [TestMethod]
    public void TestComplexXPathCalculations()
    {
        var doc = new XmlDocument();
        var root = doc.CreateElement("data");
        doc.AppendChild(root);
        for (int i = 1; i <= 100; i++)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("value", i.ToString());
            root.AppendChild(item);
        }
        var stopwatch = Stopwatch.StartNew();
        var directives = new Directives()
            .XPath("/data")
            .Add("sum")
            .XSet("sum(//item/@value)")
            .Up()
            .Add("average")
            .XSet("sum(//item/@value) div count(//item)")
            .Up()
            .Add("max")
            .XSet("//item[not(@value < //item/@value)]/@value")
            .Up()
            .Add("count")
            .XSet("count(//item)");
        new Xembler(directives).Apply(doc);
        stopwatch.Stop();
        Assert.AreEqual("5050", doc.SelectSingleNode("//sum")?.InnerText);
        Assert.AreEqual("50.5", doc.SelectSingleNode("//average")?.InnerText);
        Assert.AreEqual("100", doc.SelectSingleNode("//count")?.InnerText);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 200,
            $"Should perform complex calculations efficiently, took {stopwatch.ElapsedMilliseconds}ms");
    }
}
