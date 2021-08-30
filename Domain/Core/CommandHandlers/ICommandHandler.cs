namespace Domain.Core
{
    public interface ICommandHandler
    {
        void Handle(string command);
    }
}
