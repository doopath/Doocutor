using CommandHandling;
using Common;
using Common.Options;
using CUI;
using CUI.ColorSchemes;
using CUI.OutBuffers;
using CUI.Scenes;
using Domain.Core;
using InputHandling;
using InputHandling.CommandHandlers;
using InputHandling.FlowHandlers;
using InputHandling.Iterators;
using Utils.Exceptions;

namespace Doocutor;

public class DynamicEditorSetup : IEditorSetup
{
    public int? BufferSizeUpdateRate { get; set; }
    public uint SourceCodeBufferHistoryLimit { get; set; }
    public ITextBuffer TextBuffer { get; set; }
    public DynamicConsoleInputFlowIterator Iterator { get; set; }
    public DynamicCommandHandler CommandHandler { get; set; }
    public IOutBuffer OutBuffer { get; set; }
    public IScene CuiScene { get; set; }
    public OutBufferSizeHandler OutBufferSizeHandler { get; set; }
    public DynamicKeyFlowHandler InputFlowHandler { get; set; }
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
        TextBuffer = new TextBuffer.TextBuffers.TextBuffer(SourceCodeBufferHistoryLimit);
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
