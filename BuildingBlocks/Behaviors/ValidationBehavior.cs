using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BuildingBlocks.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> (IEnumerable<IValidator<TRequest>> validators): IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse> 
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResult = await Task.WhenAll(validators.Select(validator=>validator.ValidateAsync(context,cancellationToken)));

            var failures = validationResult.Where(vr=>vr.Errors.Any()).SelectMany(v=>v.Errors).ToList();

            if(failures.Any())
                throw new ValidationException(failures);

            return await next();
        }
    }
}
