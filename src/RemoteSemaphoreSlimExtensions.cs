﻿using OwlCore.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Remoting
{
    public static partial class RemoteSemaphoreSlimExtensions
    {
        /// <summary>
        /// Provides syntactic sugar for releasing a <see cref="RemoteSemaphoreSlim"/> when execution leaves a <c>using</c> statement.
        /// </summary>
        /// <remarks>
        /// This will also enter / release the semaphore on all remote instances of <see cref="RemoteSemaphoreSlim"/>, keeping the semaphore in sync.
        /// <para/>
        /// Due to the pubsub nature of OwlCore.Remoting, releasing happens on ALL listening nodes.
        /// Not taking this into account may cause undesired behavior.
        /// </remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. When this task completes, the semaphore has entered (locally). Value is a disposable wrapper around the semaphore that calls <see cref="SemaphoreSlim.Release()"/> when disposed.</returns>
        public static async Task<DisposeToReleaseRemoteSemaphoreWrapper> DisposableWaitAsync(this RemoteSemaphoreSlim semaphore)
        {
            var wrapper = new DisposeToReleaseRemoteSemaphoreWrapper(semaphore);
            await semaphore.WaitAsync();
            return wrapper;
        }

        /// <summary>
        /// A wrapper that disposes the given semaphore when disposed. Used for <see cref="DisposableWaitAsync(RemoteSemaphoreSlim)"/>.
        /// </summary>
        public class DisposeToReleaseRemoteSemaphoreWrapper : IDisposable
        {
            private readonly RemoteSemaphoreSlim _semaphore;

            /// <summary>
            /// Creates a new instance of <see cref="SemaphoreSlimExtensions.DisposeToReleaseSemaphoreWrapper"/>.
            /// </summary>
            /// <param name="semaphore">The semaphore to wrap around and release when disposed.</param>
            public DisposeToReleaseRemoteSemaphoreWrapper(RemoteSemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            /// <summary>
            /// Called to release the semaphore.
            /// </summary>
            public void Dispose() => _semaphore.Release();
        }
    }
}
