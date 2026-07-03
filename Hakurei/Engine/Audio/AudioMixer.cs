using SDL3;

namespace Hakurei.Engine.Audio;

public class AudioMixer : IDisposable
{
    private static uint _audioMixerId = 0;
    private uint _id;
    private nint _mixer;

    public nint mixer { get => _mixer; }

    public AudioMixer(uint deviceId)
    {
        _mixer = Mixer.CreateMixerDevice(deviceId, nint.Zero);
        if (_mixer == nint.Zero)
            Logger.Fatal("Audio", $"AudioMixer failed to initialized: {SDL.GetError()}");

        _id = _audioMixerId++;
        Logger.Log("Audio", $"AudioMixer created ({_id})");
    }

    public void Play(Sound sound)
    {
        Mixer.PlayAudio(_mixer, sound.audio);
    }

    public void Play(Music sound)
    {
        Mixer.PlayAudio(_mixer, sound.audio);
    }

    public void Stop(long fade = 0)
    {
        Mixer.StopAllTracks(_mixer, fade);
    }

    public void Dispose()
    {
        if (_mixer == nint.Zero) return;
        Mixer.DestroyMixer(_mixer);
        _mixer = nint.Zero;
        Logger.Log("Audio", $"AudioMixer Destroyed ({_id})");
    }
}
