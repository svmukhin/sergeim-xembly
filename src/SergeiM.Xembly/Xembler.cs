using System.Xml;

namespace SergeiM.Xembly;

/// <summary>
/// Main entry point for applying Xembly directives to XML documents.
/// </summary>
/// <remarks>
/// The Xembler executes a sequence of directives on an XML document,
/// either modifying an existing document or creating a new one.
/// </remarks>
public sealed class Xembler
{
    private readonly Directives _directives;

    /// <summary>
    /// Initializes a new instance of the <see cref="Xembler"/> class.
    /// </summary>
    /// <param name="directives">The directives to execute.</param>
    /// <exception cref="ArgumentNullException">Thrown when directives is null.</exception>
    public Xembler(Directives directives)
    {
        _directives = directives ?? throw new ArgumentNullException(nameof(directives));
    }

    /// <summary>
    /// Applies all directives to an existing XML document.
    /// </summary>
    /// <param name="document">The document to modify.</param>
    /// <exception cref="ArgumentNullException">Thrown when document is null.</exception>
    /// <exception cref="XemblyException">Thrown when directive execution fails.</exception>
    public void Apply(XmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var initialNodes = document.DocumentElement != null
            ? [document.DocumentElement]
            : new XmlNode[] { document };        
        var cursor = new Cursor(document, initialNodes);
        foreach (var directive in _directives)
        {
            directive.Execute(cursor);
        }
    }

    /// <summary>
    /// Generates a new XML document and returns it as a string.
    /// </summary>
    /// <returns>The generated XML as a string.</returns>
    /// <exception cref="XemblyException">Thrown when directive execution fails.</exception>
    public string Xml()
    {
        var document = Document();
        return document.OuterXml;
    }

    /// <summary>
    /// Generates a new XML document.
    /// </summary>
    /// <returns>The generated XML document.</returns>
    /// <exception cref="XemblyException">Thrown when directive execution fails.</exception>
    public XmlDocument Document()
    {
        var document = new XmlDocument();
        Apply(document);
        return document;
    }
}
