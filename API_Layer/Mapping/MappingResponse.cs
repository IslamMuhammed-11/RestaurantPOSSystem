using Contracts.DTOs.BaseResponse;
using Contracts.Enums;
using Contracts.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API_Layer.Mapping
{
    public static class ResultMappingExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, string? message = null, bool content = true)
        {
            if (result.IsSuccess && content)
                return new OkObjectResult(ApiResponse<T>.Success(result.Value, message));

            if (result.IsSuccess && !content)
                return new NoContentResult();

            return new ObjectResult(new ProblemDetails
            {
                Title = result.Error!.Code.ToString(),
                Detail = result.Error.Message,
                Status = MapStatusCode(result.Error.Code)
            })
            {
                StatusCode = MapStatusCode(result.Error.Code)
            };
        }

        private static int MapStatusCode(ErrorCodes.enErrorCodes code)
        {
            return code switch
            {
                ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION => 409,
                ErrorCodes.enErrorCodes.NOT_FOUND => 404,
                ErrorCodes.enErrorCodes.INVALID_DATA or ErrorCodes.enErrorCodes.INVALID_ID => 400,
                ErrorCodes.enErrorCodes.INVALID_AMOUNT => 400,
                ErrorCodes.enErrorCodes.AUTHENCATION_FAILED => 401,
                ErrorCodes.enErrorCodes.DB_ERROR => 500,
                ErrorCodes.enErrorCodes.AUTHORIZE_FAILED => 403,
                _ => 500
            };
        }
    }
}