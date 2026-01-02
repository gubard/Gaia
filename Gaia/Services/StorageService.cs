using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface IStorageService
{
    DirectoryInfo GetAppDirectory();
    DirectoryInfo GetDbDirectory();
}

public class StorageService : IStorageService
{
    private readonly DirectoryInfo _appDirectory;
    private readonly DirectoryInfo _dbDirectory;

    public StorageService(string appName)
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
                    _appDirectory = new DirectoryInfo(appDirectoryPath).Combine(appName);

                    break;
                }

                appDirectoryPath = Environment.SpecialFolder.LocalApplicationData.GetPath();

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new DirectoryInfo(appDirectoryPath).Combine(appName);

                    break;
                }

                appDirectoryPath = Environment.SpecialFolder.CommonApplicationData.GetPath();

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new DirectoryInfo(appDirectoryPath).Combine(appName);

                    break;
                }

                _appDirectory = new("./storage");

                break;
            }
            case Os.Android:
            {
                var appDirectoryPath = Environment.SpecialFolder.Personal.GetPath();
                _appDirectory = new(appDirectoryPath);

                break;
            }
            case Os.Browser:
            case Os.Ios:
            case Os.MacCatalyst:
            case Os.TvOs:
            case Os.WatchOs:
            case Os.Wasi:
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(OsHelper.OsType),
                    OsHelper.OsType,
                    null
                );
        }

        _dbDirectory = Environment
            .SpecialFolder.Personal.GetDir()
            .Combine("Databases")
            .Combine(appName);

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
}
