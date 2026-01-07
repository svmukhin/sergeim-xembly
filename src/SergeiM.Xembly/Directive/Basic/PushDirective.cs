// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Basic;

/// <summary>
/// PUSH directive - saves current cursor position to stack.
/// </summary>
/// <remarks>
/// Syntax: PUSH
/// Saves the current cursor position onto a stack for later restoration with POP.
/// Uses First-In-Last-Out (FILO) stack behavior.
/// </remarks>
public sealed class PushDirective : IDirective
{
    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        cursor.Push();
    }

    /// <inheritdoc/>
    public override string ToString() => "PUSH";
}
