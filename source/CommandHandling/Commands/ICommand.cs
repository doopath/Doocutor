namespace CommandHandling.Commands
{
    public interface ICommand
    {
        CommandType Type { get; }
        string Content { get; }
    }
}
