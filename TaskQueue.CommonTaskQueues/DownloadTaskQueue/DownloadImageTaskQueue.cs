using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods;
using Microsoft.Extensions.Logging;
using TaskQueue.CommonTaskQueues.SpiderTaskQueue;

namespace TaskQueue.CommonTaskQueues.DownloadTaskQueue
{
    public class DownloadImageTaskQueue : SpiderTaskQueue<DownloadImageTaskQueueOptions, DownloadImageTaskData>
    {
        protected int FilteredCount;
        protected int SkippedCount;
        protected int DownloadedCount;

        public override object State =>
            string.Format(
                $"[{GetType().Name}]Total: {TotalCount}, Succeed: {SuccessCount}, Downloaded: {DownloadedCount}, Failed: {FailureCount}, Skipped: {SkippedCount}, Filtered: {FilteredCount}, Threads: {CurrentThreadCount}/{Options.MaxThreads}");

        public DownloadImageTaskQueue(DownloadImageTaskQueueOptions options, ILoggerFactory loggerFactory) : base(
            options, loggerFactory)
        {
        }

        protected override async Task<List<TaskData>> ExecuteAsyncInternal(DownloadImageTaskData taskData)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var filename = Path.GetFileName(taskData.RelativeFilename);
            var newFilename = invalidChars.Aggregate(filename, (current, c) => current.Replace(c.ToString(), ""));
            taskData.RelativeFilename = taskData.RelativeFilename.Replace(filename, newFilename);
            var fullname = Path.Combine(Options.DownloadPath, taskData.RelativeFilename);
            if (!File.Exists(fullname))
            {
                var client = await GetHttpClient();
                var imgRsp = await client.GetAsync(taskData.Url);
                if (imgRsp.StatusCode != HttpStatusCode.NotFound)
                {
                    var directory = Path.GetDirectoryName(fullname);
                    // Ensure directory exist
                    Directory.CreateDirectory(directory);
                    var ms = new MemoryStream();
                    await imgRsp.Content.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    if (Options.Filter != null)
                    {
                        if (!await Options.Filter(ms))
                        {
                            Interlocked.Increment(ref FilteredCount);
                            return null;
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                    }
                    using (var fs = File.Create(fullname))
                    {
                        await imgRsp.Content.CopyToAsync(fs);
                        Interlocked.Increment(ref DownloadedCount);
                    }
                }
            }
            else
            {
                Interlocked.Increment(ref SkippedCount);
            }
            return null;
        }
    }
}