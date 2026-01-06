// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Exceptions;
using System.Xml;

namespace SergeiM.Xembly.Directive.Advanced;

/// <summary>
/// NS directive - declares an XML namespace.
/// </summary>
/// <remarks>
/// This directive adds a namespace declaration to the current nodes.
/// If prefix is empty, it sets the default namespace.
/// </remarks>
public sealed class NsDirective : IDirective
{
    private readonly string _prefix;
    private readonly string _uri;

    /// <summary>
    /// Initializes a new instance of the <see cref="NsDirective"/> class.
    /// </summary>
    /// <param name="prefix">The namespace prefix (empty string for default namespace).</param>
    /// <param name="uri">The namespace URI.</param>
    public NsDirective(string prefix, string uri)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(uri);
        _prefix = prefix;
        _uri = uri;
    }

    /// <inheritdoc/>
    public void Execute(ICursor cursor)
    {
        if (cursor.Nodes.Count == 0)
        {
            throw new CursorException("Cannot set namespace on empty cursor");
        }
        foreach (var node in cursor.Nodes)
        {
            if (string.IsNullOrEmpty(_prefix))
            {
                new AttrDirective("xmlns", _uri).Execute(new Cursor(cursor.Document, [node]));
            }
            else
            {
                new AttrDirective($"xmlns:{_prefix}", _uri).Execute(new Cursor(cursor.Document, [node]));
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrEmpty(_prefix)
        ? $"NS('{_uri}')"
        : $"NS('{_prefix}', '{_uri}')";
}
