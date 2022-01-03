namespace CommandHandling.CommandHandlers
{
    public interface ICommandHandler
    {
        void Handle(string command);
    }
}
