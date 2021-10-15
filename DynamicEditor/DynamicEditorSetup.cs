using Domain.Core;
using Domain.Core.OutBuffers;
using DynamicEditor.Core;
using DynamicEditor.Core.CommandHandlers;
using DynamicEditor.Core.FlowHandlers;
using DynamicEditor.Core.Iterators;
using DynamicEditor.Core.Scenes;

namespace DynamicEditor
{
    public class DynamicEditorSetup : IEditorSetup
    {
        public void Run(string[] args)
        {
            // Set update rate in milliseconds. I do not recommend to set the value less than 300,
            // because of the OutBufferSizeHandler behaves unstable.
            var bufferSizeUpdateRate = 300;

            var iterator = new DynamicConsoleInputFlowIterator();
            var commandHandler = new DynamicCommandHandler();
            var standardOutBuffer = new StandardConsoleOutBuffer();
            var cuiScene = new CuiScene();
            var cuiRender = new CuiRender(NativeCommandExecutionProvider.SourceCode, standardOutBuffer, cuiScene);
            var outBufferSizeHandler = new OutBufferSizeHandler(standardOutBuffer, cuiRender, bufferSizeUpdateRate);
            var handler = new DynamicKeyFlowHandler(iterator, commandHandler, KeyCombinationsMap.Map, KeyMap.Map, cuiRender);

            cuiRender.Render();
            outBufferSizeHandler.Start();
            handler.StartHandling();
            outBufferSizeHandler.Stop();
        }
    }
}
