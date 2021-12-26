using Domain.Core;
using Domain.Core.ColorSchemes;
using Domain.Core.CommandHandlers;
using Domain.Core.Exceptions.NotExitExceptions;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Core.TextBuffers;
using Domain.Core.Widgets;
using Domain.Options;
using DynamicEditor.Core;
using DynamicEditor.Core.CommandHandlers;
using DynamicEditor.Core.FlowHandlers;
using DynamicEditor.Core.Iterators;
using DynamicEditor.Core.Scenes;

namespace DynamicEditor;

public class DynamicEditorSetup : IEditorSetup
{
    public int? BufferSizeUpdateRate { get; set; }
    public uint SourceCodeBufferHistoryLimit { get; set; }
    public ITextBuffer TextBuffer { get; set; }
    public IInputFlowIterator Iterator { get; set; }
    public ICommandHandler CommandHandler { get; set; }
    public IOutBuffer OutBuffer { get; set; }
    public IScene CuiScene { get; set; }
    public OutBufferSizeHandler OutBufferSizeHandler { get; set; }
    public IInputFlowHandler InputFlowHandler { get; set; }
    public IColorScheme ColorScheme { get; set; }
    public ColorSchemesRepository ColorSchemesRepository { get; set; }

    public DynamicEditorSetup()
    {
        Settings.ColorScheme = new DefaultLightColorScheme();

        ColorSchemesRepository = new();
        ColorSchemesRepository.Add(Settings.ColorScheme);
        ColorSchemesRepository.Add(new DefaultDarkColorScheme());

        BufferSizeUpdateRate = 300;
        SourceCodeBufferHistoryLimit = 10000;
        TextBuffer = new TextBuffer(SourceCodeBufferHistoryLimit);
        EditorCommands.InitializeCodeBuffer(TextBuffer);
        Iterator = new DynamicConsoleInputFlowIterator();
        CommandHandler = new DynamicCommandHandler();
        OutBuffer = new StandardConsoleOutBuffer();
        CuiScene = new CuiScene();

        CuiRender.Scene = CuiScene;
        CuiRender.OutBuffer = OutBuffer;
        CuiRender.TextBuffer = TextBuffer;
        CuiRender.InitializeDeveloperMonitor();

        WidgetsMount.Scene = CuiScene;
        WidgetsMount.OutBuffer = OutBuffer;
        WidgetsMount.Refresh = CuiRender.Render;

        OutBufferSizeHandler = new OutBufferSizeHandler(
                OutBuffer,
                BufferSizeUpdateRate!.Value);

        InputFlowHandler = new DynamicKeyFlowHandler(
                Iterator,
                CommandHandler,
                KeyCombinationsMap.Map);

        Settings.OutBuffer = OutBuffer;
    }

    public void Run(ProgramOptions options)
    {
        Settings.ColorScheme = ColorSchemesRepository!
            .Get(options.ColorScheme ?? "DoocutorDark");
        OutBuffer!.CursorVisible = false;

#if DEBUG
        CuiRender.EnableDeveloperMonitor();
#endif

        CuiRender.Render();
        OutBufferSizeHandler!.Start();

        try
        {
            InputFlowHandler!.StartHandling();
        }
        catch (InterruptedExecutionException)
        {
            OutBufferSizeHandler.Stop();
            OutBuffer.CursorVisible = true;
            CuiRender.Clear();
            throw;
        }
    }
}
