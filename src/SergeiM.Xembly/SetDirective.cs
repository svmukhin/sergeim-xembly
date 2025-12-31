// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly;

/// <summary>
/// SET directive - sets the text content of current nodes.
/// </summary>
/// <remarks>
/// Syntax: SET 'value'
/// Sets the InnerText of all currently selected nodes to the specified value.
/// </remarks>
public sealed class SetDirective : IDirective
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetDirective"/> class.
    /// </summary>
    /// <param name="value">The text value to set.</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public SetDirective(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot set text: cursor has no current nodes");
        }
        foreach (var node in cursor.Nodes)
        {
            node.InnerText = _value;
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"SET '{_value}'";
}
