// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Collections;

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
    public Directives(string script)
    {
        ArgumentNullException.ThrowIfNull(script);
        _directives = [];
        // TODO: Implement parsing in Phase 5
        throw new NotImplementedException("Script parsing will be implemented in Phase 5");
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
    /// Gets the count of directives in this collection.
    /// </summary>
    public int Count => _directives.Count;

    /// <inheritdoc/>
    public IEnumerator<IDirective> GetEnumerator() => _directives.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
