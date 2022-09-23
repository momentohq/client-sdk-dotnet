using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Momento.Sdk.Internal.Middleware
{
    // A very simplistic implementation of a semaphore that is based on .NET
    // async channels.  This is used for our MaxConcurrentRequestsMiddleware.
    // Channel read and write requests exhibit some degree
    // of fairness, which ensures that we don't starve requests if the
    // user exceeds the maximum number of concurrent requests.
    internal class FairAsyncSemaphore
    {
        private readonly Channel<bool> _ticketChannel;

        internal FairAsyncSemaphore(int numTickets)
        {
            _ticketChannel = Channel.CreateBounded<bool>(numTickets);

            for (var i = 0; i < numTickets; i++)
            {
                bool success = _ticketChannel.Writer.TryWrite(true);
                if (!success)
                {
                    throw new ApplicationException("Unable to initialize async channel");
                }
            }
        }

        public async Task WaitOne()
        {
            await _ticketChannel.Reader.ReadAsync();
        }

        public void Release()
        {
            var balanced = _ticketChannel.Writer.TryWrite(true);
            if (!balanced)
            {
                throw new ApplicationException("more releases than waits! These must be 1:1");
            }
        }
    }
}

