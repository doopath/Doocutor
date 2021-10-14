using Domain.Core;
using Domain.Core.CommandHandlers;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using StaticEditor.Core.CommandHandlers;
using StaticEditor.Core.FlowHandlers;
using StaticEditor.Core.Iterators;

namespace StaticEditor
{
    public class StaticEditorSetup : IEditorSetup
    {
        public void Run(string[] args)
        {
            IInputFlowIterator iterator = new StaticConsoleInputFlowIterator();
            ICommandHandler commandHandler = new StaticCommandHandler();
            IInputFlowHandler handler = new StaticCommandFlowHandler(iterator, commandHandler);

            handler.StartHandling();
        }
    }
}
