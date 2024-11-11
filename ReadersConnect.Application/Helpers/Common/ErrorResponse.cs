using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace ReadersConnect.Application.Helpers.Common
{
    public class ErrorResponse
    {
        public string ErrorDescription { get; set; } = "An Error Occured, Try Again Later.";
        public static ErrorResponse GetModeStateErrors(ValueEnumerable errors)
        {
            var message = string.Join(" | ", errors
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return new ErrorResponse { ErrorDescription = !string.IsNullOrEmpty(message) ? message :"Fill Required Values" };
        }
    }
}
