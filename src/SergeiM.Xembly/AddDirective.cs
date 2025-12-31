// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly;

/// <summary>
/// ADD directive - adds a new child node to all current nodes.
/// </summary>
/// <remarks>
/// Syntax: ADD 'nodeName'
/// Creates a new element and appends it to each currently selected node.
/// </remarks>
public sealed class AddDirective : IDirective
{
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddDirective"/> class.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    public AddDirective(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Node name cannot be null, empty or whitespace.", nameof(name));
        }
        _name = name;
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot add node: cursor has no current nodes");
        }
        var nodes = new List<XmlNode>();
        foreach (var node in cursor.Nodes)
        {
            var element = cursor.Document.CreateElement(_name);
            node.AppendChild(element);
            nodes.Add(element);
        }
        cursor.Set(nodes);
    }

    /// <inheritdoc/>
    public override string ToString() => $"ADD '{_name}'";
}
