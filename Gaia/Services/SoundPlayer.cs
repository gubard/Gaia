using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Gaia.Helpers;
using Gaia.Models;

namespace Gaia.Services;

public interface ISoundPlayer
{
    ConfiguredValueTaskAwaitable PlayAsync(
        ReadOnlyMemory<byte> soundData,
        bool isLooping,
        CancellationToken ct
    );
}

public sealed class SoundPlayer : ISoundPlayer
{
    public SoundPlayer()
    {
        _soundPlayer = OsHelper.OsType switch
        {
            Os.Windows => new WindowsSoundPlayer(),
            Os.Linux => new LinuxSoundPlayer(),
            _ => new EmptySoundPlayer(),
        };
    }

    public ConfiguredValueTaskAwaitable PlayAsync(
        ReadOnlyMemory<byte> soundData,
        bool isLooping,
        CancellationToken ct
    )
    {
        return _soundPlayer.PlayAsync(soundData, isLooping, ct);
    }

    private readonly ISoundPlayer _soundPlayer;
}

public sealed class EmptySoundPlayer : ISoundPlayer
{
    public ConfiguredValueTaskAwaitable PlayAsync(
        ReadOnlyMemory<byte> soundData,
        bool isLooping,
        CancellationToken ct
    )
    {
        return TaskHelper.ConfiguredCompletedTask;
    }
}

public sealed class LinuxSoundPlayer : ISoundPlayer
{
    public ConfiguredValueTaskAwaitable PlayAsync(
        ReadOnlyMemory<byte> soundData,
        bool isLooping,
        CancellationToken ct
    )
    {
        if (snd_pcm_open(out var pcm, "default", SND_PCM_STREAM_PLAYBACK, 0) < 0)
        {
            throw new("Failed to open PCM device.");
        }

        using var reg = ct.Register(() =>
        {
            try
            {
                snd_pcm_drop(pcm);
            }
            catch
            {
                // best-effort
            }
        });

        try
        {
            do
            {
                if (
                    snd_pcm_set_params(
                        pcm,
                        SND_PCM_FORMAT_S16_LE,
                        SND_PCM_ACCESS_RW_INTERLEAVED,
                        2,
                        44100,
                        1,
                        500000
                    ) < 0
                )
                {
                    throw new("Failed to set PCM parameters.");
                }

                const int bytesPerFrame = 4;
                const int chunkFrames = 2048;
                var chunkBytes = chunkFrames * bytesPerFrame;
                var tmp = new byte[chunkBytes];
                var remaining = soundData;

                while (remaining.Length > 0)
                {
                    ct.ThrowIfCancellationRequested();
                    var take = Math.Min(tmp.Length, remaining.Length);
                    remaining.Span.Slice(0, take).CopyTo(tmp);
                    remaining = remaining.Slice(take);
                    var framesToWrite = take / bytesPerFrame;

                    if (framesToWrite <= 0)
                    {
                        break;
                    }

                    var written = snd_pcm_writei(pcm, tmp, framesToWrite);

                    if (written < 0)
                    {
                        snd_pcm_prepare(pcm);
                    }
                }

                ct.ThrowIfCancellationRequested();
                snd_pcm_drain(pcm);
            } while (isLooping);
        }
        catch (OperationCanceledException)
        {
            snd_pcm_drop(pcm);

            throw;
        }
        finally
        {
            snd_pcm_close(pcm);
        }

        return TaskHelper.ConfiguredCompletedTask;
    }

    private const int SND_PCM_STREAM_PLAYBACK = 0;
    private const int SND_PCM_FORMAT_S16_LE = 2;
    private const int SND_PCM_ACCESS_RW_INTERLEAVED = 3;

    [DllImport("libasound.so")]
    private static extern int snd_pcm_prepare(nint pcm);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_open(out nint pcm, string name, int stream, int mode);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_drop(nint pcm);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_set_params(
        nint pcm,
        int format,
        int access,
        int channels,
        uint rate,
        int soft_resample,
        uint latency
    );

    [DllImport("libasound.so")]
    private static extern int snd_pcm_writei(nint pcm, byte[] buffer, int size);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_drain(nint pcm);

