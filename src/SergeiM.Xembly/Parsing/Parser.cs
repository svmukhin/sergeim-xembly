// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

using SergeiM.Xembly.Directive.Advanced;
using SergeiM.Xembly.Directive.Basic;
using SergeiM.Xembly.Directive.XPath;
using SergeiM.Xembly.Exceptions;

namespace SergeiM.Xembly.Parsing;

/// <summary>
/// Parses Xembly script text into directives.
/// </summary>
internal sealed class Parser
{
    private readonly string _script;
    private int _position;
    private int _line = 1;
    private int _column = 1;
    private readonly List<IDirective> _directives = [];

    public Parser(string script)
    {
        _script = script ?? throw new ArgumentNullException(nameof(script));
        _position = 0;
    }

    public IEnumerable<IDirective> Parse()
    {
        try
        {
            while (_position < _script.Length)
            {
                SkipWhitespaceAndComments();
                if (_position >= _script.Length)
                    break;
                var directive = ParseDirective();
                if (directive != null)
                {
                    _directives.Add(directive);
                }
            }
            return _directives;
        }
        catch (ParsingException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ParsingException($"Parsing failed: {ex.Message}", _line, _column);
        }
    }

    private void Advance()
    {
        if (_position < _script.Length)
        {
            if (_script[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            _position++;
        }
    }

    private char CurrentChar => _position < _script.Length ? _script[_position] : '\0';

    private void SkipWhitespaceAndComments()
    {
        while (_position < _script.Length)
        {
            var ch = CurrentChar;
            if (char.IsWhiteSpace(ch))
            {
                Advance();
                continue;
            }
            if (ch == '#' || (_position + 1 < _script.Length && ch == '/' && _script[_position + 1] == '/'))
            {
                while (_position < _script.Length && CurrentChar != '\n')
                {
                    Advance();
                }
                continue;
            }
            break;
        }
    }

    private IDirective? ParseDirective()
    {
        var directiveName = ReadToken();
        if (string.IsNullOrEmpty(directiveName))
            return null;
        try
        {
            IDirective? directive = directiveName.ToUpperInvariant() switch
            {
                "ADD" => new AddDirective(ReadArgument()),
                "ADDIF" => new AddIfDirective(ReadArgument()),
                "SET" => new SetDirective(ReadArgument()),
                "ATTR" => ParseAttr(),
                "XATTR" => ParseXAttr(),
                "XSET" => new XSetDirective(ReadArgument()),
                "XPATH" => new XPathDirective(ReadArgument()),
                "UP" => new UpDirective(),
                "REMOVE" => new RemoveDirective(),
                "CDATA" => new CDataDirective(ReadArgument()),
                "PI" => ParsePi(),
                "STRICT" => ParseStrict(),
                "NS" => ParseNs(),
                _ => throw new ParsingException($"Unknown directive: {directiveName}", _line, _column)
            };
            SkipWhitespaceAndComments();
            if (_position < _script.Length && CurrentChar == ';')
            {
                Advance();
            }
            return directive;
        }
        catch (ParsingException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ParsingException($"Failed to parse {directiveName} directive: {ex.Message}", _line, _column);
        }
    }

    private string ReadToken()
    {
        SkipWhitespaceAndComments();
        var start = _position;
        while (_position < _script.Length &&
               (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_'))
        {
            Advance();
        }
        return _script[start.._position];
    }

    private string ReadArgument()
    {
        SkipWhitespaceAndComments();
        if (_position >= _script.Length)
            throw new ParsingException("Expected argument but reached end of script", _line, _column);
        var ch = CurrentChar;
        if (ch == '\'' || ch == '"')
        {
            return ReadQuotedString();
        }
        var start = _position;
        while (_position < _script.Length)
        {
            ch = CurrentChar;
            if (char.IsWhiteSpace(ch) || ch == ',' || ch == ';')
                break;
            Advance();
        }
        return _script[start.._position];
    }

    private string ReadQuotedString()
    {
        var quote = CurrentChar;
        var startLine = _line;
        var startColumn = _column;
        Advance();
        var result = new System.Text.StringBuilder();
        var escaped = false;
        while (_position < _script.Length)
        {
            var ch = CurrentChar;
            Advance();
            if (escaped)
            {
                result.Append(ch switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\\' => '\\',
                    '\'' => '\'',
                    '"' => '"',
                    _ => ch
                });
                escaped = false;
            }
            else if (ch == '\\')
            {
                escaped = true;
            }
            else if (ch == quote)
            {
                return result.ToString();
            }
            else
            {
                result.Append(ch);
            }
        }
        throw new ParsingException($"Unterminated string", startLine, startColumn);
    }

    private AttrDirective ParseAttr()
    {
        var name = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && CurrentChar == ',')
        {
            Advance();
        }
        var value = ReadArgument();
        return new AttrDirective(name, value);
    }

    private XAttrDirective ParseXAttr()
    {
        var name = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && CurrentChar == ',')
        {
            Advance();
        }
        var expression = ReadArgument();
        return new XAttrDirective(name, expression);
    }

    private PiDirective ParsePi()
    {
        var target = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && CurrentChar == ',')
        {
            Advance();
        }
        var data = ReadArgument();
        return new PiDirective(target, data);
    }

    private StrictDirective ParseStrict()
    {
        SkipWhitespaceAndComments();
        if (_position < _script.Length)
        {
            var ch = CurrentChar;
            if (char.IsDigit(ch))
            {
                var countStr = ReadArgument();
                if (int.TryParse(countStr, out var count))
                {
                    return new StrictDirective(count);
                }
            }
            else if (ch == ';' || char.IsWhiteSpace(ch))
            {
                return new StrictDirective();
            }
        }
        return new StrictDirective();
    }

    private NsDirective ParseNs()
    {
        var firstArg = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && CurrentChar == ',')
        {
            Advance();
            var uri = ReadArgument();
            return new NsDirective(firstArg, uri);
        }
        return new NsDirective("", firstArg);
    }
}
