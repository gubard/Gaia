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

    public StorageService()
    {
        switch (OsHelper.OsType)
        {
            case Os.Windows:
            {
                var appDirectoryPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData
                );

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new(appDirectoryPath);

                    break;
                }

                appDirectoryPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                );

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new(appDirectoryPath);

                    break;
                }

                appDirectoryPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData
                );

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new(appDirectoryPath);

                    break;
                }

                _appDirectory = new("./storage");

                break;
            }
            case Os.Android:
            {
                var appDirectoryPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal
                );
                _appDirectory = new(appDirectoryPath);

                break;
            }
            case Os.MacOs:
            case Os.Linux:
            case Os.Browser:
            case Os.FreeBsd:
            case Os.Ios:
            case Os.MacCatalyst:
            case Os.TvOs:
            case Os.WatchOs:
            case Os.Wasi:
            default:
                throw new ArgumentOutOfRangeException();
        }

        _dbDirectory = new(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Databases")
        );
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
