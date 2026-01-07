// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Basic;

/// <summary>
/// POP directive - restores cursor position from stack.
/// </summary>
/// <remarks>
/// Syntax: POP
/// Restores the cursor position from the stack that was saved by PUSH.
/// Uses First-In-Last-Out (FILO) stack behavior.
/// </remarks>
public sealed class PopDirective : IDirective
{
    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        try
        {
            cursor.Pop();
        }
        catch (InvalidOperationException ex)
        {
            throw new CursorException($"POP directive failed: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public override string ToString() => "POP";
}
