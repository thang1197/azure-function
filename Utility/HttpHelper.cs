using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Azure_Function.Utility
{
    public class HttpHelper
    {
        public static IActionResult GenerateResponse<T>(T data, int statusCode = StatusCodes.Status200OK)
        {
            try
            {
                var response = new
                {
                    status = "success",
                    code = statusCode,
                    data = data
                };
                var result = new ObjectResult(response)
                {
                    StatusCode = statusCode
                };

                result.ContentTypes.Add("application/json");
                return result;
            }
            catch (Exception ex)
            {
                var response = new
                {
                    status = "error",
                    code = StatusCodes.Status500InternalServerError,
                    error = ex.Message
                };
                var result = new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return result;
            }
        }
    }
}