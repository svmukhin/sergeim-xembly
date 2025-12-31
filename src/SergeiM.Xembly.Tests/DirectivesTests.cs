using System.Xml;

namespace SergeiM.Xembly.Tests;

[TestClass]
public class DirectivesTests
{
    [TestMethod]
    public void TestFluentApi()
    {
        var directives = new Directives()
            .Add("root")
            .Add("child")
            .Attr("id", "1")
            .Set("value")
            .Up()
            .Remove();
        Assert.AreEqual(6, directives.Count);
    }

    [TestMethod]
    public void TestEnumeration()
    {
        var list = new Directives()
            .Add("first")
            .Add("second").ToList();
        Assert.AreEqual(2, list.Count);
        Assert.IsInstanceOfType(list[0], typeof(AddDirective));
        Assert.IsInstanceOfType(list[1], typeof(AddDirective));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAddNullNameThrows()
    {
        _ = new Directives
        {
            null!
        };
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestSetNullValueThrows()
    {
        new Directives().Set(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAttrNullNameThrows()
    {
        new Directives().Attr(null!, "value");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAttrNullValueThrows()
    {
        new Directives().Attr("name", null!);
    }
}
