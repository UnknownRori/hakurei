using SDL3;

namespace Hakurei.Engine.Audio;

public class Music : IDisposable
{
    private static uint _audioId = 0;
    private uint _id;
    private nint _audio;

    public nint audio { get => _audio; }

    public Music(AudioMixer mixer, string filename)
    {
        _audio = Mixer.LoadAudio(mixer.mixer, filename, false);
        if (_audio == nint.Zero) Logger.Fatal("Audio", $"Music failed to load: {SDL.GetError()}");

        Logger.Log("Audio", $"Music loaded ({_id}) {filename}");
    }

    public void Dispose()
    {
        if (_audio == nint.Zero) return;
        Mixer.DestroyAudio(_audio);
        _audio = nint.Zero;
        Logger.Log("Audio", $"AudioMixer Destroyed ({_id})");
    }
}
