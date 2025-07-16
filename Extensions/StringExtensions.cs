using System;
using System.Collections.Generic;
using System.Text;

public static class StringExtensions
{
    private const int NUMBER_OF_PREDEFINED_FORMATS = 11;

    public static readonly string[] StringFormatsForNumericValuesWithConstantLength = new string[] { "", "0", "00", "000", "0000", "00000", "000000", "0000000", "00000000", "000000000", "0000000000" };
    public static readonly string[] StringFormatsForNumericValuesWithDecimalPlaces = new string[] { "0", "0.0", "0.00", "0.000", "0.0000", "0.00000", "0.000000", "0.0000000", "0.00000000", "0.000000000", "0.0000000000" };

    public static string GetFormat(int _decimalPlaces)
    {
        if (_decimalPlaces.IsInRange(0, NUMBER_OF_PREDEFINED_FORMATS - 1))
        {
            return StringFormatsForNumericValuesWithDecimalPlaces[_decimalPlaces];
        }
        else
        {
            return CreateFormat(_decimalPlaces);
        }
    }

    public static string CreateFormat(int _decimalPlaces)
    {
        string _textFormat = "0";

        if (_decimalPlaces > 0)
        {
            _textFormat += ".";

            for (int i = 0; i < _decimalPlaces; i++)
            {
                _textFormat += "0";
            }
        }

        return _textFormat;
    }

    public static string ToStringWithSign(this int _value)
    {
        return _value > 0
                ? "+" + _value.ToString()
                : _value.ToString();
    }

    public static string ToStringWithSign(this float _value)
    {
        return _value > 0f
                ? "+" + _value.ToString()
                : _value.ToString();
    }

    public static string ToStringWithSign(this float _value, int _decimalPlaces)
    {
        return _value > 0
                ? "+" + _value.ToString(GetFormat(_decimalPlaces))
                : _value.ToString(GetFormat(_decimalPlaces));
    }

    public static string ToStringWithDecimalPlaces(this float _value, int _decimalPlaces)
    {
        return _value.ToString(GetFormat(_decimalPlaces));
    }

    public static string GetConstLengthFormat(int _length)
    {
        if (_length.IsInRange(0, NUMBER_OF_PREDEFINED_FORMATS))
        {
            return StringFormatsForNumericValuesWithConstantLength[_length];
        }
        else
        {
            return CreateConstLengthFormat(_length);
        }
    }

    public static bool IsNullEmptyOrWhitespace(this string _text)
    {
        return _text.IsNullOrEmpty() || _text.IsNullOrWhitespace();
    }

    public static bool IsNullOrWhitespace(this string _text)
    {
        return string.IsNullOrWhiteSpace(_text);
    }

    public static bool IsNullOrEmpty(this string _text)
    {
        return string.IsNullOrEmpty(_text);
    }

    public static string CreateConstLengthFormat(int _length)
    {
        string _format = "";

        for (int i = 0; i < _length; i++)
        {
            _format += "0";
        }

        return _format;
    }

    public static string ToStringWithConstLength(this int _value, int _length = 2)
    {
        return _value.ToString(GetConstLengthFormat(_length));
    }

    public static string ToStringWithConstLengthAndSign(this int _value, int _length = 2)
    {
        return _value > 0
                ? "+" + _value.ToStringWithConstLength(_length)
                : _value.ToStringWithConstLength(_length);
    }

    public static string ToStringWithConstLength(this float _value, int _length = 2)
    {
        return _value.ToString(GetConstLengthFormat(_length));
    }

    public static string InsertSpacesBetweenCharacters(this string _text)
    {
        int _charactersCount = _text.Length;

        if (_charactersCount <= 1)
        {
            return _text;
        }

        int _newLength = (2 * _charactersCount) - 1;
        char[] _newText = new char[_newLength];

        for (int i = 0; i < _charactersCount; i++)
        {
            _newText[2 * i] = _text[i];

            int _spaceIndex = (2 * i + 1);

            if (_spaceIndex < _newLength)
            {
                _newText[_spaceIndex] = ' ';
            }
        }

        return new string(_newText);
    }

    public static string WithRoundBrackets(this string _text)
    {
        return "(" + _text + ")";
    }

