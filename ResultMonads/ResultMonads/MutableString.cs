
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

    [DataContract]
    public sealed class MutableString : ValueObject<MutableString>
    {
        [DataMember]
        private readonly StringComparison _valueComparison;

        public MutableString(string originalValue, StringComparison valueComparison)
        {
            OriginalValue = originalValue;
            CurrentValue = originalValue;
            _valueComparison = valueComparison;
        }

        [DataMember]
        public string OriginalValue { get; private set; }

        [DataMember]
        public string CurrentValue { get; private set; }

        public bool HasChanged => string.Equals(OriginalValue, CurrentValue, _valueComparison) == false;

        public void Mutate(string newValue)
        {
            CurrentValue = newValue;
        }

        protected override bool EqualsCore(MutableString other)
        {
            return string.Equals(CurrentValue, other.CurrentValue, _valueComparison) &&
                   string.Equals(OriginalValue, other.OriginalValue, _valueComparison);
        }

        protected override int GetHashCodeCore()
        {
            return CurrentValue.GetHashCode();
        }

        public static implicit operator string(MutableString mutableString)
        {
            return mutableString.CurrentValue;
        }
    }
}