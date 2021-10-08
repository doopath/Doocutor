using Domain.Core;
using Domain.Core.OutBuffers;
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
            var standardOutBuffer = new StandardConsoleOutBuffer();
            var cuiRender = new CuiRender(NativeCommandExecutionProvider.SourceCode, standardOutBuffer);
            var handler = new DynamicKeyFlowHandler(iterator, commandHandler, KeyCombinationsMap.Map, KeyMap.Map, cuiRender);

            cuiRender.Clear();
            cuiRender.Render();
            handler.StartHandling();
        }
    }
}
