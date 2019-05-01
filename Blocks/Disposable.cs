
namespace Blocks
{
    using System;

    /// <summary>
    /// Provides a mechanism for releasing unmanaged resources for an array of objects implementing 
    /// the IDisposable interface. 
    /// </summary>
    public sealed class Disposable : IDisposable
    {
        /// <summary>
        /// Array of objects implementing the IDisposable interface.
        /// </summary>
        private readonly IDisposable[] _disposables;

        /// <summary>
        /// Initializes a new instance of the Disposable class with an array of objects implementing
        /// the IDisposable interface.
        /// </summary>
        /// <param name="disposables">Array of objects implementing the IDisposable interface.</param>
        public Disposable(params IDisposable[] disposables)
        {
            _disposables = disposables;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        //  unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
