using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.BaseResponse
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null) => new()
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };

        public static ApiResponse<T> Failure(List<string> errors, string? message = null) => new()
        {
            IsSuccess = false,
            Errors = errors,
            Message = message
        };
    }
}