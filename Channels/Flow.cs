
namespace Channels
{
    using System;
    using Logging;

    /// <summary>
    /// Represents a dataflow or workflow. Flow wraps the concepts of error and success 
    /// handling into one class. Any workflow or dataflow is run through a DataChannel 
    /// in conjunction with the .NET TPL dataflow library. Each Flow is handled both errors
    /// and successes on two parallel railways as explained in the concept of Railway 
    /// Oriented Programming. 
    /// </summary>
    public class Flow : IFlow
    {
        /// <summary>
        /// Gets the success status of the flow.
        /// </summary>
        public bool Success => Exception is null;

        /// <summary>
        /// Gets the failure status of the flow.
        /// </summary>
        public bool Failure => !Success;

        /// <summary>
        /// Gets or sets an exception which can be logged.
        /// </summary>
        public LoggableException Exception { get; private set; }

        /// <summary>
        /// Fails the flow to be processed by an FailureBlock with a loggable exception.
        /// </summary>
        public void Fail(LoggableException exception)
        {
            Exception = exception;
        }

        public Flow()
        { }

        public Flow(LoggableException exception)
        {
            Fail(exception);
        }

        public static Flow<T> FromValue<T>(T data)
            => new Flow<T>(data);
    }

    /// <summary>
    /// Represents a dataflow or workflow. Flow wraps the concepts of error and success 
    /// handling into one class. Any workflow or dataflow is run through a DataChannel 
    /// in conjunction with the .NET TPL dataflow library. Each Flow is handled both errors
    /// and successes on two parallel railways as explained in the concept of Railway 
    /// Oriented Programming. 
    /// </summary>
    public class Flow<T> : Flow
    {
        /// <summary>
        /// Gets or sets data passed to the workflow or dataflow..
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the Flow class using the data
        /// passed to the workflow or dataflow.
        /// </summary>
        public Flow(T value) => Data = value;

        public Flow(LoggableException exception)
            : base(exception)
        { }
    }
}
