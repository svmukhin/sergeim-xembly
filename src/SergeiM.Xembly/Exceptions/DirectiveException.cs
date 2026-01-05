// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly.Exceptions;

/// <summary>
/// Exception thrown when directive execution fails.
/// </summary>
public class DirectiveException : XemblyException
{
    /// <summary>
    /// Gets the directive that caused the exception.
    /// </summary>
    public string? DirectiveName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DirectiveException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="directiveName">The name of the directive that failed.</param>
    public DirectiveException(string message, string directiveName) : base($"{directiveName}: {message}")
    {
        DirectiveName = directiveName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectiveException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="directiveName">The name of the directive that failed.</param>
    /// <param name="innerException">The inner exception.</param>
    public DirectiveException(string message, string directiveName, Exception innerException)
        : base($"{directiveName}: {message}", innerException)
    {
        DirectiveName = directiveName;
    }
}
