using Domain.Core;
using Domain.Core.Iterators;
using DynamicEditor.Core.Iterators;

namespace DynamicEditor
{
    public class DynamicEditorSetup : EditorSetup
    {
        public void Run(string[] args)
        {
            IInputFlowIterator iterator = new DynamicConsoleInputFlowIterator();
            IInputFlowHandler handler = new CommandFlowHandler(iterator);
            handler.StartHandling();
        }
    }
}
