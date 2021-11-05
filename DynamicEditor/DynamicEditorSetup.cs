using Domain.Core;
using Domain.Core.CodeBuffers;
using Domain.Core.ColorSchemes;
using Domain.Core.CommandHandlers;
using Domain.Core.Exceptions;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Options;
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
        public IColorScheme ColorScheme { get; set; }
        public ColorSchemesRepository ColorSchemesRepository { get; set; }

        public DynamicEditorSetup()
        {
            var defaultLightColorScheme = new DefaultLightColorScheme();
            
            ColorSchemesRepository = new();
            ColorSchemesRepository.Add(defaultLightColorScheme);
            ColorSchemesRepository.Add(new DefaultDarkColorScheme());

            SourceCodeBuffer = NativeCommandExecutionProvider.SourceCodeBuffer;
            BufferSizeUpdateRate = 300;
            Iterator = new DynamicConsoleInputFlowIterator();
            CommandHandler = new DynamicCommandHandler();
            OutBuffer = new StandardConsoleOutBuffer();
            CuiScene = new CuiScene();
            Render = new CuiRender(SourceCodeBuffer, OutBuffer, CuiScene, defaultLightColorScheme);
            OutBufferSizeHandler = new OutBufferSizeHandler(OutBuffer, Render, BufferSizeUpdateRate);
            InputFlowHandler = new DynamicKeyFlowHandler(
                    Iterator, CommandHandler, KeyCombinationsMap.Map, Render);
        }

        public void Run(ProgramOptions options)
        {
            // Set update rate in milliseconds. I do not recommend to set the value less than 300,
            // because of the OutBufferSizeHandler behaves unstable.
            ColorScheme = ColorSchemesRepository.Get(options.ColorScheme ?? "DoocutorDark");
            OutBuffer.CursorVisible = false;
            Render.ColorScheme = ColorScheme;

#if DEBUG
            Render.EnableDeveloperMonitor();
#endif

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

                throw;
            }
        }
    }
}
