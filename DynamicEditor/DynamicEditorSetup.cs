using Domain.Core;
using Domain.Core.ColorSchemes;
using Domain.Core.CommandHandlers;
using Domain.Core.Exceptions;
using Domain.Core.FlowHandlers;
using Domain.Core.Iterators;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Core.TextBuffers;
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
    public uint? SourceCodeBufferHistoryLimit { get; set; }
    public ITextBuffer? TextBuffer { get; set; }
    public IInputFlowIterator? Iterator { get; set; }
    public ICommandHandler? CommandHandler { get; set; }
    public IOutBuffer? OutBuffer { get; set; }
    public IScene? CuiScene { get; set; }
    public CuiRender? Render { get; set; }
    public OutBufferSizeHandler? OutBufferSizeHandler { get; set; }
    public IInputFlowHandler? InputFlowHandler { get; set; }
    public IColorScheme? ColorScheme { get; set; }
    public ColorSchemesRepository? ColorSchemesRepository { get; set; }

    public DynamicEditorSetup()
    {
        var defaultLightColorScheme = new DefaultLightColorScheme();

        ColorSchemesRepository = new();
        ColorSchemesRepository.Add(defaultLightColorScheme);
        ColorSchemesRepository.Add(new DefaultDarkColorScheme());

        BufferSizeUpdateRate = 300;
        SourceCodeBufferHistoryLimit = 10000;
        TextBuffer = new TextBuffer(SourceCodeBufferHistoryLimit!.Value);
        ITextBuffer sourceCodeBuffer = TextBuffer;
        EditorCommandsList.InitializeCodeBuffer(ref sourceCodeBuffer);
        Iterator = new DynamicConsoleInputFlowIterator();
        CommandHandler = new DynamicCommandHandler();
        OutBuffer = new StandardConsoleOutBuffer();
        CuiScene = new CuiScene();
        Render = new CuiRender(
                TextBuffer,
                OutBuffer,
                CuiScene,
                defaultLightColorScheme);

        OutBufferSizeHandler = new OutBufferSizeHandler(
                OutBuffer,
                Render,
                BufferSizeUpdateRate!.Value);

        InputFlowHandler = new DynamicKeyFlowHandler(
                Iterator,
                CommandHandler,
                KeyCombinationsMap.Map, Render);
    }

    public void Run(ProgramOptions options)
    {
        ColorScheme = ColorSchemesRepository!
            .Get(options.ColorScheme ?? "DoocutorDark");
        OutBuffer!.CursorVisible = false;
        Render!.ColorScheme = ColorScheme;

#if DEBUG
        Render.EnableDeveloperMonitor();
#endif

        Render.Render();
        OutBufferSizeHandler!.Start();

        try
        {
            InputFlowHandler!.StartHandling();
        }
        catch (InterruptedExecutionException)
        {
            OutBufferSizeHandler.Stop();
            OutBuffer.CursorVisible = true;

            throw;
        }
    }
}
