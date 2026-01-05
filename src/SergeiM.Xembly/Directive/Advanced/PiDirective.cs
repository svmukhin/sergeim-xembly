// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Directive.Advanced;

/// <summary>
/// PI directive - adds a processing instruction.
/// </summary>
/// <remarks>
/// Syntax: PI 'target', 'data'
/// Creates an XML processing instruction and appends it to current nodes.
/// </remarks>
public sealed class PiDirective : IDirective
{
    private readonly string _target;
    private readonly string _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="PiDirective"/> class.
    /// </summary>
    /// <param name="target">The processing instruction target.</param>
    /// <param name="data">The processing instruction data.</param>
    /// <exception cref="ArgumentNullException">Thrown when target or data is null.</exception>
    /// <exception cref="ArgumentException">Thrown when target is empty or whitespace.</exception>
    public PiDirective(string target, string data)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentException("PI target cannot be empty or whitespace.", nameof(target));
        }
        _target = target;
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        ArgumentNullException.ThrowIfNull(cursor);
        if (!cursor.HasNodes)
        {
            throw new CursorException("Cannot add PI: cursor has no current nodes");
        }
        foreach (var node in cursor.Nodes)
        {
            var pi = cursor.Document.CreateProcessingInstruction(_target, _data);
            node.AppendChild(pi);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"PI '{_target}', '{_data}'";
}
