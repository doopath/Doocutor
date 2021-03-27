
namespace Doocutor.Core.Commands
{
    class EditorCommand : Command
    {
        public CommandType Type { get; private set; } = CommandType.EDITOR_COMMAND;
        public string Content { get; private set; }

        public EditorCommand(string content)
        {
            this.Content = content;
        }
    }
}
