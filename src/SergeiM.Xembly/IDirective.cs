// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly;

/// <summary>
/// Represents a single Xembly directive that can modify an XML document.
/// </summary>
/// <remarks>
/// Each directive performs a specific operation on the XML document through the cursor,
/// such as adding nodes, setting attributes, navigating the tree, etc.
/// </remarks>
public interface IDirective
{
    /// <summary>
    /// Executes this directive, modifying the XML document via the cursor.
    /// </summary>
    /// <param name="cursor">The cursor pointing to current position(s) in the XML document.</param>
    /// <exception cref="XemblyException">Thrown when directive execution fails.</exception>
    void Execute(ICursor cursor);
}
