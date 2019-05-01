
namespace Blocks
{
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Serilog;

    /// <summary>
    /// Represents a dataflow block, used for workflows and dataflows with an
    /// API used for writing log events.abstract The logging interface is an
    /// implementation of Serilog.ILogger. 
    /// </summary>
    public abstract class LogBlock : IDataflowBlock
    {
        /// <summary>
        /// Gets an instance of a Serilog.ILogger API, used for writing log events.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Initizalizes a new instance of the Q.Data.Blocks.LogBlock class
        /// with a logging API, used for writing log events
        /// </summary>
        /// <param name="logger">Serilog logging API, used for writing log events.</param>
        protected LogBlock(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Signals to the System.Threading.Tasks.Dataflow.IDataflowBlock that it should
        /// not accept nor produce any more messages nor consume any more postponed messages.
        /// </summary>
        public abstract void Complete();

        /// <summary>
        /// Causes the System.Threading.Tasks.Dataflow.IDataflowBlock to complete in a System.Threading.Tasks.TaskStatus.Faulted
        /// state.
        /// </summary>
        public abstract void Fault(Exception exception);

        /// <summary>
        /// Gets a System.Threading.Tasks.Task that represents the asynchronous operation
        /// and completion of the dataflow block.
        /// </summary>
        public abstract Task Completion { get; }
    }
}
