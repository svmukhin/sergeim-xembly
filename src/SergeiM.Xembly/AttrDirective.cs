// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly;

/// <summary>
/// ATTR directive - sets an attribute on current nodes.
/// </summary>
/// <remarks>
/// Syntax: ATTR 'name', 'value'
/// Sets or updates an attribute with the specified name and value on all currently selected nodes.
/// </remarks>
public sealed class AttrDirective : IDirective
{
    private readonly string _name;
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttrDirective"/> class.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <param name="value">The attribute value.</param>
    /// <exception cref="ArgumentNullException">Thrown when name or value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    public AttrDirective(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(name));
        }
        _name = name;
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot set attribute: cursor has no current nodes");
        }
        foreach (var node in cursor.Nodes)
        {
            if (node is XmlElement element)
            {
                element.SetAttribute(_name, _value);
            }
            else
            {
                throw new XemblyException($"Cannot set attribute on node type {node.NodeType}");
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"ATTR '{_name}', '{_value}'";
}
