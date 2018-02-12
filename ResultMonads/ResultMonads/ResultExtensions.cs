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
    using System.Threading.Tasks;

    public static class ResultExtensions
    {
        #region IResult Extensions

        public static IResult OnSuccess(this IResult result, Action action)
        {
            if (result.IsSuccess)
            {
                action();
            }
            return result;
        }

        public static IResult OnSuccess(this IResult result, Func<IResult> fn)
        {
            if (result.IsSuccess)
            {
                return fn();
            }
            return result;
        }

        public static Result<T> OnSuccess<T>(this IResult result, Func<Result<T>> fn)
        {
            if (result.IsSuccess)
            {
                return fn();
            }
            return Result<T>.Failure(result.Error);
        }

        public static IResult OnFailure(this IResult result, Action<IResult> action)
        {
            if (result.IsFailure)
            {
                action(result);
            }
            return result;
        }

        public static async Task<Result> OnSuccessAsync(this IResult result, Func<Task<Result>> fn)
        {
            if (result.IsSuccess)
            {
                return await fn();
            }
            return Result.Failure(result.Error);
        }

        public static async Task<IResult> OnSuccessAsync(this IResult result, Func<Task<IResult>> fn)
        {
            if (result.IsSuccess)
            {
                return await fn();
            }
            return Result.Failure(result.Error);
        }

        public static IResult OnBoth(this IResult result, Action action)
        {
            action();
            return result;
        }

        #endregion

        #region Result Extensions

        public static async Task<Result> OnFailureAsync(this Task<Result> resultTask, Func<Task<Result>> fn)
        {
            var result = await resultTask;
            if (result.IsFailure)
            {
                await fn();
            }
            return result;
        }

        public static async Task<Result> OnTransientFailureAsync(this Task<Result> resultTask, Func<Task<Result>> fn)
        {
            var result = await resultTask;
            if (result.IsTransientFailure)
            {
                await fn();
            }
            return result;
        }

        public static Result OnBoth(this Result result, Action action)
        {
            action();
            return result;
        }

        public static Result OnSuccess(this Result result, Func<Result> fn)
        {
            if (result.IsSuccess)
            {
                return fn();
            }
            return result;
        }

        public static async Task<Result> OnBothAsync(this Task<Result> result, Action action)
        {
            action();
            return await result;
        }

        #endregion

        #region Result<T> Extensions

        public static async Task<Result<TResult>> OnSuccessAsync<T, TResult>(
            this Result<T> result, Func<T, Task<TResult>> fn)
        {
            if (result.IsSuccess)
            {
                var newResult = await fn(result.ResultValue);
                return Result<TResult>.Ok(newResult);
            }
            return Result<TResult>.Failure(result.Error);
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Func<Result<T>> fn)
        {
            if (result.IsFailure)
            {
                return fn();
            }
            return result;
        }

        public static Result<TResult> OnSuccess<T, TResult>(this Result<T> result, Func<T, Result<TResult>> fn)
        {
            if (result.IsSuccess)
            {
                return fn(result.ResultValue);
            }
            return Result<TResult>.Failure(result.Error);
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Func<T, Result> fn)
        {
            if (result.IsSuccess)
            {
                var fnResult = fn(result.ResultValue);
                if (fnResult.IsSuccess)
                {
                    return Result<T>.Ok(result.ResultValue);
                }
                return Result<T>.Failure(fnResult.Error);
            }
            return Result<T>.Failure(result.Error);
        }

        public static async Task<Result> OnSuccessAsync<T>(this Result<T> result, Func<T, Task<Result>> fn)
        {
            if (result.IsSuccess)
            {
                var newResult = await fn(result.ResultValue);
                return newResult;
            }
            return Result.Failure(result.Error);
        }

        public static Result<T> Log<T>(
            this Result<T> result, string processorName, Action<string, T> successLog, Action<string, Error> errorLog)
        {
            if (result.IsSuccess)
            {
                successLog(processorName, result.ResultValue);
            }
            else
            {
                errorLog(processorName, result.Error);
            }
            return result;
        }

        #endregion
    }
}