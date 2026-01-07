// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.XPath;

/// <summary>
/// XSET directive - sets text content using XPath evaluation.
/// </summary>
/// <remarks>
/// Syntax: XSET 'xpath-expression'
/// Evaluates the XPath expression and uses the result as the text content for current nodes.
/// </remarks>
public sealed class XSetDirective : IDirective
{
    private readonly string _expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="XSetDirective"/> class.
    /// </summary>
    /// <param name="expression">The XPath expression to evaluate.</param>
    /// <exception cref="ArgumentNullException">Thrown when expression is null.</exception>
    /// <exception cref="ArgumentException">Thrown when expression is empty or whitespace.</exception>
    public XSetDirective(string expression)
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
            throw new CursorException("Cannot execute XSET: cursor has no current nodes");
        }
        try
        {
            foreach (var node in cursor.Nodes)
            {
                var navigator = node.CreateNavigator() ?? throw new XemblyException($"Cannot create navigator for node '{node.Name}'");
                var result = navigator.Evaluate(_expression);
                string textValue;
                if (result is string str)
                {
                    textValue = str;
                }
                else if (result is double dbl)
                {
                    textValue = dbl.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (result is bool bln)
                {
                    textValue = bln.ToString().ToLowerInvariant();
                }
                else if (result is XPathNodeIterator iterator)
                {
                    if (iterator.MoveNext())
                    {
                        textValue = iterator.Current?.Value ?? string.Empty;
                    }
                    else
                    {
                        textValue = string.Empty;
                    }
                }
                else
                {
                    textValue = result?.ToString() ?? string.Empty;
                }
                node.InnerText = textValue;
            }
        }
        catch (XPathException ex)
        {
            throw new XemblyException($"Invalid XPath expression '{_expression}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"XSET '{_expression}'";
}
