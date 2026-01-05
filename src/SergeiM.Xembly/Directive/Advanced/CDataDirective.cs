// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Advanced;

/// <summary>
/// CDATA directive - sets CDATA section as node content.
/// </summary>
/// <remarks>
/// Syntax: CDATA 'text'
/// Creates a CDATA section with the specified text and sets it as the content of current nodes.
/// </remarks>
public sealed class CDataDirective : IDirective
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CDataDirective"/> class.
    /// </summary>
    /// <param name="value">The CDATA text value.</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public CDataDirective(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot set CDATA: cursor has no current nodes");
        }
        foreach (var node in cursor.Nodes)
        {
            node.RemoveAll();
            var cdata = cursor.Document.CreateCDataSection(_value);
            node.AppendChild(cdata);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"CDATA '{_value}'";
}
