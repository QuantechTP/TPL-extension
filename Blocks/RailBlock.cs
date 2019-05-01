namespace Blocks
{
    using System;
    using System.Threading.Tasks.Dataflow;
    using Serilog;
    using Channels;
    using System.Linq;

    public abstract class RailBlock<TInput, TOutput>
        : CustomPropagatorBlock<Flow<TInput>, Flow<TOutput>>, IDataflowBlock, IPropagatorBlock<Flow<TInput>, Flow<TOutput>>
    {
        public OutputBlock OutputBlock { get; }

        /// <summary>
        /// Initizalizes a new instance of the CustomPropagatorBlock class with a
        /// logging API, used for writing log events and with an OutputBlock, used
        /// for error handling of a workflow.
        /// </summary>
        /// <param name="log">The Serilog.ILogging API, used for writing log events.</param>
        public RailBlock(OutputBlock outputBlock)
            : base(outputBlock.Logger)
        {
            this.OutputBlock = outputBlock;
        }

        /// <summary>
        /// Signals to the System.Threading.Tasks.Dataflow.IDataflowBlock that it should
        //  not accept nor produce any more messages nor consume any more postponed messages.
        /// </summary>
        /// <param name="target">The System.Threading.Tasks.Dataflow.ITargetBlock`1 to which to connect this source.</param>
        /// <param name="linkOptions">A System.Threading.Tasks.Dataflow.DataflowLinkOptions instance that configures the link.</param>
        /// <param name="outputBlock">The error block where </param>
        /// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>
        public override IDisposable LinkTo(
            ITargetBlock<Flow<TOutput>> target,
            DataflowLinkOptions linkOptions)
            => new Disposable(
                Source.LinkTo(target, linkOptions, flow => flow.Success),
                Source.LinkTo(OutputBlock, linkOptions, flow => flow.Failure));

        public IDisposable ForkTo(
            ITargetBlock<Flow<TOutput>> target,
            Func<TOutput, bool> selector)
            => ForkTo(target, new DataflowLinkOptions(), selector);

        public IDisposable ForkTo(
            ITargetBlock<Flow<TOutput>> target,
            DataflowLinkOptions linkOptions,
            Func<TOutput, bool> selector)
        {
            var block = new FlowBlock<TOutput, TOutput>(
                (f, l) => f,
                OutputBlock);

            return new Disposable(
                Source.LinkTo(target, linkOptions, flow => flow.Success && selector(flow.Data)),
                Source.LinkTo(block, linkOptions, flow => flow.Success && !selector(flow.Data)),
                Source.LinkTo(OutputBlock, linkOptions, flow => flow.Failure),
                block.LinkTo(OutputBlock, linkOptions));
        }

        public IDisposable LinkTo(ITargetBlock<Flow<TOutput>> target)
            => Source.LinkTo(target);
    }
}
