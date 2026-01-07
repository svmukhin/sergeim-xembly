// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly.Exceptions;

/// <summary>
/// Exception thrown when cursor operations fail.
/// </summary>
public class CursorException : XemblyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CursorException"/> class.
    /// </summary>
    public CursorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CursorException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public CursorException(string message) : base(message)
    {
    }
}
