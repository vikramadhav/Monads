/*********************************************************************************
Copyright (C) 2018 Vikram Verma

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*************************************************************************************/

namespace ResultMonads
{
    using System;
    using System.Runtime.Serialization;
    using Functional.Maybe;

    /// <summary>
    /// Represents an error which can be returned by any function.
    /// </summary>
    [DataContract]
    public struct Error
    {
        [DataMember]
        private readonly string _errorType;

        [DataMember]
        private readonly bool _isRetryable;

        [DataMember]
        private readonly SerializedException? _serializedUnderlyingException;

        public string ErrorType => _errorType;

        public bool IsRetryable => _isRetryable;

        public Maybe<Exception> UnderlyingException { get; }

        public Maybe<SerializedException> SerializedUnderlyingException => _serializedUnderlyingException.ToMaybe();

        public Maybe<T> ToErrorType<T>() where T : struct
        {
            if (typeof(T).BaseType != typeof(Enum))
            {
                return Maybe<T>.Nothing;
            }
            return Enum.TryParse(ErrorType, out T enumValue) ? enumValue.ToMaybe() : Maybe<T>.Nothing;
        }

        private Error(string errorType, bool isRetryable, Exception underlyingException = null)
        {
            _errorType = errorType;
            _isRetryable = isRetryable;
            UnderlyingException = underlyingException.ToMaybe();
            if (UnderlyingException.IsSomething())
            {
                _serializedUnderlyingException = new SerializedException(underlyingException);
            }
            else
            {
                _serializedUnderlyingException = null;
            }
        }

        public static Error Create<T>(T errorType, bool isRetryable = false)
        {
            try
            {
                return new Error(Enum.GetName(typeof(T), errorType), isRetryable);
            }
            catch (ArgumentException)
            {
                var error = errorType?.ToString() ?? "UnknownErrorType";
                return new Error(error, isRetryable);
            }
        }

        public static Error Create<T>(T errorType, bool isRetryable, Exception underlyingException)
        {
            try
            {
                return new Error(Enum.GetName(typeof(T), errorType), isRetryable, underlyingException);
            }
            catch (ArgumentException)
            {
                var error = errorType?.ToString() ?? "UnknownErrorType";
                return new Error(error, isRetryable);
            }
        }
    }
}