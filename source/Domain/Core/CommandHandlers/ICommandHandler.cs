namespace Domain.Core.CommandHandlers
{
    public interface ICommandHandler
    {
        void Handle(string command);
    }
}
