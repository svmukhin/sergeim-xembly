// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly;

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

/// <summary>
/// Exception thrown when parsing Xembly script fails.
/// </summary>
public class ParsingException : XemblyException
{
    /// <summary>
    /// Gets the line number where the error occurred.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the column number where the error occurred.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="line">The line number where the error occurred.</param>
    /// <param name="column">The column number where the error occurred.</param>
    public ParsingException(string message, int line, int column) : base($"{message} at line {line}, column {column}")
    {
        Line = line;
        Column = column;
    }
}

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
