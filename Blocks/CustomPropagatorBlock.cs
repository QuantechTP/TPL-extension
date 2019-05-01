
namespace Blocks
{
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Serilog;

    /// <summary>
    /// Interface for custom propagator blocks. The source for this was heavily inspired from
    /// the official Dataflow documentation.
    /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/walkthrough-creating-a-custom-dataflow-block-type"/>
    /// </summary>
    public abstract class CustomPropagatorBlock<TInput, TOutput> : LogBlock, IDataflowBlock, IPropagatorBlock<TInput, TOutput>
    {
        /// <summary>
        /// Instance of an ITargetBlock used in the implementation of IPropagatorBlock of 
        /// the current class.
        /// </summary>
        protected abstract ITargetBlock<TInput> Target { get; }

        /// <summary>
        /// Instance of an ITargetBlock used in the implementation of IPropagatorBlock of 
        /// the current class.
        /// </summary>
        protected abstract ISourceBlock<TOutput> Source { get; }

        /// <summary>
        /// Initizalizes a new instance of the Q.Data.Blocks.CustomPropagatorBlock 
        /// class with a logging API, used for writing log events
        /// </summary>
        /// <param name="log">Serilog logging API, used for writing log events.</param>
        protected CustomPropagatorBlock(ILogger log)
            : base(log)
        { }

        /// <summary>
        /// Links this dataflow block to the provided target.
        /// </summary>
        public virtual IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
            => Source.LinkTo(target, linkOptions);

        /// <summary>
        /// Called by a target to reserve a message previously offered by a source 
        /// but not yet consumed by this target.
        /// </summary>
        bool ISourceBlock<TOutput>.ReserveMessage(
            DataflowMessageHeader messageHeader,
            ITargetBlock<TOutput> target)
            => Source.ReserveMessage(messageHeader, target);

        /// <summary>
        /// Called by a target to consume a previously offered message from a source.
        /// </summary>
        TOutput ISourceBlock<TOutput>.ConsumeMessage(
            DataflowMessageHeader messageHeader,
            ITargetBlock<TOutput> target, out bool messageConsumed)
            => Source.ConsumeMessage(
                messageHeader,
                target,
                out messageConsumed);

        /// <summary>
        /// Called by a target to release a previously reserved message from a source.
        /// </summary>
        void ISourceBlock<TOutput>.ReleaseReservation(
            DataflowMessageHeader messageHeader,
            ITargetBlock<TOutput> target)
            => Source.ReleaseReservation(messageHeader, target);

        /// <summary>
        /// Passes a message to the target block, giving the target the 
        /// opportunity to consume the message.
        /// </summary>
        DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(
            DataflowMessageHeader messageHeader,
            TInput messageValue,
            ISourceBlock<TInput> source,
            bool consumeToAccept)
            => Target.OfferMessage(
                messageHeader,
                messageValue,
                source,
                consumeToAccept);

        /// <summary>
        /// Gets a Task that represents the completion of this dataflow block.
        /// </summary>
        public override Task Completion => Source.Completion;

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
