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
            // find interfaces that implement ITaskTemplateHandler
            var handlersInterfaces = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ITaskTemplateHandler).IsAssignableFrom(t) && t.IsInterface && t != typeof(ITaskTemplateHandler))
                .ToList();

            ITaskTemplateHandler selectedHandler = null;
            // find the handler that is for this task type by looping through each one, instantiating it and checking its task type Id
            foreach (var handlerType in handlersInterfaces)
            {
                var handler = _serviceProvider.GetService(handlerType) as ITaskTemplateHandler;
                if (handler == null)
                {
                    continue;
                }
                if (handler.GetTaskTypeId() == taskTypeId)
                {
                    selectedHandler = handler;
                }
                else
                {
                    continue;
                }
            }
            return selectedHandler;
        }
    }
}
