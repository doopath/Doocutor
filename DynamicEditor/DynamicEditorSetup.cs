using Domain.Core;
using DynamicEditor.Core;
using DynamicEditor.Core.CommandHandlers;
using DynamicEditor.Core.FlowHandlers;
using DynamicEditor.Core.Iterators;

namespace DynamicEditor
{
    public class DynamicEditorSetup : EditorSetup
    {
        public void Run(string[] args)
        {
            var iterator = new DynamicConsoleInputFlowIterator();
            var commandHandler = new DynamicCommandHandler();
            var cuiRender = new CuiRender(NativeCommandExecutionProvider.SourceCode);
            var handler = new DynamicKeyFlowHandler(iterator, commandHandler, KeyCombinationsMap.Map, KeyMap.Map, cuiRender);

            cuiRender.Clear();
            handler.StartHandling();
        }
    }
}
