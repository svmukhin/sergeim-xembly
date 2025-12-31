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
