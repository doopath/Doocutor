namespace Utils;

public static class Info
{
    /// <summary>
    /// Build version variants:
    /// U / S - unstable / stable,
    /// F / C - full / changes,
    /// B - build,
    /// DDMMYY - DayMonthYear datetime
    /// </summary>
    public const string BuildInfo = "UCB-100122";
    public const string Updated = "10th of January 2022";
    public const string Version = "0.5.29.0";
    public const string ProductName = "Doocutor";
    public const string BuildType = "Debug";

    public static string DoocutorInfo
        => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
           "Free open source software by Doopath. 2022.\n" +
           "Try :help (or --help option) command for help list, for a description of the Doocutor or :quit for quitting.";

    public static string Description
        => $"Doocutor v{Version}. Build: {BuildInfo}.\n" +
           $"Last update: {Updated}.\n" +
           "It's a terminal text editor written in C# by Doopath.";

    public static void ShowDoocutorInfo()
        => Console.WriteLine(DoocutorInfo);
}
