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
    using System.Text;

    /// <summary>
    /// Provides a container to serialize exception details since <see cref="Exception"/> are
    /// not serializable via <see cref="DataContractSerializer"/>.
    /// </summary>
    [DataContract]
    public struct SerializedException
    {
        [DataMember] private readonly string _exceptionTypeName;

        public string ExceptionTypeName => _exceptionTypeName;

        [DataMember] private readonly string _exceptionStackTrace;

        public string ExceptionStackTrace => _exceptionStackTrace;

        [DataMember] private readonly string _exceptionMessage;

        public string ExceptionMessage => _exceptionMessage;

        public SerializedException(Exception exception)
        {
            if (exception != null)
            {
                _exceptionTypeName = exception?.GetType().FullName;
                var stackTraceBuilder = new StringBuilder();
                var exceptionMessageBuilder = new StringBuilder();

                var exp = exception;
                while (exp != null)
                {
                    stackTraceBuilder.AppendLine(exp.StackTrace ?? "No StackTrace Available");
                    exceptionMessageBuilder.AppendLine(exp.Message);
                    exp = exp.InnerException;
                }
                _exceptionStackTrace = stackTraceBuilder.ToString();
                _exceptionMessage = exceptionMessageBuilder.ToString();
            }
            else
            {
                _exceptionTypeName = string.Empty;
                _exceptionMessage = string.Empty;
                _exceptionStackTrace = string.Empty;
            }
        }
    }
}