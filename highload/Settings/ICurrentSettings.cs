namespace highload.Settings
{
    public interface ICurrentSettings
    {
        int NumberOfThreads { get; }

        int MinimumWordLength { get; }
    }
}