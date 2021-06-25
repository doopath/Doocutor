
namespace Doocutor.Core.Commands
{
    internal interface ICommand
    {
        CommandType Type { get; }
        string Content { get; }
    }
}
