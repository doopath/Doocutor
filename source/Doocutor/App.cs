using CommandHandling;
using Common;
using Common.Options;
using CUI;
using CUI.ColorSchemes;
using CUI.OutBuffers;
using CUI.Scenes;
using CUI.Widgets;
using InputHandling;
using InputHandling.CommandHandlers;
using InputHandling.FlowHandlers;
using InputHandling.Iterators;
using TextBuffer;
using Utils.Exceptions;
using Utils.Exceptions.NotExitExceptions;

namespace Doocutor;

public class App : IApplication
{
    public int BufferSizeUpdateRate { get; set; }
    public int SourceTextBufferHistoryLimit { get; set; }
    public ITextBuffer TextBuffer { get; set; }
    public DynamicConsoleInputFlowIterator Iterator { get; set; }
    public DynamicCommandHandler CommandHandler { get; set; }
    public IOutBuffer OutBuffer { get; set; }
    public IScene CuiScene { get; set; }
    public OutBufferSizeHandler OutBufferSizeHandler { get; set; }
    public DynamicKeyFlowHandler InputFlowHandler { get; set; }
    public IColorScheme ColorScheme { get; set; }
    public ColorSchemesRepository ColorSchemesRepository { get; set; }

    public App()
    {
        ColorScheme = new DefaultDarkColorScheme();
        ColorSchemesRepository = new();
        ColorSchemesRepository.Add(Settings.ColorScheme);
        ColorSchemesRepository.Add(new DefaultDarkColorScheme());

        BufferSizeUpdateRate = 100;
        SourceTextBufferHistoryLimit = 10000;
        TextBuffer = new TextBuffer.TextBuffers.TextBuffer(SourceTextBufferHistoryLimit);

        EditorCommands.InitializeCodeBuffer(TextBuffer);

        Iterator = new DynamicConsoleInputFlowIterator();
        CommandHandler = new DynamicCommandHandler();
        OutBuffer = new StandardConsoleOutBuffer();
        CuiScene = new CuiScene();
        
        Settings.ColorScheme = new DefaultLightColorScheme();
        Settings.OutBuffer = OutBuffer;

        CuiRender.Scene = CuiScene;
        CuiRender.OutBuffer = OutBuffer;
        CuiRender.TextBuffer = TextBuffer;

        WidgetsMount.Scene = CuiScene;
        WidgetsMount.OutBuffer = OutBuffer;

        OutBufferSizeHandler = new OutBufferSizeHandler(
            OutBuffer,
            BufferSizeUpdateRate);

        InputFlowHandler = new DynamicKeyFlowHandler(
            Iterator,
            CommandHandler,
            KeyCombinationsMap.Map);
    }

    public void Run(AppOptions options)
    {
        Configure(options);

        CuiRender.Clear();
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

    private void Configure(AppOptions options)
    {
        try
        {
            Settings.ColorScheme = ColorSchemesRepository.Get(options.ColorScheme);
        }
        catch (ColorSchemeWasNotFoundException error)
        {
            Settings.ColorScheme = ColorScheme;
            WidgetsMount.Mount(new AlertWidget(error.Message, AlertLevel.ERROR));
        }
        
        OutBuffer!.CursorVisible = false;

        if (options.IsDeveloperMonitorEnabled)
            WidgetsMount.Mount(new DeveloperMonitorWidget());

        OutBufferSizeHandler.UpdateRate = options.OutBufferSizeHandlerUpdateRate;
        TextBuffer.HistoryLimit = options.TextBufferHistoryLimit;
    }
}
