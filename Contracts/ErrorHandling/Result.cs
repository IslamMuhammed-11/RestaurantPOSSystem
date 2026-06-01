using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ErrorHandling
{
    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(T value, bool isSuccess, Error? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(value, true, null);

        public static Result<T> Failure(Error error) => new(default!, false, error);
    }
}