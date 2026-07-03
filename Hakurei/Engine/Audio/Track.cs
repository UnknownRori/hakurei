using SDL3;
using System.Security.Cryptography;
namespace Hakurei.Engine.Audio;

public class Track : IDisposable
{
    private static uint _audioId = 0;
    private uint _id;
    nint _track;

    public Track(AudioMixer mixer)
    {
        _track = Mixer.CreateTrack(mixer.mixer);
        if (_track == nint.Zero) Logger.Fatal("Audio", $"Sound failed to load: {SDL.GetError()}");

        Logger.Log("Audio", $"Track created ({_id})");
    }

    public void Play()
    {
        uint options = SDL.CreateProperties();
        Mixer.PlayTrack(_track, options);
        SDL.DestroyProperties(options);
    }

    public void Stop(long fadeout = 0)
    {
        Mixer.StopTrack(_track, fadeout);
    }

    public void SetAudio(Sound snd)
    {
        Mixer.SetTrackAudio(_track, snd.audio);
        
    }

    public void SetAudio(Music snd)
    {
        Mixer.SetTrackAudio(_track, snd.audio);
    }

    public void Dispose()
    {
        if (_track == nint.Zero) return;
        Mixer.DestroyTrack(_track);
        _track = nint.Zero;
        Logger.Log("Audio", $"AudioMixer Destroyed ({_id})");
    }
}
