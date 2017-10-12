using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaskQueue.CommonTaskQueues.SpiderTaskQueue
{
    public class SpiderTaskQueueOptions : TaskQueueOptions
    {
        public string Purpose { get; set; }
        public string HttpClientProviderDbConnectionString { get; set; }
    }
}
