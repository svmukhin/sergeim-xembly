// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly.Exceptions;

/// <summary>
/// Base exception for all Xembly-related errors.
/// </summary>
public class XemblyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XemblyException"/> class.
    /// </summary>
    public XemblyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XemblyException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public XemblyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XemblyException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public XemblyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
