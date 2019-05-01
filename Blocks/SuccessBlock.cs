
namespace Blocks
{
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Channels;
    using Serilog;

    public sealed class SuccessBlock : LogBlock, ITargetBlock<IFlow>
    {
        /// <summary>
        /// The target block of the ITargetBlock interface.
        /// </summary>
        private ITargetBlock<IFlow> Target { get; }
        public SuccessBlock(
            ILogger logger,
            ExecutionDataflowBlockOptions options,
            Action<IFlow> successHandler)
            : base(logger)
            => this.Target = new ActionBlock<IFlow>(
                successHandler,
                options);

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
