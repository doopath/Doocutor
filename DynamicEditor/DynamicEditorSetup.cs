using Domain.Core;
using Domain.Core.CodeBuffers;
using Domain.Core.CommandHandlers;
using Domain.Core.Exceptions;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using DynamicEditor.Core;
using DynamicEditor.Core.CommandHandlers;
using DynamicEditor.Core.FlowHandlers;
using DynamicEditor.Core.Iterators;
using DynamicEditor.Core.Scenes;

namespace DynamicEditor
{
    public class DynamicEditorSetup : IEditorSetup
    {
        public int BufferSizeUpdateRate { get; set; }
        public ICodeBuffer SourceCodeBuffer { get; set; }
        public IInputFlowIterator Iterator { get; set; }
        public ICommandHandler CommandHandler { get; set; }
        public IOutBuffer OutBuffer { get; set; }
        public IScene CuiScene { get; set; }
        public CuiRender Render { get; set; }
        public OutBufferSizeHandler OutBufferSizeHandler { get; set; }
        public IInputFlowHandler InputFlowHandler { get; set; }

        public DynamicEditorSetup()
        {
            BufferSizeUpdateRate = 300;
            Iterator = new DynamicConsoleInputFlowIterator();
            CommandHandler = new DynamicCommandHandler();
            OutBuffer = new StandardConsoleOutBuffer();
            CuiScene = new CuiScene();
            Render = new CuiRender(NativeCommandExecutionProvider.SourceCodeBuffer, OutBuffer, CuiScene);
            OutBufferSizeHandler = new OutBufferSizeHandler(OutBuffer, Render, BufferSizeUpdateRate);
            InputFlowHandler = new DynamicKeyFlowHandler(Iterator, CommandHandler, KeyCombinationsMap.Map, KeyMap.Map, Render);
        }

        public void Run(string[] args)
        {
            // Set update rate in milliseconds. I do not recommend to set the value less than 300,
            // because of the OutBufferSizeHandler behaves unstable.
            OutBuffer.CursorVisible = false;
            Render.EnableDeveloperMonitor(); // dev-only feature
            Render.Render();
            OutBufferSizeHandler.Start();

            try
            {
                InputFlowHandler.StartHandling();

            }
            catch (InterruptedExecutionException)
            {
                OutBufferSizeHandler.Stop();
                OutBuffer.CursorVisible = true;
            }
        }
    }
}