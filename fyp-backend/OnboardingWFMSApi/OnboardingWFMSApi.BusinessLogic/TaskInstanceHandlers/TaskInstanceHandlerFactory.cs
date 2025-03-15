using Microsoft.Extensions.DependencyInjection;
using OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers
{
    public interface ITaskInstanceHandlerFactory
    {
        public ITaskInstanceHandler GetHandler(string taskTypeId);
    }

    public class TaskInstanceHandlerFactory : ITaskInstanceHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskInstanceHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITaskInstanceHandler GetHandler(string taskTypeId)
        {
            // find interfaces that implement ITaskTemplateHandler
            var handlersInterfaces = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ITaskInstanceHandler).IsAssignableFrom(t) && t.IsInterface && t != typeof(ITaskInstanceHandler))
                .ToList();

            ITaskInstanceHandler selectedHandler = null;
            foreach (var handlerType in handlersInterfaces)
            {
                var handler = _serviceProvider.GetService(handlerType) as ITaskInstanceHandler;
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
