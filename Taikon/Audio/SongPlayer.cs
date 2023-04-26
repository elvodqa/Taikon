using System;
using ManagedBass;

namespace Taikon.Audio;

public class SongPlayer
{
    private int _streamHandle;
    private long _songLength;

    public bool IsPlaying { get; private set; }

    public void Load(string filePath)
    {
        Bass.Init();
        _streamHandle = Bass.CreateStream(filePath, 0, 0, BassFlags.Default);

        if (_streamHandle != 0)
        {
            // Set up a callback function to monitor the playback status
            Bass.ChannelSetSync(_streamHandle, SyncFlags.End, 0, (handle, channel, data, user) =>
            {
                IsPlaying = false;
            }, IntPtr.Zero);
            
            _songLength = Bass.ChannelGetLength(_streamHandle, PositionFlags.Bytes);
            _songLength = (long)(Bass.ChannelBytes2Seconds(_streamHandle, _songLength) * 1000);
        }
        else
        {
            throw new Exception("Failed to load the song file.");
        }
    }

    public void Play()
    {
        if (_streamHandle != 0)
        {
            Bass.ChannelPlay(_streamHandle, false);
            IsPlaying = true;
        }
        else
        {
            throw new Exception("No song loaded.");
        }
    }

    public void Pause()
    {
        if (_streamHandle != 0)
        {
            Bass.ChannelPause(_streamHandle);
            IsPlaying = false;
        }
        else
        {
            throw new Exception("No song loaded.");
        }
    }

    public void Stop()
    {
        if (_streamHandle != 0)
        {
            Bass.ChannelStop(_streamHandle);
            IsPlaying = false;
        }
        else
        {
            throw new Exception("No song loaded.");
        }
    }

    public void Unload()
    {
        if (_streamHandle != 0)
        {
            Bass.StreamFree(_streamHandle);
            IsPlaying = false;
        }
        Bass.Free();
    }

    public void SetPosition(long milliseconds)
    {
        if (_streamHandle != 0)
        {
            // Convert the time in milliseconds to bytes and set the channel position ??????
            long bytes = Bass.ChannelSeconds2Bytes(_streamHandle, milliseconds / 1000.0);
            Bass.ChannelSetPosition(_streamHandle, bytes);
        }
        else
        {
            throw new Exception("No song loaded.");
        }
    }

    public long GetPosition()
    {
        if (_streamHandle != 0)
        {
            // Get the current position of the channel in bytes and convert to milliseconds ??????
            long bytes = Bass.ChannelGetPosition(_streamHandle, PositionFlags.Bytes);
            return (long)(Bass.ChannelBytes2Seconds(_streamHandle, bytes) * 1000);
        }
        else
        {
            throw new Exception("No song loaded.");
        }
    }

    public long GetLength()
    {
        return _songLength;
    }
}
