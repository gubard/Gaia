using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IStorageService
{
    DirectoryInfo GetAppDirectory();
    DirectoryInfo GetDbDirectory();
}

public sealed class StorageService : IStorageService
{
    private readonly DirectoryInfo _appDirectory;
    private readonly DirectoryInfo _dbDirectory;

    public StorageService(string appName)
    {
#if DEBUG
        _appDirectory = CreateAppDirectory(appName).Combine("Debug");
#else
        _appDirectory = CreateAppDirectory(appName);
#endif
        _dbDirectory = CreateDbDirectory(appName);

        if (!_appDirectory.Exists)
        {
            _appDirectory.Create();
        }

        if (!_dbDirectory.Exists)
        {
            _dbDirectory.Create();
        }
    }

    public DirectoryInfo GetAppDirectory()
    {
        return _appDirectory;
    }

    public DirectoryInfo GetDbDirectory()
    {
        return _dbDirectory;
    }

    private DirectoryInfo CreateDbDirectory(string appName)
    {
        return OsHelper.OsType switch
        {
            Os.Windows
            or Os.MacOs
            or Os.Linux
            or Os.Browser
            or Os.FreeBsd
            or Os.iOS
            or Os.MacCatalyst
            or Os.TvOs
            or Os.WatchOs
            or Os.Wasi => Environment
                .SpecialFolder.UserProfile.GetDir()
                .Combine("Databases")
                .Combine(appName),
            Os.Android => Environment
                .SpecialFolder.Personal.GetDir()
                .Combine("Databases")
                .Combine(appName),
            _ => throw new ArgumentOutOfRangeException(
                nameof(OsHelper.OsType),
                OsHelper.OsType,
                $"Specified {OsHelper.OsType} argument {nameof(OsHelper.OsType)} was out of the range of valid values."
            ),
        };
    }

    private DirectoryInfo CreateAppDirectory(string appName)
    {
        switch (OsHelper.OsType)
        {
            case Os.MacOs:
            case Os.FreeBsd:
            case Os.Linux:
            case Os.Windows:
            {
                var appDirectoryPath = Environment.SpecialFolder.ApplicationData.GetPath();

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    return new DirectoryInfo(appDirectoryPath).Combine(appName);
                }

                appDirectoryPath = Environment.SpecialFolder.LocalApplicationData.GetPath();

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    return new DirectoryInfo(appDirectoryPath).Combine(appName);
                }

                appDirectoryPath = Environment.SpecialFolder.CommonApplicationData.GetPath();

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    return new DirectoryInfo(appDirectoryPath).Combine(appName);
                }

                return AppDomain.CurrentDomain.BaseDirectory.ToDir().Combine("storage");
            }
            case Os.Android:
            {
                var appDirectoryPath = Environment.SpecialFolder.Personal.GetPath();

                return new(appDirectoryPath);
            }
            case Os.Browser:
            case Os.iOS:
            case Os.MacCatalyst:
            case Os.TvOs:
            case Os.WatchOs:
            case Os.Wasi:
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(OsHelper.OsType),
                    OsHelper.OsType,
                    $"Specified {OsHelper.OsType} argument {nameof(OsHelper.OsType)} was out of the range of valid values."
                );
        }
    }
}
