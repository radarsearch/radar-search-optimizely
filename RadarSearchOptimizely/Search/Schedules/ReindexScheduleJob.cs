using EPiServer.Logging;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using RadarSearchOptimizely.Search.Contracts;

namespace RadarSearchOptimizely.Search.Schedules
{
    [ScheduledPlugIn(DisplayName = "[RADAR] Reindekseringsjobb", GUID = "65bb45a9-1f4c-4aca-b13b-a404d73b6be4", Description = "")]
    public class ReindexingScheduleJob : ScheduledJobBase
    {
        private bool _stopSignaled;
        private readonly IRadarContentService _radarContentService;
        private readonly ISearchService _searchService;

        public ReindexingScheduleJob(IRadarContentService radarContentService, ISearchService searchService)
        {
            _radarContentService = radarContentService;
            _searchService = searchService;
            IsStoppable = true;
        }

        public ReindexingScheduleJob()
        {
            IsStoppable = true;
        }

        public override void Stop()
        {
            _stopSignaled = true;
        }

        public override string Execute()
        {
            OnStatusChanged($"Starting execution of {GetType()}");

            var allData = Task.Run(() => _radarContentService.ParseAllData())
                .GetAwaiter()
                .GetResult();

            if (allData == null || !allData.Any())
            {
                LogManager.GetLogger(typeof(ReindexingScheduleJob)).Error("Parsed data is null or empty, nothing to send to RadarStandalone");
                return "No data to index";
            }

            OnStatusChanged("Sending data to RadarStandalone");
            var status = Task.Run(() => _searchService.SendIndexData(allData))
                .GetAwaiter()
                .GetResult();

            if (_stopSignaled)
            {
                return "Stop of job was called";
            }

            return $"Completed. Status: {status}";
        }
    }
}
