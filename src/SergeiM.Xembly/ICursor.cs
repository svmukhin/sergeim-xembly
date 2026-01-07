// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using System.Xml;

namespace SergeiM.Xembly;

/// <summary>
/// Cursor for navigating and manipulating XML document nodes.
/// </summary>
/// <remarks>
/// The cursor maintains a collection of currently selected XML nodes and provides
/// methods for navigating the document tree and performing operations on those nodes.
/// </remarks>
public interface ICursor
{
    /// <summary>
    /// Gets the collection of currently selected XML nodes.
    /// </summary>
    IReadOnlyList<XmlNode> Nodes { get; }

    /// <summary>
    /// Gets the XML document being manipulated.
    /// </summary>
    XmlDocument Document { get; }

    /// <summary>
    /// Adds a node to the current collection of nodes.
    /// </summary>
    /// <param name="node">The node to add to the cursor.</param>
    void Add(XmlNode node);

    /// <summary>
    /// Replaces the current collection of nodes with a new collection.
    /// </summary>
    /// <param name="nodes">The new collection of nodes.</param>
    void Set(IEnumerable<XmlNode> nodes);

    /// <summary>
    /// Checks if the cursor currently has any nodes selected.
    /// </summary>
    bool HasNodes { get; }

    /// <summary>
    /// Gets the count of currently selected nodes.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Saves the current cursor position to a stack.
    /// </summary>
    void Push();

    /// <summary>
    /// Restores the cursor position from the stack.
    /// </summary>
    void Pop();
}
