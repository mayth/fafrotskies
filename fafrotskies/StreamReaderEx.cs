using System;
using System.Threading;
using System.Threading.Tasks;

namespace fafrotskies
{
    public static class StreamReaderEx
    {
        public static async Task<string> ReadLineAsync(
            this System.IO.StreamReader reader,
            int timeoutMilliseconds,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(
                () =>
                {
                    var readTask = reader.ReadLineAsync();
                    try
                    {
                        var hasResult = readTask.Wait(timeoutMilliseconds, cancellationToken);
                        if (hasResult)
                            return readTask.Result;
                        else
                            return null;
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                });
        }
    }
}

