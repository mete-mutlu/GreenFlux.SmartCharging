using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using GreenFlux.SmartCharging.API.Domain.Exceptions;

namespace GreenFlux.SmartCharging.API.Attributes
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException)
                context.Result = new BadRequestObjectResult(context.Exception.Message);

        }
    }
}
