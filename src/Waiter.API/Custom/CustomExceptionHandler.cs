﻿using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Waiter.Application.Exceptions;
using Waiter.Application.Models;

namespace Waiter.API.Custom
{
    /// <summary>
    ///
    /// </summary>
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

        /// <summary>
        ///
        /// </summary>
        public CustomExceptionHandler()
        {
            _exceptionHandlers = new()
            {
                { typeof(ValidationException), HandleValidationException }
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            var exceptionType = exception.GetType();

            if (_exceptionHandlers.ContainsKey(exceptionType))
            {
                await _exceptionHandlers[exceptionType].Invoke(httpContext, exception);
                return true;
            }

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static async Task HandleValidationException(HttpContext httpContext, Exception ex)
        {
            var exception = (ValidationException)ex;

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            await httpContext.Response.WriteAsJsonAsync(new ValidationResult(exception));
        }
    }
}