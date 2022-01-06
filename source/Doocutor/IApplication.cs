using Common;
using Common.Options;
using CUI;
using CUI.ColorSchemes;
using CUI.OutBuffers;
using CUI.Scenes;
using InputHandling;
using InputHandling.CommandHandlers;
using InputHandling.FlowHandlers;
using InputHandling.Iterators;

namespace Doocutor;

public interface IApplication
{
    void Run(AppOptions options);
    
    int BufferSizeUpdateRate { get; set; }
    int SourceTextBufferHistoryLimit { get; set; }
    ITextBuffer TextBuffer { get; set; }
    DynamicConsoleInputFlowIterator Iterator { get; set; }
    DynamicCommandHandler CommandHandler { get; set; }
    IOutBuffer OutBuffer { get; set; }
    IScene CuiScene { get; set; }
    OutBufferSizeHandler OutBufferSizeHandler { get; set; }
    DynamicKeyFlowHandler InputFlowHandler { get; set; }
    IColorScheme ColorScheme { get; set; }
    ColorSchemesRepository ColorSchemesRepository { get; set; }
}
