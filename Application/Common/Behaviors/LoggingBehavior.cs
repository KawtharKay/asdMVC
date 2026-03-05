using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            if (request is ISensitiveRequest)
                logger.LogInformation("Handling {RequestName}", requestName);
            else
                logger.LogInformation(
                    "Handling {RequestName} | Data: {@Request}",
                    requestName, request);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var response = await next();
                stopwatch.Stop();

                logger.LogInformation(
                    "Handled {RequestName} | Duration: {Duration}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(ex,
                    "Error handling {RequestName} | Duration: {Duration}ms | Error: {Error}",
                    requestName, stopwatch.ElapsedMilliseconds, ex.Message);

                throw;
            }
        }
    }
}