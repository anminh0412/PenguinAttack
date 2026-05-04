namespace Services
{
    using System;
    using Services.Interface;
    using UnityEngine;

    public class DummyAudioService : MonoBehaviour, IAudioService
    {
        public void PlaySound(string clipId, float volume = 1, bool loop = false) { throw new NotImplementedException(); }

        public void PlayMusic(string clipId, float volume = 1, bool loop = true) { throw new NotImplementedException(); }

        public void StopSound(string clipId, AudioSource target = null) { throw new NotImplementedException(); }

        public void StopAllSounds() { throw new NotImplementedException(); }

        public void StopMusic()          { throw new NotImplementedException(); }
        public void CheckToActiveMusic() { }
    }
}