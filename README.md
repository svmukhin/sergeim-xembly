# SergeiM.Xembly - XML Modifying Imperative Language for .NET

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/svmukhin/sergeim-xembly/build.yml)
![NuGet](https://img.shields.io/nuget/v/SergeiM.Xembly?color=%230000FF)
[![Hits-of-Code](https://hitsofcode.com/github/svmukhin/sergeim-xembly)](https://hitsofcode.com/github/svmukhin/sergeim-xembly/view)
![GitHub License](https://img.shields.io/github/license/svmukhin/sergeim-xembly)

**Xembly** is an Assembly-like imperative programming language for data
manipulation in XML documents. It is a much simpler alternative to DOM,
XSLT, and XQuery.

This is a .NET (C#) implementation of the original Java library by [Yegor Bugayenko](https://github.com/yegor256/xembly).

## Quick Start

### Installation

```bash
dotnet add package SergeiM.Xembly
```

### Basic Usage

```csharp
using SergeiM.Xembly;

// Create directives
var directives = new Directives()
    .Add("root")
    .Add("order")
    .Attr("id", "553")
    .Set("$140.00");

// Generate XML
var xembler = new Xembler(directives);
string xml = xembler.Xml();

// Output: <root><order id="553">$140.00</order></root>
```

## Why Xembly?

Suppose you have an XML document:

```xml
<orders>
  <order id="553">
    <amount>$45.00</amount>
  </order>
</orders>
```

You want to change the amount of order #553 from `$45.00` to `$140.00`.
With Xembly, you write:

```csharp
var directives = new Directives()
    .Add("orders")
    .Add("order")
    .Attr("id", "553")
    .Add("amount")
    .Set("$140.00");
```

Much simpler than DOM manipulation or XSLT transformations!

## Supported Directives

| Directive | Description                   | Example                                    |
| --------- | ----------------------------- | ------------------------------------------ |
| `ADD`     | Adds new child node           | `.Add("order")`                            |
| `ADDIF`   | Adds child node if not exists | `.AddIf("order")`                          |
| `SET`     | Sets text content             | `.Set("$140.00")`                          |
| `ATTR`    | Sets attribute                | `.Attr("id", "553")`                       |
| `UP`      | Moves to parent               | `.Up()`                                    |
| `REMOVE`  | Removes nodes                 | `.Remove()`                                |
| `PUSH`    | Saves cursor position         | `.Push()`                                  |
| `POP`     | Restores cursor position      | `.Pop()`                                   |
| `XPATH`   | Navigate using XPath          | `.XPath("//order[@id='553']")`             |
| `XSET`    | Set text from XPath           | `.XSet("count(//order)")`                  |
| `XATTR`   | Set attribute from XPath      | `.XAttr("total", "sum(//price)")`          |
| `CDATA`   | Add CDATA section             | `.CData("<script>...</script>")`           |
| `PI`      | Add processing instruction    | `.Pi("xml-stylesheet", "type='text/xsl'")` |
| `STRICT`  | Validate cursor state         | `.Strict(1)`                               |
| `NS`      | Define XML namespace          | `.Ns("ns", "http://example.com/ns")`      |

## More Examples

### Using Script Syntax

```csharp
var directives = new Directives("""
    ADD 'orders';
    ADD 'order';
    ATTR 'id', '553';
    ADD 'amount';
    SET '$140.00';
    """);

var xml = new Xembler(directives).Xml();
// Output: <orders><order id="553"><amount>$140.00</amount></order></orders>
```

### XPath Navigation

```csharp
var directives = new Directives()
    .Add("root")
    .Add("order").Attr("id", "1").Set("100").Up()
    .Add("order").Attr("id", "2").Set("200").Up()
    .XPath("//order[@id='2']")
    .Set("250");

var xml = new Xembler(directives).Xml();
// Changes the value of order with id='2' to 250
```

### Multiple Nodes

```csharp
var directives = new Directives()
    .Add("root")
    .Add("item")
    .Set("First")
    .Up()
    .Add("item")
    .Set("Second");

var xml = new Xembler(directives).Xml();
// Output: <root><item>First</item><item>Second</item></root>
```

### Stack Operations (PUSH/POP)

Save and restore cursor position using stack operations:

```csharp
var directives = new Directives()
    .Add("catalog")
    .Add("books")
    .Push()                      // Save position at <books>
    .Add("book").Attr("id", "1")
    .Add("title").Set("First Book").Up().Up()
    .Pop()                       // Return to <books>
    .Add("book").Attr("id", "2")
    .Add("title").Set("Second Book");

var xml = new Xembler(directives).Xml();
// Both books are added under <books>, even though we navigated deep into the first book
```

### Error Handling

```csharp
try
{
    var directives = new Directives("INVALID 'test';");
    new Xembler(directives).Xml();
}
catch (ParsingException ex)
{
    Console.WriteLine($"Parse error at line {ex.Line}, column {ex.Column}: {ex.Message}");
}

try
{
    var directives = new Directives()
        .Add("root")
        .Strict(2); // Expecting 2 nodes, but only have 1
    new Xembler(directives).Xml();
}
catch (StrictException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
```

## Exception Types

| Exception            | Description                                 | Usage                              |
| -------------------- | ------------------------------------------- | ---------------------------------- |
| `XemblyException`    | Base exception for all Xembly errors        | Catch-all for any Xembly operation |
| `ParsingException`   | Script parsing errors with line/column info | Invalid script syntax              |
| `CursorException`    | Cursor operation errors                     | Invalid navigation, empty cursor   |
| `StrictException`    | Validation failures                         | STRICT directive violations        |

### Modifying Existing Documents

```csharp
var document = new XmlDocument();
document.LoadXml("<root><existing>node</existing></root>");

var directives = new Directives()
    .Add("new")
    .Set("value");

new Xembler(directives).Apply(document);
// Document now has: <root><existing>node</existing><new>value</new></root>
```

## Building from Source

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## References

- Original Java implementation: [xembly](https://github.com/yegor256/xembly)
- Blog post: [Xembly, an Assembly for XML](https://www.yegor256.com/2014/04/09/xembly-intro.html)

## License

MIT License - see [LICENSE.txt](LICENSE.txt) for details.
