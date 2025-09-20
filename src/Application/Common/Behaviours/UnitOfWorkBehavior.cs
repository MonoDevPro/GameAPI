using GameWeb.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameWeb.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior do MediatR que dispara SaveChanges no final de cada request.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var isCommand = request is ICommand || request is ICommand<TResponse>;
        var response = await next();
        if (!isCommand)
        {
            return response; // Não persiste para queries
        }

        var result = await unitOfWork.SaveChangesAsync(ct);
        logger.LogInformation("Save changes for command {RequestType} HasSaves: {Saves}", typeof(TRequest).Name, result);
        return response;
    }
}
