using System;
using System.IO;
using System.Threading.Tasks;
using TaskQueue.CommonTaskQueues.SpiderTaskQueue;

namespace TaskQueue.CommonTaskQueues.DownloadTaskQueue
{
    public class DownloadImageTaskQueueOptions : SpiderTaskQueueOptions
    {
        public string DownloadPath { get; set; }
        public Func<Stream, Task<bool>> Filter { get; set; }
    }
}