    public static string WithSquareBrackets(this string _text)
    {
        return "[" + _text + "]";
    }

    public static string WithCurlyBrackets(this string _text)
    {
        return "{" + _text + "}";
    }

    public static string WithAngleBrackets(this string _text)
    {
        return "<" + _text + ">";
    }

    public static string ReplaceCommasWithDots(this string _text)
    {
        return _text.Replace(',', '.');
    }

    public static string RemoveSpaces(this string _text)
    {
        return _text.Remove(" ");
    }

    public static string RemoveCommas(this string _text)
    {
        return _text.Remove(",");
    }

    public static string RemoveDots(this string _text)
    {
        return _text.Remove(".");
    }

    public static string Remove(this string _text, string _textToRemove)
    {
        return _text.Replace(_textToRemove, "");
    }

    public static void ToUpper(this string[] _array)
    {
        if (_array == null)
        {
            return;
        }

        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            _array[i] = _array[i].ToUpper();
        }
    }

    public static void ToLower(this string[] _array)
    {
        if (_array == null)
        {
            return;
        }

        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            _array[i] = _array[i].ToLower();
        }
    }

    public static void RemoveWhiteSpacesFromTheEnd(this string[] _array)
    {
        if (_array == null)
        {
            return;
        }

        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            _array[i] = _array[i].TrimEnd();
        }
    }

    public static string GetCharCodes(this string _text)
    {
        string _code = "";
        int _length = _text.Length;

        for (int i = 0; i < _length; i++)
        {
            _code += _text[i] + ((int)_text[i]).ToString().WithRoundBrackets();
        }

        return _code;
    }

    public static string GetCharMask(this char _character, int _length)
    {
        StringBuilder _builder = new StringBuilder(_length);

        for (int i = 0; i < _length; i++)
        {
            _builder.Append(_character);
        }

        return _builder.ToString();
    }

    public static string InsertSpaceBeforeUpperCase(this string _text)
    {
        return _text.InsertBeforeUpperCase(' ');
    }

    public static string InsertBeforeUpperCase(this string _text, char _toInsert)
    {
        return _text.InsertBeforeUpperCase(_toInsert.ToString());
    }

    public static string InsertBeforeUpperCase(this string _text, string _toInsert)
    {
        if (_text.IsNullOrWhitespace())
        {
            return _text;
        }

        int _textLength = _text.Length;

        if (_textLength <= 1)
        {
            return _text;
        }

        StringBuilder _string = new StringBuilder();
        _string.Append(_text[0]);

        for (int i = 1; i < _textLength; i++)
        {
            if (char.IsUpper(_text[i - 1]) == false && char.IsUpper(_text[i]))
            {
                _string.Append(_toInsert);
            }

            _string.Append(_text[i]);
        }

        return _string.ToString();
    }

    public static string[] InsertSpaceBeforeUpperCaseAndNumeric(this string[] _text)
    {
        if (_text == null)
        {
            return _text;
        }

        for (int i = 0; i < _text.Length; i++)
        {
            _text[i] = _text[i].InsertSpaceBeforeUpperCaseAndNumeric();
        }

        return _text;
    }

    public static string InsertSpaceBeforeUpperCaseAndNumeric(this string _text)
    {
        return _text.InsertBeforeUpperCaseAndNumeric(' ');
    }

    public static string InsertBeforeUpperCaseAndNumeric(this string _text, char _char)
    {
        return _text.InsertBeforeUpperCaseAndNumeric(_char.ToString());
    }

    public static string InsertBeforeUpperCaseAndNumeric(this string _text, string _toInsert)
    {
        if (_text.IsNullOrWhitespace())
        {
            return _text;
        }

        int _textLength = _text.Length;

        if (_textLength <= 1)
        {
            return _text;
        }

        StringBuilder _string = new StringBuilder();
        _string.Append(_text[0]);

        for (int i = 1; i < _textLength; i++)
        {
            if ((char.IsUpper(_text[i - 1]) == false && char.IsUpper(_text[i]))
                || (char.IsDigit(_text[i - 1]) == false && char.IsDigit(_text[i]))
                || (char.IsDigit(_text[i - 1]) && char.IsLetter(_text[i])))
            {
                _string.Append(_toInsert);
            }

            _string.Append(_text[i]);
        }

        return _string.ToString();
    }

    public static string InsertSpaceBeforeUpperCaseAndNumericWithUpperSeparation(this string _text)
    {
        return _text.InsertBeforeUpperCaseAndNumericWithUpperSeparation(' ');
    }

    public static string InsertBeforeUpperCaseAndNumericWithUpperSeparation(this string _text, char _char)
    {
        return _text.InsertBeforeUpperCaseAndNumericWithUpperSeparation(_char.ToString());
    }

    public static string InsertBeforeUpperCaseAndNumericWithUpperSeparation(this string _text, string _toInsert)
    {
        if (_text.IsNullOrWhitespace())
        {
            return _text;
        }

        int _textLength = _text.Length;

        if (_textLength <= 1)
        {
            return _text;
        }

        StringBuilder _string = new StringBuilder();
        _string.Append(_text[0]);

        for (int i = 1; i < _textLength; i++)
        {
            if ((char.IsUpper(_text[i - 1]) == false && char.IsUpper(_text[i]))
                || (char.IsDigit(_text[i - 1]) == false && char.IsDigit(_text[i]))
                || (char.IsDigit(_text[i - 1]) && char.IsLetter(_text[i]))
                || (char.IsUpper(_text[i - 1]) && char.IsUpper(_text[i]) && i + 1 < _textLength && char.IsLower(_text[i + 1])))
            {
                _string.Append(_toInsert);
            }

            _string.Append(_text[i]);
        }

        return _string.ToString();
    }

    public static string ReplaceMultipleWhitespacesWithSingleSpace(this string _text)
    {
        return System.Text.RegularExpressions.Regex.Replace(_text, @"\s+", " ");
    }

    public static string ClampLength(this string _text, int _characterLimit, out bool _clamped)
    {
        if (_characterLimit < 0)
        {
            _characterLimit = 0;
        }

        int _length = _text == null ? 0 : _text.Length;

        if (_length <= _characterLimit)
        {
            _clamped = false;
            return _text;
        }

        _clamped = true;
        return _text.Remove(_characterLimit);
    }

    public static string PascalToSentence(this string _text, int _characterLimit)
    {
        if (_text == null || _text.Length == 0)
        {
            return _text;
        }

        StringBuilder _returnBuilder = new StringBuilder(_characterLimit);

        _returnBuilder.Append(char.ToUpper(_text[0]));

        for (int i = 1; i < _text.Length; i++)
        {
            if (char.IsLower(_text[i]))
            {
                _returnBuilder.Append(_text[i]);
            }
            else
            {
                _returnBuilder.Append(" ");
                _returnBuilder.Append(char.ToLower(_text[i]));
            }
        }

        return _returnBuilder.ToString();
    }

    public static string FirstCharToUpper(this string _text)
    {
        return _text == null || _text.Length < 1
            ? _text
            : string.Concat(_text[0].ToString().ToUpper(), _text[1..]);
    }

    public static string EveryWordToUpper(this string _text)
    {
        string[] _words = _text.Split(' ');

        for (int i = 0; i < _words.Length; i++)
        {
            if (_words[i].IsNullEmptyOrWhitespace())
            {
                continue;
            }

            _words[i] = char.ToUpper(_words[i][0]) + _words[i].Substring(1);
        }

        return string.Join(" ", _words);
    }

    public static string FirstCharToLower(this string _text)
    {
        return _text == null || _text.Length < 1
            ? _text
            : string.Concat(_text[0].ToString().ToLower(), _text[1..]);
    }

    public static string RemoveStart(this string _text, string _textToRemove, System.StringComparison _comparison = System.StringComparison.OrdinalIgnoreCase)
    {
        if (_text.StartsWith(_textToRemove, _comparison))
        {
            return _text.Remove(0, _textToRemove.Length);
        }

        return _text;
    }

    public static string RemoveEnd(this string _text, string _textToRemove, System.StringComparison _comparison = System.StringComparison.OrdinalIgnoreCase)
    {
        if (_text.EndsWith(_textToRemove, _comparison))
        {
            int _toRemoveCount = _textToRemove.Length;
            return _text.Remove(_text.Length - _toRemoveCount, _toRemoveCount);
        }

        return _text;
    }

    public static string RemoveCharacters(this string _text, params char[] _characters)
    {
        StringBuilder _builder = new StringBuilder();

        int _textLength = _text.Length;
        int _charCount = _characters.Length;

        for (int i = 0; i < _textLength; i++)
        {
            if (_isCharValid(_text[i]))
            {
                _builder.Append(_text[i]);
            }
        }

        return _builder.ToString();

        bool _isCharValid(char _char)
        {
            for (int i = 0; i < _charCount; i++)
            {
                if (_characters[i] == _char)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public static string RemovePart(this string _text, string _textToRemove)
    {
        while (_text.Contains(_textToRemove))
        {
            _text = _text.Remove(_text.IndexOf(_textToRemove), _textToRemove.Length);
        }

        return _text;
    }

    public static string Join(this string[] _array, string _separator = "")
    {
        StringBuilder _builder = new StringBuilder();
        int _arrLength = _array.Length;

        for (int i = 0; i < _arrLength; i++)
        {
            _builder.Append(_array[i]);

            if (i < _arrLength - 1)
            {
                _builder.Append(_separator);
            }
        }

        return _builder.ToString();
    }

    public static string[] RemoveWhitespaces(this string[] _array)
    {
        List<string> _list = new List<string>();

        for (int i = 0; i < _array.Length; i++)
        {
            if (_array[i].ContainsOnlyWhiteSpace() == false)
            {
                _list.Add(_array[i]);
            }
        }

        return _list.ToArray();
    }

    public static string[] RemoveWhitespacesAndTrimElements(this string[] _array)
    {
        List<string> _list = new List<string>();

        for (int i = 0; i < _array.Length; i++)
        {
            string _element = _array[i].RemoveSpaces().TrimStart().TrimEnd();

            if (_element.ContainsOnlyWhiteSpace() == false)
            {
                _list.Add(_element);
            }
        }

        return _list.ToArray();
    }

    public static string TrimEnd(this string _input, string _suffixToRemove)
    {
        if (_suffixToRemove != null && _input.EndsWith(_suffixToRemove))
        {
            return _input.Substring(0, _input.Length - _suffixToRemove.Length);
        }

        return _input;
    }

    public static string TrimStart(this string _input, string _prefixToRemove)
    {
        if (_prefixToRemove != null && _input.StartsWith(_prefixToRemove))
        {
            return _input.Substring(_prefixToRemove.Length, _input.Length - _prefixToRemove.Length);
        }

        return _input;
    }

    public static bool ContainsOnlyWhiteSpace(this string _text)
    {
        for (int i = 0; i < _text.Length; ++i)
        {
            if (char.IsWhiteSpace(_text[i]) == false)
            {
                return false;
            }
        }

        return true;
    }

    public static string CheckIfEmpty(this string _text, string _messageToReturnWhenEmpty = "Empty")
    {
        return _text.IsNullEmptyOrWhitespace() ? _messageToReturnWhenEmpty : _text;
    }

    public static int CountLines(this string _text)
    {
        int _count = 0;

        if (string.IsNullOrEmpty(_text) == false)
        {
            _count = _text.Length - _text.Replace("\n", string.Empty).Length;

            // if the last char of the string is not a newline, make sure to count that line too
            if (_text[_text.Length - 1] != '\n')
            {
                ++_count;
            }
        }

        return _count;
    }

    public static string GetStringInRange(this string _text, int _startIndex, int _endIndex)
    {
        StringBuilder _stringBuilder = new StringBuilder();

        if (_endIndex > _text.Length)
        {
            _endIndex = _text.Length - 1;
        }

        for (int i = _startIndex; i < _endIndex; i++)
        {
            _stringBuilder.Append(_text[i]);
        }

        return _stringBuilder.ToString();
    }

    public static bool ContainsNewLineStringText(this string _text)
    {
        return _text.Contains("\\n");
    }

    public static string FixNewLineCharacters(this string _text)
    {
        return _text.Replace("\\n", "\n").Replace("/n", "\n", StringComparison.Ordinal);
    }

    public static string ToString01(this bool _bit)
    {
        return _bit ? "1" : "0";
    }

    public static char ToChar01(this bool _bit)
    {
        return _bit ? '1' : '0';
    }
}