    [DllImport("libasound.so")]
    private static extern int snd_pcm_close(nint pcm);
}

public class WindowsSoundPlayer : ISoundPlayer
{
    public delegate void WaveOutProc(
        nint hwo,
        uint uMsg,
        nint dwInstance,
        nint dwParam1,
        nint dwParam2
    );

    private const int MMSYSERR_NOERROR = 0;
    private const int CALLBACK_NULL = 0;
    private const int WAVE_FORMAT_PCM = 1;
    private const int WHDR_DONE = 0x00000001;

    public ConfiguredValueTaskAwaitable PlayAsync(
        ReadOnlyMemory<byte> soundData,
        bool isLooping,
        CancellationToken ct
    )
    {
        return PlayCore(soundData, isLooping, ct).ConfigureAwait(false);
    }

    private async ValueTask PlayCore(
        ReadOnlyMemory<byte> soundData,
        bool isLooping,
        CancellationToken ct
    )
    {
        do
        {
            using var options = new WaveHeaderOptions(soundData.Span);
            var result = waveOutWrite(
                options.HWaveOut,
                options.Header.Handle,
                (uint)Marshal.SizeOf(options.Header.Value)
            );

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to write waveform audio data.");
            }

            while ((options.Header.Value.dwFlags & WHDR_DONE) != WHDR_DONE)
            {
                await Task.Delay(100, ct);
            }
        } while (isLooping);
    }

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutOpen(
        out nint hWaveOut,
        uint uDeviceID,
        ref WaveFormatEx lpFormat,
        WaveOutProc? dwCallback,
        nint dwInstance,
        uint dwFlags
    );

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutPrepareHeader(nint hWaveOut, nint lpWaveOutHdr, uint uSize);

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutWrite(nint hWaveOut, nint lpWaveOutHdr, uint uSize);

    [DllImport("winmm.dll", SetLastError = true)]
    public static extern int waveOutClose(nint hWaveOut);

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveHeader
    {
        public nint lpData;
        public uint dwBufferLength;
        public uint dwBytesRecorded;
        public nint dwUser;
        public uint dwFlags;
        public uint dwLoops;
        public nint lpNext;
        public nint reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveFormatEx
    {
        public ushort wFormatTag;
        public ushort nChannels;
        public uint nSamplesPerSec;
        public uint nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;
    }

    private struct WaveHeaderOptions : IDisposable
    {
        private GCHandle handle;

        public WaveHeaderOptions(ReadOnlySpan<byte> soundData)
        {
            var waveFormat = new WaveFormatEx
            {
                wFormatTag = WAVE_FORMAT_PCM,
                nChannels = 2, // 2 for stereo, 1 for mono
                nSamplesPerSec = 44100, // 44.1 kHz sample rate
                wBitsPerSample = 16, // 16-bit samples
                nBlockAlign = 2 * 16 / 8,
                nAvgBytesPerSec = 44100 * 2 * 16 / 8,
                cbSize =
                    0 // No extra information
                ,
            };

            var result = waveOutOpen(
                out HWaveOut,
                0xFFFFFFFF,
                ref waveFormat,
                null,
                nint.Zero,
                CALLBACK_NULL
            );

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to open wave output device.");
            }

            // Prepare the wave header
            handle = GCHandle.Alloc(soundData.ToArray(), GCHandleType.Pinned);

            var header = new WaveHeader
            {
                lpData = handle.AddrOfPinnedObject(),
                dwBufferLength = (uint)soundData.Length,
                dwFlags = 0,
            };

            Header = new(header);
            result = waveOutPrepareHeader(
                HWaveOut,
                Header.Handle,
                (uint)Marshal.SizeOf(Header.Value)
            );

            if (result == MMSYSERR_NOERROR)
            {
                return;
            }

            Dispose();

            throw new("Failed to prepare wave header.");
        }

        public readonly nint HWaveOut;
        public MarshalRef<WaveHeader> Header;

        public void Dispose()
        {
            handle.Free();
            Header.Dispose();
            var result = waveOutClose(HWaveOut);

            if (result != MMSYSERR_NOERROR)
            {
                throw new("Failed to close waveform.");
            }
        }
    }
}
