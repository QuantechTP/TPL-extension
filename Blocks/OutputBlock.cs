
namespace Blocks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Channels;
    using Serilog;

    public class OutputBlock : ITargetBlock<IFlow>
    {
        public FailureBlock FailureBlock { get; }
        public SuccessBlock SuccessBlock { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public ILogger Logger { get; }
        public OutputBlock(
            ILogger logger,
            Action<IFlow> successHandler)
        {
            CancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var options = new ExecutionDataflowBlockOptions
            {
                CancellationToken = CancellationTokenSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
            };
            FailureBlock = new FailureBlock(logger, options);
            SuccessBlock = new SuccessBlock(logger, options, successHandler);
            Logger = logger;
        }
        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader,
            IFlow messageValue,
            ISourceBlock<IFlow> source,
            bool consumeToAccept)
        {
            if (messageValue.Success)
            {
                return (SuccessBlock as ITargetBlock<IFlow>)
                    .OfferMessage(messageHeader, messageValue, source, consumeToAccept);
            }
            else
            {
                return (FailureBlock as ITargetBlock<IFlow>)
                    .OfferMessage(messageHeader, messageValue, source, consumeToAccept);
            }
        }

        void IDataflowBlock.Complete()
        {
            SuccessBlock.Complete();
            FailureBlock.Complete();
        }

        public Task Completion => Task.WhenAll(SuccessBlock.Completion, FailureBlock.Completion);

        void IDataflowBlock.Fault(Exception exception)
        {
            SuccessBlock.Fault(exception);
            FailureBlock.Fault(exception);
        }
    }
}
