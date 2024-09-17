using Microsoft.Extensions.Logging;
using SimpleBlogMVC.Data;
using System;
using System.Threading.Tasks;

namespace SimpleBlogMVC.Services
{
    public abstract class BaseService
    {
        protected readonly ILogger _logger;
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseService(ILogger logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string errorMessage, bool logError = true)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                if (logError)
                {
                    _logger.LogError(ex, errorMessage);
                }
                throw;
            }
        }

        protected async Task ExecuteAsync(Func<Task> action, string errorMessage, bool logError = true)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                if (logError)
                {
                    _logger.LogError(ex, errorMessage);
                }
                throw;
            }
        }
    }
}
