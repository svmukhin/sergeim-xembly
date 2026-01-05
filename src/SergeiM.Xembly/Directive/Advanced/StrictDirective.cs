// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Advanced;

/// <summary>
/// STRICT directive - validates that cursor has the expected number of nodes.
/// </summary>
/// <remarks>
/// Syntax: STRICT 'count' or STRICT
/// Throws an exception if the cursor doesn't have exactly the specified number of nodes.
/// If no count is specified, throws if cursor has zero nodes.
/// </remarks>
public sealed class StrictDirective : IDirective
{
    private readonly int _expectedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictDirective"/> class.
    /// </summary>
    /// <param name="expectedCount">The expected number of nodes. Use -1 to only check for non-empty cursor.</param>
    public StrictDirective(int expectedCount = -1)
    {
        _expectedCount = expectedCount;
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (_expectedCount == -1)
        {
            if (!cursor.HasNodes)
            {
                throw new StrictException("Cursor has no nodes");
            }
        }
        else
        {
            if (cursor.Count != _expectedCount)
            {
                throw new StrictException(
                    $"Expected {_expectedCount} node(s), but cursor has {cursor.Count}");
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString() =>
        _expectedCount == -1 ? "STRICT" : $"STRICT {_expectedCount}";
}
