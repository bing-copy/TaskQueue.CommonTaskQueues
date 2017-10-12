namespace TaskQueue.CommonTaskQueues.DownloadTaskQueue
{
    public class DownloadImageTaskData : TaskData
    {
        public string Url { get; set; }
        public string RelativeFilename { get; set; }
    }
}
