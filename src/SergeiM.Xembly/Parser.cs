// SPDX-FileCopyrightText: Copyright (c) [2025] [Sergei Mukhin]
// SPDX-License-Identifier: MIT

namespace SergeiM.Xembly;

/// <summary>
/// Parses Xembly script text into directives.
/// </summary>
internal sealed class Parser
{
    private readonly string _script;
    private int _position;
    private readonly List<IDirective> _directives = [];

    public Parser(string script)
    {
        _script = script ?? throw new ArgumentNullException(nameof(script));
        _position = 0;
    }

    public IEnumerable<IDirective> Parse()
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

    private void SkipWhitespaceAndComments()
    {
        while (_position < _script.Length)
        {
            var ch = _script[_position];
            if (char.IsWhiteSpace(ch))
            {
                _position++;
                continue;
            }
            if (ch == '#' || (_position + 1 < _script.Length && ch == '/' && _script[_position + 1] == '/'))
            {
                while (_position < _script.Length && _script[_position] != '\n')
                {
                    _position++;
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
            _ => throw new XemblyException($"Unknown directive: {directiveName}")
        };
        SkipWhitespaceAndComments();
        if (_position < _script.Length && _script[_position] == ';')
        {
            _position++;
        }
        return directive;
    }

    private string ReadToken()
    {
        SkipWhitespaceAndComments();
        var start = _position;
        while (_position < _script.Length && 
               (char.IsLetterOrDigit(_script[_position]) || _script[_position] == '_'))
        {
            _position++;
        }
        return _script[start.._position];
    }

    private string ReadArgument()
    {
        SkipWhitespaceAndComments();        
        if (_position >= _script.Length)
            throw new XemblyException("Expected argument but reached end of script");
        var ch = _script[_position];
        if (ch == '\'' || ch == '"')
        {
            return ReadQuotedString();
        }
        var start = _position;
        while (_position < _script.Length)
        {
            ch = _script[_position];
            if (char.IsWhiteSpace(ch) || ch == ',' || ch == ';')
                break;
            _position++;
        }
        return _script[start.._position];
    }

    private string ReadQuotedString()
    {
        var quote = _script[_position];
        _position++;
        var result = new System.Text.StringBuilder();
        var escaped = false;
        while (_position < _script.Length)
        {
            var ch = _script[_position];
            _position++;
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
        throw new XemblyException($"Unterminated string starting at position {_position}");
    }

    private AttrDirective ParseAttr()
    {
        var name = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && _script[_position] == ',')
        {
            _position++;
        }        
        var value = ReadArgument();
        return new AttrDirective(name, value);
    }

    private XAttrDirective ParseXAttr()
    {
        var name = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && _script[_position] == ',')
        {
            _position++;
        }        
        var expression = ReadArgument();
        return new XAttrDirective(name, expression);
    }

    private PiDirective ParsePi()
    {
        var target = ReadArgument();
        SkipWhitespaceAndComments();
        if (_position < _script.Length && _script[_position] == ',')
        {
            _position++;
        }        
        var data = ReadArgument();
        return new PiDirective(target, data);
    }

    private StrictDirective ParseStrict()
    {
        SkipWhitespaceAndComments();
        if (_position < _script.Length)
        {
            var ch = _script[_position];
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
}
