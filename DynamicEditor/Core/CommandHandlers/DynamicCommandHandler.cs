using Domain.Core.CommandHandlers;
using System.Collections.Generic;

namespace DynamicEditor.Core.CommandHandlers
{
    public sealed class DynamicCommandHandler : ICommandHandler
    {
        private readonly KeyCombinationTranslating _keyCombinationTranslating;
        private readonly Dictionary<string, string> _keyCombinationsMap;

        public DynamicCommandHandler()
        {
            _keyCombinationTranslating = new KeyCombinationTranslating(_keyCombinationsMap);
        }

        public void Handle(string command)
        {

        }
    }
}
