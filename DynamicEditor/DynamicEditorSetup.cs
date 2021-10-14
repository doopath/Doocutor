using System.Threading.Tasks;
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
            // Set update rate in milliseconds. I do not recommend to set the value less than 300,
            // because of the OutBufferSizeHandler behaves unstable.
            var bufferSizeUpdateRate = 300;

            var iterator = new DynamicConsoleInputFlowIterator();
            var commandHandler = new DynamicCommandHandler();
            var standardOutBuffer = new StandardConsoleOutBuffer();
            var cuiRender = new CuiRender(NativeCommandExecutionProvider.SourceCode, standardOutBuffer);
            var outBufferSizeHandler = new OutBufferSizeHandler(standardOutBuffer, cuiRender, bufferSizeUpdateRate);
            var handler = new DynamicKeyFlowHandler(iterator, commandHandler, KeyCombinationsMap.Map, KeyMap.Map, cuiRender);

            cuiRender.Render();
            outBufferSizeHandler.Start();
            handler.StartHandling();
            outBufferSizeHandler.Stop();
        }
    }
}
