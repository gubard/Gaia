using Gaia.Enums;
using Gaia.Extensions;
using Gaia.Helpers;

namespace Gaia.Services;

public interface IStorageService
{
    DirectoryInfo GetAppDirectory();
}

public class StorageService : IStorageService
{
    private readonly DirectoryInfo _appDirectory;

    public StorageService()
    {
        switch (OsHelper.OsType)
        {
            case Os.Windows:
            {
                var appDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new DirectoryInfo(appDirectoryPath);

                    break;
                }

                appDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new DirectoryInfo(appDirectoryPath);

                    break;
                }

                appDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                if (!appDirectoryPath.IsNullOrWhiteSpace())
                {
                    _appDirectory = new DirectoryInfo(appDirectoryPath);

                    break;
                }

                _appDirectory = new DirectoryInfo("./storage");

                break;
            }
            case Os.Android:
            {
                var appDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                _appDirectory = new DirectoryInfo(appDirectoryPath);

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
    }

    public DirectoryInfo GetAppDirectory()
    {
        return _appDirectory;
    }
}