using Domain.Core;
using Domain.Core.Iterators;
using StaticEditor.Core.Iterators;

namespace StaticEditor
{
    public class StaticEditorSetup : EditorSetup
    {
        public void Run(string[] args)
        {
            IInputFlowIterator descriptor = new StaticConsoleInputFlowIterator();
            IInputFlowHandler handler = new CommandFlowHandler(descriptor);
            handler.Handle();
        }
    }
}
