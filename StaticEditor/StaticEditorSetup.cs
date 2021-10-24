using Domain.Core;
using Domain.Core.CommandHandlers;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Options;
using StaticEditor.Core.CommandHandlers;
using StaticEditor.Core.FlowHandlers;
using StaticEditor.Core.Iterators;

namespace StaticEditor
{
    public class StaticEditorSetup : IEditorSetup
    {
        public void Run(ProgramOptions options)
        {
            IInputFlowIterator iterator = new StaticConsoleInputFlowIterator();
            ICommandHandler commandHandler = new StaticCommandHandler();
            IInputFlowHandler handler = new StaticCommandFlowHandler(iterator, commandHandler);

            handler.StartHandling();
        }
    }
}
