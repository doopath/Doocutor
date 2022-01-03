using CommandHandling.Commands;

namespace CommandHandling.CommandRecognizers
{
    public interface ICommandRecognizer
    {
        ICommand Recognize(string command);
        ICommand? TryRecognize(string command);
    }
}
