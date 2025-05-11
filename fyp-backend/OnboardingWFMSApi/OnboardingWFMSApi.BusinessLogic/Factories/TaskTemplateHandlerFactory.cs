using Microsoft.Extensions.DependencyInjection;
using OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers;
using OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Factories
{
    public interface ITaskTemplateHandlerFactory
    {
        public ITaskTemplateHandler? GetHandler(string taskTypeId);
    }

    public class TaskTemplateHandlerFactory : ITaskTemplateHandlerFactory
    {
        private IServiceProvider _serviceProvider;
        private Dictionary<string, ITaskTemplateHandler> taskTemplateHandlers;

        public TaskTemplateHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            taskTemplateHandlers = CacheHandlers();
        }

        private Dictionary<string, ITaskTemplateHandler> CacheHandlers()
        {
            // find interfaces that implement ITaskTemplateHandler
            var handlersInterfaces = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ITaskTemplateHandler).IsAssignableFrom(t) && t.IsInterface && t != typeof(ITaskTemplateHandler))
                .ToList();

            // build cache of handlers linked to their task type Ids
            var handlersDict = new Dictionary<string, ITaskTemplateHandler>();
            foreach (var handlerType in handlersInterfaces)
            {
                var handler = _serviceProvider.GetService(handlerType) as ITaskTemplateHandler;
                if (handler == null)
                {
                    continue;
                }
                handlersDict.Add(handler.GetTaskTypeId(), handler);
            }
            return handlersDict;
        }

        public ITaskTemplateHandler? GetHandler(string taskTypeId)
        {
            if (taskTemplateHandlers.Count == 0)
            {
                CacheHandlers();
            }
            // retrieve appriopriate task template handler by task type Id
            var selectedHandler = taskTemplateHandlers.GetValueOrDefault(taskTypeId);
            return selectedHandler;
        }
    }
}
