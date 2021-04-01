using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    internal class NativeCommandExecutor : ICommandExecutor<NativeCommand>
    {
        public void Execute(NativeCommand command)
        {
            NativeCommander.GetExecutingFunction(command)();
        }
    }
}
