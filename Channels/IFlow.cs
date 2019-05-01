
namespace Channels
{
    using Logging;

    /// <summary>
    /// Interface for dataflows and workflows. Flow wraps the concepts of error and success 
    /// handling into one class. Any workflow or dataflow is run through a DataChannel 
    /// in conjunction with the .NET TPL dataflow library. Each Flow is handled both errors
    /// and successes on two parallel railways as explained in the concept of Railway 
    /// Oriented Programming. 
    /// </summary>
    public interface IFlow
    {
        /// <summary>
        /// Gets the success status of the flow.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Gets the failure status of the flow.
        /// </summary>
        bool Failure { get; }

        /// <summary>
        /// Gets an exception which can be logged.
        /// </summary>
        LoggableException Exception { get; }

        /// <summary>
        /// Fails the flow to be processed by an FailureBlock with a loggable exception.
        /// </summary>
        void Fail(LoggableException exception);
    }
}
