using System.Xml;

namespace SergeiM.Xembly;

/// <summary>
/// Implementation of <see cref="ICursor"/> for navigating and manipulating XML nodes.
/// </summary>
public sealed class Cursor : ICursor
{
    private readonly List<XmlNode> _nodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="Cursor"/> class with an XML document.
    /// </summary>
    /// <param name="document">The XML document to manipulate.</param>
    /// <exception cref="ArgumentNullException">Thrown when document is null.</exception>
    public Cursor(XmlDocument document)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        _nodes = [document];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cursor"/> class with specific nodes.
    /// </summary>
    /// <param name="document">The XML document being manipulated.</param>
    /// <param name="nodes">Initial collection of nodes.</param>
    /// <exception cref="ArgumentNullException">Thrown when document or nodes is null.</exception>
    public Cursor(XmlDocument document, IEnumerable<XmlNode> nodes)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        _nodes = [.. nodes ?? throw new ArgumentNullException(nameof(nodes))];
    }

    /// <inheritdoc/>
    public IReadOnlyList<XmlNode> Nodes => _nodes.AsReadOnly();

    /// <inheritdoc/>
    public XmlDocument Document { get; }

    /// <inheritdoc/>
    public bool HasNodes => _nodes.Count > 0;

    /// <inheritdoc/>
    public int Count => _nodes.Count;

    /// <inheritdoc/>
    public void Add(XmlNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        _nodes.Add(node);
    }

    /// <inheritdoc/>
    public void Set(IEnumerable<XmlNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        _nodes.Clear();
        _nodes.AddRange(nodes);
    }
}
