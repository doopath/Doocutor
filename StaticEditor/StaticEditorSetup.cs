using Domain.Core;
using Domain.Core.Descriptors;

namespace StaticEditor
{
    public class StaticEditorSetup : EditorSetup
    {
        public void Run(string[] args)
        {
            IInputFlowDescriptor descriptor = new StaticConsoleInputFlowDescriptor();
            IInputFlowHandler handler = new CommandFlowHandler(descriptor);
            handler.Handle();
        }
    }
}
