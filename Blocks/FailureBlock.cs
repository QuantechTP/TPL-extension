
namespace Blocks
{
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Serilog;
    using Channels;
    using Logging;
    using System.Threading;

    /// <summary>
    /// This DataflowBlock is part of an implementation of the Railway Oriented 
    /// Programming concept which splits any workflow into two parallel flows, 
    /// one for successful and one for failures (Exceptions). The FailureBlock
    /// is the receiving block of flows which encountered an exception or error.
    /// <see href="https://fsharpforfunandprofit.com/rop/"/>
    /// </summary>
    public sealed class FailureBlock : LogBlock, ITargetBlock<IFlow>
    {
        /// <summary>
        /// The target block of the ITargetBlock interface.
        /// </summary>
        private ITargetBlock<IFlow> Target { get; }

        /// <summary>
        /// Initizalizes a new instance of the Q.Data.Blocks.FailureBlock class
        /// with a logging API, used for writing log events
        /// </summary>
        /// <param name="log">Serilog logging API, used for writing log events.</param>
        public FailureBlock(ILogger log, ExecutionDataflowBlockOptions options)
            : base(log)
        {
            ChannelResult = new ChannelResult();
            Target = new ActionBlock<IFlow>(
                flow =>
                {
                    if (flow.Exception != null)
                    {
                        flow.Exception.LogTo(Logger);
                    }
                },
                options);
        }

        /// <summary>
        /// Asynchronously passes a message to the target block, giving the target the 
        /// opportunity to consume the message.
        /// </summary>
        DataflowMessageStatus ITargetBlock<IFlow>.OfferMessage(
            DataflowMessageHeader messageHeader,
            IFlow messageValue,
            ISourceBlock<IFlow> source,
            bool consumeToAccept)
            => Target.OfferMessage(
                messageHeader,
                messageValue,
                source,
                consumeToAccept);

        /// <summary>
        /// Gets a Task that represents the completion of this dataflow block.
        /// </summary>
        public override Task Completion => Target.Completion;

        /// <summary>
        /// Signals to this target block that it should not accept any more messages, 
        /// nor consume postponed messages. 
        /// </summary>
        public override void Complete() => Target.Complete();

        /// <summary>
        /// The Dataflow's faulted or not state is the target block's state.
        /// </summary>
        public override void Fault(Exception error) => Target.Fault(error);
    }
}
