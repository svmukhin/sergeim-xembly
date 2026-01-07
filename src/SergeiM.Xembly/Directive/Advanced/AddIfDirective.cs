// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Advanced;

/// <summary>
/// ADDIF directive - adds a new child node only if it doesn't already exist.
/// </summary>
/// <remarks>
/// Syntax: ADDIF 'nodeName'
/// Creates a new element and appends it to each current node only if a child with that name doesn't exist.
/// If the child already exists, the cursor moves to that existing child.
/// </remarks>
public sealed class AddIfDirective : IDirective
{
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddIfDirective"/> class.
    /// </summary>
    /// <param name="name">The name of the node to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when name is null.</exception>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    public AddIfDirective(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Node name cannot be empty or whitespace.", nameof(name));
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
        var resultNodes = new List<XmlNode>();
        foreach (var node in cursor.Nodes)
        {
            XmlNode? existingChild = null;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == _name)
                {
                    existingChild = child;
                    break;
                }
            }
            if (existingChild != null)
            {
                resultNodes.Add(existingChild);
            }
            else
            {
                var element = cursor.Document.CreateElement(_name);
                node.AppendChild(element);
                resultNodes.Add(element);
            }
        }
        cursor.Set(resultNodes);
    }

    /// <inheritdoc/>
    public override string ToString() => $"ADDIF '{_name}'";
}
