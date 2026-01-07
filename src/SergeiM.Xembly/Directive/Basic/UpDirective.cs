// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Basic;

/// <summary>
/// UP directive - moves the cursor to parent nodes.
/// </summary>
/// <remarks>
/// Syntax: UP
/// Navigates from each currently selected node to its parent node.
/// </remarks>
public sealed class UpDirective : IDirective
{
    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot move up: cursor has no current nodes");
        }
        var parents = new List<XmlNode>();
        foreach (var node in cursor.Nodes)
        {
            if (node.ParentNode != null)
            {
                if (!parents.Contains(node.ParentNode))
                {
                    parents.Add(node.ParentNode);
                }
            }
            else
            {
                throw new CursorException($"Cannot move up from node '{node.Name}': no parent node");
            }
        }
        cursor.Set(parents);
    }

    /// <inheritdoc/>
    public override string ToString() => "UP";
}
