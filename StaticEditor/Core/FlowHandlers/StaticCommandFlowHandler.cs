using Domain.Core.CommandHandlers;
using Domain.Core.Iterators;
using Domain.Core.FlowHandlers;
using StaticEditor.Core.CommandHandlers;

namespace StaticEditor.Core.FlowHandlers
{
    public class StaticCommandFlowHandler : IInputFlowHandler
    {
        private readonly IInputFlowIterator _inputFlowIterator;
        private readonly ICommandHandler _commandHandler;
        public bool IsClosed { get; private set; } = false;

        public StaticCommandFlowHandler(IInputFlowIterator inputFlowDescriptor, ICommandHandler commandHandler)
        {
            _inputFlowIterator = inputFlowDescriptor;
            _commandHandler = commandHandler;
        }

        public void StartHandling()
        {
            while (_inputFlowIterator.HasNext())
                _commandHandler.Handle(_inputFlowIterator.NextLine());

            IsClosed = true;
        }
    }
}
