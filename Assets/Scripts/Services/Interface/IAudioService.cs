namespace Services.Interface
{
    using UnityEngine;

    public interface IAudioService
    {
        void PlaySound(string clipId, float volume = 1f, bool loop = false);
        void PlayMusic(string clipId, float volume = 1f, bool loop = true);

        void StopSound(string clipId, AudioSource target = null);
        void StopAllSounds();
        void StopMusic();
        void CheckToActiveMusic();
    }
}