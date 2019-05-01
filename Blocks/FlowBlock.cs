namespace Blocks
{
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Serilog;
    using Channels;
    using Logging;

    public class FlowBlock<TInput, TOutput>
        : RailBlock<TInput, TOutput>, IDataflowBlock, IPropagatorBlock<Flow<TInput>, Flow<TOutput>>
    {
        /// <summary>
        /// The target block component of the IPropagatorBlock interface.
        /// </summary>
        protected override ITargetBlock<Flow<TInput>> Target => TransformBlock;

        /// <summary>
        /// The source block component of the IPropagatorBlock interface.
        /// </summary>
        protected override ISourceBlock<Flow<TOutput>> Source => TransformBlock;

        private TransformBlock<Flow<TInput>, Flow<TOutput>> TransformBlock { get; }

        private static ExecutionDataflowBlockOptions DefaultOptions => new ExecutionDataflowBlockOptions();

        public FlowBlock(
            Func<Flow<TInput>, ILogger, Task<Flow<TOutput>>> transform,
            ExecutionDataflowBlockOptions executionDataflowBlockOptions,
            OutputBlock outputBlock)
            : this(outputBlock)
        {
            TransformBlock = new TransformBlock<Flow<TInput>, Flow<TOutput>>(f => transform(f, this.Logger), executionDataflowBlockOptions);
        }

        public FlowBlock(
            Func<Flow<TInput>, ILogger, Flow<TOutput>> transform,
            OutputBlock outputBlock)
            : this(transform, DefaultOptions, outputBlock)
        { }

        public FlowBlock(
            Func<Flow<TInput>, ILogger, Flow<TOutput>> transform,
            ExecutionDataflowBlockOptions dataflowBlockOptions,
            OutputBlock outputBlock)
            : this(outputBlock)
        {
            this.TransformBlock = new TransformBlock<Flow<TInput>, Flow<TOutput>>(f => transform(f, this.Logger), dataflowBlockOptions);
        }

        public FlowBlock(
            Func<TInput, ILogger, Task<TOutput>> transform,
            Func<Exception, LoggableException> exceptionHandler,
            OutputBlock outputBlock)
            : this(transform, exceptionHandler, new ExecutionDataflowBlockOptions(), outputBlock)
        { }

        public FlowBlock(
            Func<TInput, ILogger, Task<TOutput>> transform,
            Func<Exception, LoggableException> exceptionHandler,
            ExecutionDataflowBlockOptions dataflowBlockOptions,
            OutputBlock outputBlock)
            : this(outputBlock)
        {
            TransformBlock = new TransformBlock<Flow<TInput>, Flow<TOutput>>(
                async inFlow =>
                {
                    try
                    {
                        return new Flow<TOutput>(await transform(inFlow.Data, this.Logger));
                    }
                    catch (Exception exception)
                    {
                        return new Flow<TOutput>(exceptionHandler(exception));
                    }
                },
                dataflowBlockOptions);
        }

        /// <summary>
        //// Initizalizes a new instance of the FlowBlock class with a logging API, 
        /// used for writing log events and with an OutputBlock, used for error handling
        /// of a workflow.
        /// </summary>
        /// <param name="log">The Serilog.ILogging API, used for writing log events.</param>
        private FlowBlock(OutputBlock outputBlock)
            : base(outputBlock)
        { }
    }
}
