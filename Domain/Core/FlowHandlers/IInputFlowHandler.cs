namespace Domain.Core.FlowHandlers
{
    /// <summary>
    /// InputFlowHandler uses an InputFlowDescriptor
    /// (for example: that may be console or some file)
    /// to read and handle every containing command.
    /// </summary>
    public interface IInputFlowHandler
    {
        bool IsClosed { get; }
        void StartHandling();
    }
}
