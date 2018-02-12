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

    [DataContract]
    public struct ResultCommon
    {
        [DataMember] private readonly Error? _error;

        private Maybe<Error> SafeError => _error.ToMaybe();

        public bool IsFailure => SafeError.IsSomething();

        public bool IsSuccess => !IsFailure;

        public bool IsTransientFailure => SafeError.IsSomething() && SafeError.Value.IsRetryable;

        public Error Error
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException("There is no error for success.");
                }
                return _error.GetValueOrDefault();
            }
        }

        public ResultCommon(Maybe<Error> error)
        {
            if (error.IsSomething())
            {
                _error = error.Value;
            }
            else
            {
                _error = null;
            }
        }
    }
}