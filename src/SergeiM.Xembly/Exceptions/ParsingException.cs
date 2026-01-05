// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly.Exceptions;

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
