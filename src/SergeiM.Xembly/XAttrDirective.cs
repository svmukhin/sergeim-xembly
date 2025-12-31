// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using System.Xml.XPath;

namespace SergeiM.Xembly;

/// <summary>
/// XATTR directive - sets attribute value using XPath evaluation.
/// </summary>
/// <remarks>
/// Syntax: XATTR 'name', 'xpath-expression'
/// Evaluates the XPath expression and uses the result as the attribute value for current nodes.
/// </remarks>
public sealed class XAttrDirective : IDirective
{
    private readonly string _name;
    private readonly string _expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="XAttrDirective"/> class.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="expression">The XPath expression to evaluate.</param>
    /// <exception cref="ArgumentNullException">Thrown when name or expression is null.</exception>
    /// <exception cref="ArgumentException">Thrown when name or expression is empty or whitespace.</exception>
    public XAttrDirective(string name, string expression)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("XPath expression cannot be empty or whitespace.", nameof(expression));
        }
        _name = name;
        _expression = expression;
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot execute XATTR: cursor has no current nodes");
        }
        try
        {
            foreach (var node in cursor.Nodes)
            {
                if (node is not XmlElement element)
                {
                    throw new XemblyException($"Cannot set attribute on node type {node.NodeType}");
                }
                var navigator = node.CreateNavigator() ?? throw new XemblyException($"Cannot create navigator for node '{node.Name}'");
                var result = navigator.Evaluate(_expression);
                string attrValue;
                if (result is string str)
                {
                    attrValue = str;
                }
                else if (result is double dbl)
                {
                    attrValue = dbl.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (result is bool bln)
                {
                    attrValue = bln.ToString().ToLowerInvariant();
                }
                else if (result is XPathNodeIterator iterator)
                {
                    if (iterator.MoveNext())
                    {
                        attrValue = iterator.Current?.Value ?? string.Empty;
                    }
                    else
                    {
                        attrValue = string.Empty;
                    }
                }
                else
                {
                    attrValue = result?.ToString() ?? string.Empty;
                }
                element.SetAttribute(_name, attrValue);
            }
        }
        catch (XPathException ex)
        {
            throw new XemblyException($"Invalid XPath expression '{_expression}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"XATTR '{_name}', '{_expression}'";
}
