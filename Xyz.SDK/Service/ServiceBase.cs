using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Xyz.SDK.Dao;

namespace Xyz.SDK.Service
{
    public abstract class ServiceBase
    {
        protected readonly IUnitOfWork Uow;
        protected readonly ILogger Logger;

        protected ServiceBase(IUnitOfWork uow, ILogger<ServiceBase> logger)
        {
            Uow = uow;
            Logger = logger;
        }

        protected bool Validate<T>(IValidator validator,  T objectToValidate)
        {
            var context = new ValidationContext<T>(objectToValidate);
            var validationResult = validator.Validate(context);

            if (validationResult.IsValid)
                return true;
            
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    Logger.LogWarning($"Validation error: {error.ErrorMessage}");
                }
            }
            return false;
        }
    }
}
