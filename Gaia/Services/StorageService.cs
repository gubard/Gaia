using Gaia.Helpers;
using Gaia.Models;
using Microsoft.Extensions.Logging;

namespace Gaia.Services;

public interface IStorageService
{
    DirectoryInfo GetAppConfigDirectory();
    DirectoryInfo GetDbDirectory();
    DirectoryInfo GetAppDictionary();
}

public sealed class StorageService : IStorageService
{
    private readonly DirectoryInfo _appDirectory;
    private readonly DirectoryInfo _dbDirectory;
    private readonly DirectoryInfo _appConfigDirectory;

    public StorageService(string appName, ILogger<StorageService> logger)
    {
        _appDirectory = new DirectoryInfo(AppContext.BaseDirectory);

#if DEBUG
        _appConfigDirectory = CreateAppConfigDirectory(appName).Combine("Debug");
#else
        _appConfigDirectory = CreateAppConfigDirectory(appName);
#endif
        _dbDirectory = CreateDbDirectory(appName);

        if (!_appConfigDirectory.Exists)
        {
            _appConfigDirectory.Create();
        }

        if (!_dbDirectory.Exists)
        {
            _dbDirectory.Create();
        }

        logger.InitAppDirectory(_appDirectory);
        logger.InitDbDirectory(_dbDirectory);
        logger.InitConfigDirectory(_appConfigDirectory);
    }

    public DirectoryInfo GetAppConfigDirectory()
    {
        return _appConfigDirectory;
    }

    public DirectoryInfo GetDbDirectory()
    {
        return _dbDirectory;
    }

    public DirectoryInfo GetAppDictionary()
    {
        return _appDirectory;
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

    private DirectoryInfo CreateAppConfigDirectory(string appName)
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
