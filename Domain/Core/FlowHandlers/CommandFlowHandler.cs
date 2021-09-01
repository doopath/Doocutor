using Domain.Core.CommandHandlers;
using Domain.Core.Iterators;

namespace Domain.Core
{
    public class CommandFlowHandler : IInputFlowHandler
    {
        private readonly IInputFlowIterator _inputFlowDescriptor;
        private readonly ICommandHandler _commandHandler = new CommandHandler();
        public bool IsClosed { get; private set; } = false;

        public CommandFlowHandler(IInputFlowIterator inputFlowDescriptor)
        {
            _inputFlowDescriptor = inputFlowDescriptor;
        }

        public void Handle()
        {
            while (_inputFlowDescriptor.HasNext())
            {
                _commandHandler.Handle(_inputFlowDescriptor.Next());
            }

            IsClosed = true;
        }
    }
}
