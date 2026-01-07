// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using System.Xml.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.XPath;

/// <summary>
/// XPATH directive - navigates to nodes using XPath expression.
/// </summary>
/// <remarks>
/// Syntax: XPATH 'xpath-expression'
/// Uses XPath to select nodes from the document and sets them as current nodes.
/// Supports both absolute and relative XPath expressions.
/// </remarks>
public sealed class XPathDirective : IDirective
{
    private readonly string _expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="XPathDirective"/> class.
    /// </summary>
    /// <param name="expression">The XPath expression to evaluate.</param>
    /// <exception cref="ArgumentNullException">Thrown when expression is null.</exception>
    /// <exception cref="ArgumentException">Thrown when expression is empty or whitespace.</exception>
    public XPathDirective(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("XPath expression cannot be empty or whitespace.", nameof(expression));
        }
        _expression = expression;
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot execute XPATH: cursor has no current nodes");
        }
        var results = new List<XmlNode>();
        try
        {
            foreach (var node in cursor.Nodes)
            {
                var navigator = node.CreateNavigator() ?? throw new XemblyException($"Cannot create navigator for node '{node.Name}'");
                var nodeIterator = navigator.Select(_expression);
                while (nodeIterator.MoveNext())
                {
                    if (nodeIterator.Current is IHasXmlNode hasXmlNode)
                    {
                        var selectedNode = hasXmlNode.GetNode();
                        if (selectedNode != null && !results.Contains(selectedNode))
                        {
                            results.Add(selectedNode);
                        }
                    }
                }
            }
        }
        catch (XPathException ex)
        {
            throw new XemblyException($"Invalid XPath expression '{_expression}': {ex.Message}", ex);
        }
        cursor.Set(results);
    }

    /// <inheritdoc/>
    public override string ToString() => $"XPATH '{_expression}'";
}
