using Domain.Core;
using DynamicEditor.Core.Iterators;
using DynamicEditor.Core.CommandHandlers;
using DynamicEditor.Core.FlowHandlers;
using Domain.Core.CodeBuffers;
using DynamicEditor.Core;

namespace DynamicEditor
{
    public class DynamicEditorSetup : EditorSetup
    {
        public void Run(string[] args)
        {
            var iterator = new DynamicConsoleInputFlowIterator();
            var commandHandler = new DynamicCommandHandler();
            var cuiRender = new CUIRender(NativeCommandExecutionProvider.SourceCode);
            var handler = new DynamicKeyFlowHandler(iterator, commandHandler, KeyCombinationsMap.Map, cuiRender);
            handler.StartHandling();
        }
    }
}
