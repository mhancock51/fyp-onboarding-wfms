using Microsoft.Extensions.DependencyInjection;
using OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers;
using OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers;
using OnboardingWFMSApi.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Factories
{
    public interface ITaskInstanceHandlerFactory
    {
        public ITaskInstanceHandler? GetHandler(string taskTypeId);
    }

    public class TaskInstanceHandlerFactory : ITaskInstanceHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        private Dictionary<string, ITaskInstanceHandler> taskInstanceHandlers;

        public TaskInstanceHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            taskInstanceHandlers = CacheHandlers();

        }

        private Dictionary<string, ITaskInstanceHandler> CacheHandlers()
        {
            // find interfaces that implement ITaskTemplateHandler
            var handlersInterfaces = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ITaskInstanceHandler).IsAssignableFrom(t) && t.IsInterface && t != typeof(ITaskInstanceHandler))
                .ToList();

            // build cache of handlers linked to their task type Ids
            var handlersDict = new Dictionary<string, ITaskInstanceHandler>();
            foreach (var handlerType in handlersInterfaces)
            {
                var handler = _serviceProvider.GetService(handlerType) as ITaskInstanceHandler;
                if (handler == null)
                {
                    continue;
                }
                handlersDict.Add(handler.GetTaskTypeId(), handler);
            }
            return handlersDict;
        }

        public ITaskInstanceHandler? GetHandler(string taskTypeId)
        {
            taskInstanceHandlers.TryGetValue(taskTypeId, out ITaskInstanceHandler handler);
            return handler ?? null;
        }
    }
}
