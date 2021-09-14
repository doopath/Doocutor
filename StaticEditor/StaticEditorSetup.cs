using Domain.Core;
using Domain.Core.Iterators;
using Domain.Core.FlowHandlers;
using StaticEditor.Core.Iterators;
using StaticEditor.Core.FlowHandlers;
using StaticEditor.Core.CommandHandlers;
using Domain.Core.CommandHandlers;

namespace StaticEditor
{
    public class StaticEditorSetup : EditorSetup
    {
        public void Run(string[] args)
        {
            IInputFlowIterator iterator = new StaticConsoleInputFlowIterator();
            ICommandHandler commandHandler= new StaticCommandHandler();
            IInputFlowHandler handler = new StaticCommandFlowHandler(iterator, commandHandler);

            handler.StartHandling();
        }
    }
}
