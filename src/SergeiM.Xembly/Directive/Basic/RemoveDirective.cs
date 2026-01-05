// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Basic;

/// <summary>
/// REMOVE directive - removes all current nodes from the document.
/// </summary>
/// <remarks>
/// Syntax: REMOVE
/// Removes each currently selected node from its parent and clears the cursor.
/// </remarks>
public sealed class RemoveDirective : IDirective
{
    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot remove: cursor has no current nodes");
        }
        foreach (var node in cursor.Nodes.ToList())
        {
            if (node.ParentNode != null)
            {
                node.ParentNode.RemoveChild(node);
            }
            else
            {
                throw new CursorException($"Cannot remove node '{node.Name}': no parent node");
            }
        }
        cursor.Set([]);
    }

    /// <inheritdoc/>
    public override string ToString() => "REMOVE";
}
