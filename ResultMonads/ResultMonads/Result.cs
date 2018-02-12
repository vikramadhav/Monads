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
    /// Represents a result which can be returned by any function.
    /// </summary>
    [DataContract]
    public struct Result : IResult
    {
        [DataMember] private readonly ResultCommon _common;

        public bool IsFailure => _common.IsFailure;

        public bool IsTransientFailure => _common.IsTransientFailure;

        public bool IsSuccess => _common.IsSuccess;

        public Error Error => _common.Error;

        private Result(Maybe<Error> error)
        {
            _common = new ResultCommon(error);
        }

        public static Result Ok()
        {
            return new Result(Maybe<Error>.Nothing);
        }

        public static Result Failure(Error error)
        {
            return new Result(error.ToMaybe());
        }
    }

    /// <summary>
    /// Represents a result (that encapsulates a generic result value) which can be returned by any function.
    /// </summary>
    [DataContract]
    public struct Result<T> : IResult
    {
        [DataMember] private readonly ResultCommon _common;

        // Not using Maybe<T> since Maybe<> is not contravariant
        [DataMember] private readonly T _resultValue;

        public T ResultValue
        {
            get
            {
                if (IsFailure)
                {
                    throw new InvalidOperationException("There is no result value for failure.");
                }
                return _resultValue;
            }
        }

        public bool IsFailure => _common.IsFailure;

        public bool IsTransientFailure => _common.IsTransientFailure;

        public bool IsSuccess => _common.IsSuccess;

        public Error Error => _common.Error;

        private Result(Error error)
        {
            _common = new ResultCommon(error.ToMaybe());
            _resultValue = default(T);
        }

        private Result(T resultValue)
        {
            _common = new ResultCommon(Maybe<Error>.Nothing);
            _resultValue = resultValue;
        }

        public static Result<T> Ok(T resultValue)
        {
            resultValue.ToMaybe().OrElse(() => new ArgumentException(nameof(resultValue)));
            return new Result<T>(resultValue);
        }

        public static Result<T> Failure(Error error)
        {
            return new Result<T>(error);
        }
    }
}