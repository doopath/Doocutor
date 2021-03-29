using Doocutor.Core.Descriptors;

namespace Doocutor.Core
{
    class CommandFlowHandler : IInputFlowHandler
    {
        private readonly IInputFlowDescriptor inputFlowDescriptor;
        private readonly ICommandHandler commandHandler = new CommandHandler();
        public bool IsClosed { get; private set; } = false;

        public CommandFlowHandler(IInputFlowDescriptor inputFlowDescriptor)
        {
            this.inputFlowDescriptor = inputFlowDescriptor;
        }

        public void Handle()
        {
            while (this.inputFlowDescriptor.HasNext())
            {
                this.commandHandler.Handle(this.inputFlowDescriptor.Next());
            }

            this.IsClosed = true;
        }
    }
}
