// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Collections;
using SergeiM.Xembly.Directive.Advanced;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Directive.XPath;
using SergeiM.Xembly.Exceptions;
using SergeiM.Xembly.Parsing;

namespace SergeiM.Xembly;

/// <summary>
/// Collection of Xembly directives with fluent API for building directive sequences.
/// </summary>
/// <remarks>
/// This class provides both a fluent interface for programmatically building directives
/// and the ability to parse directives from Xembly script strings.
/// </remarks>
public sealed class Directives : IEnumerable<IDirective>
{
    private readonly List<IDirective> _directives;

    /// <summary>
    /// Initializes a new instance of the <see cref="Directives"/> class.
    /// </summary>
    public Directives()
    {
        _directives = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Directives"/> class from Xembly script.
    /// </summary>
    /// <param name="script">The Xembly script to parse.</param>
    /// <exception cref="ArgumentNullException">Thrown when script is null.</exception>
    /// <exception cref="XemblyException">Thrown when script parsing fails.</exception>
    public Directives(string script)
    {
        ArgumentNullException.ThrowIfNull(script);
        _directives = [.. new Parser(script).Parse()];
    }

    /// <summary>
    /// Adds a new child node with the specified name.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    public Directives Add(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _directives.Add(new AddDirective(name));
        return this;
    }

    /// <summary>
    /// Sets the text content of current nodes.
    /// </summary>
    /// <param name="value">The text value to set.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public Directives Set(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _directives.Add(new SetDirective(value));
        return this;
    }

    /// <summary>
    /// Sets an attribute on current nodes.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name or value is null.</exception>
    public Directives Attr(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);
        _directives.Add(new AttrDirective(name, value));
        return this;
    }

    /// <summary>
    /// Moves cursor up to parent nodes.
    /// </summary>
    /// <returns>This instance for method chaining.</returns>
    public Directives Up()
    {
        _directives.Add(new UpDirective());
        return this;
    }

    /// <summary>
    /// Removes all current nodes.
    /// </summary>
    /// <returns>This instance for method chaining.</returns>
    public Directives Remove()
    {
        _directives.Add(new RemoveDirective());
        return this;
    }

    /// <summary>
    /// Saves current cursor position to stack.
    /// </summary>
    /// <returns>This instance for method chaining.</returns>
    public Directives Push()
    {
        _directives.Add(new PushDirective());
        return this;
    }

    /// <summary>
    /// Restores cursor position from stack.
    /// </summary>
    /// <returns>This instance for method chaining.</returns>
    public Directives Pop()
    {
        _directives.Add(new PopDirective());
        return this;
    }

    /// <summary>
    /// Navigates to nodes using XPath expression.
    /// </summary>
    /// <param name="expression">The XPath expression to evaluate.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when expression is null.</exception>
    public Directives XPath(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        _directives.Add(new XPathDirective(expression));
        return this;
    }

    /// <summary>
    /// Sets text content using XPath evaluation.
    /// </summary>
    /// <param name="expression">The XPath expression to evaluate for the text value.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when expression is null.</exception>
    public Directives XSet(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        _directives.Add(new XSetDirective(expression));
        return this;
    }

    /// <summary>
    /// Sets attribute value using XPath evaluation.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="expression">The XPath expression to evaluate for the attribute value.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name or expression is null.</exception>
    public Directives XAttr(string name, string expression)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(expression);
        _directives.Add(new XAttrDirective(name, expression));
        return this;
    }

    /// <summary>
    /// Adds a new child node only if it doesn't already exist.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    public Directives AddIf(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _directives.Add(new AddIfDirective(name));
        return this;
    }

    /// <summary>
    /// Sets CDATA section as node content.
    /// </summary>
    /// <param name="value">The CDATA text value.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public Directives CData(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _directives.Add(new CDataDirective(value));
        return this;
    }

    /// <summary>
    /// Adds a processing instruction.
    /// </summary>
    /// <param name="target">The processing instruction target.</param>
    /// <param name="data">The processing instruction data.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when target or data is null.</exception>
    public Directives Pi(string target, string data)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(data);
        _directives.Add(new PiDirective(target, data));
        return this;
    }

    /// <summary>
    /// Validates that cursor has nodes.
    /// </summary>
    /// <returns>This instance for method chaining.</returns>
    public Directives Strict()
    {
        _directives.Add(new StrictDirective());
        return this;
    }

    /// <summary>
    /// Validates that cursor has exactly the specified number of nodes.
    /// </summary>
    /// <param name="count">The expected number of nodes.</param>
    /// <returns>This instance for method chaining.</returns>
    public Directives Strict(int count)
    {
        _directives.Add(new StrictDirective(count));
        return this;
    }

    /// <summary>
    /// Declares an XML namespace.
    /// </summary>
    /// <param name="prefix">The namespace prefix (empty string for default namespace).</param>
    /// <param name="uri">The namespace URI.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when prefix or uri is null.</exception>
    public Directives Ns(string prefix, string uri)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(uri);
        _directives.Add(new NsDirective(prefix, uri));
        return this;
    }

    /// <summary>
    /// Gets the count of directives in this collection.
    /// </summary>
    public int Count => _directives.Count;

    /// <inheritdoc/>
    public IEnumerator<IDirective> GetEnumerator() => _directives.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
