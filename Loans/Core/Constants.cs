namespace ePACSLoans.Core
{
    /// <summary>
    /// Framework constants for configuration values
    /// </summary>
    public static class FrameworkConstants
    {
        // Timeouts (in milliseconds)
        public const int DefaultTimeout = 5000;
        public const int ShortTimeout = 2000;
        public const int LongTimeout = 10000;
        public const int PageLoadTimeout = 30000;

        // Video Settings
        public const int VideoWidth = 1280;
        public const int VideoHeight = 720;

        // Retry Settings
        public const int DefaultRetryAttempts = 3;
        public const int DefaultRetryDelay = 1000;

        // Path Constants
        public const string TestReportsFolder = "TestReports";
        public const string AllureResultsFolder = "allure-results";
        public const string AllureReportFolder = "allure-report";
        public const string TestExecutionVideosFolder = "TestExecutionVideos";
        public const string TestOutputReportFolder = "TestOutputReport";
        public const string ResourcesFolder = "Resources";
        public const string TestDataFolder = "TestData";
        public const string ElementLocatorsFolder = "ElementLocaters";
        public const string TesseractOcrFolder = "TesseractOCR";

        // File Extensions
        public const string JsonExtension = ".json";
        public const string VideoExtension = ".webm";

        // Configuration Files
        public const string AppSettingsFile = "appsettings.json";
        public const string AppSettingsDevelopmentFile = "appsettings.Development.json";
        public const string AppSettingsStagingFile = "appsettings.Staging.json";
        public const string AppSettingsProductionFile = "appsettings.Production.json";
    }
}


