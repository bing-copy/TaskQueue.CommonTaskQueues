using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProxyProvider;

namespace TaskQueue.CommonTaskQueues.SpiderTaskQueue
{
    public abstract class SpiderTaskQueue<TOptions, TTaskData> : TaskQueue<TOptions, TTaskData>
        where TOptions : SpiderTaskQueueOptions where TTaskData : TaskData
    {
        private readonly HttpClientProvider _httpClientProvider;

        protected SpiderTaskQueue(TOptions options, ILoggerFactory loggerFactory) : this(options,
            new HttpClientProvider(options.HttpClientProviderDbConnectionString), loggerFactory)
        {
        }

        protected SpiderTaskQueue(TOptions options, HttpClientProvider httpClientProvider, ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
            _httpClientProvider = httpClientProvider;
        }

        protected virtual async Task<HttpClient> GetHttpClient()
        {
            return await _httpClientProvider.GetClient(Options.Purpose);
        }
    }
}