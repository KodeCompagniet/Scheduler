namespace DemiCode.Scheduling.Test.Mocks
{
    public static class SchedulerMock
    {
        /// <summary>
        /// Create an empy <see cref="Scheduler"/> with testing lic.
        /// </summary>
        public static Scheduler Create()
        {
            return SchedulerFactory.Create();
        }

        public static Scheduler CreateFromConfig(string configSection)
        {
            return SchedulerFactory.CreateFromConfig(configSection);
        }
    }
}