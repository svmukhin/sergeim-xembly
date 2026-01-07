// SPDX-FileCopyrightText: Copyright (c) [2025-2026] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly.Exceptions;

/// <summary>
/// Exception thrown when a STRICT directive validation fails.
/// </summary>
public class StrictException : XemblyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictException"/> class.
    /// </summary>
    public StrictException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public StrictException(string message) : base(message)
    {
    }
}
