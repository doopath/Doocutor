using Domain.Core.CommandHandlers;
using Domain.Core.Descriptors;

namespace Domain.Core
{
    public class CommandFlowHandler : IInputFlowHandler
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
            while (inputFlowDescriptor.HasNext())
            {
                commandHandler.Handle(inputFlowDescriptor.Next());
            }

            IsClosed = true;
        }
    }
}
