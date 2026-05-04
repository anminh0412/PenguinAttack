namespace Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Data;
    using Other;
    using Services.Interface;
    using UnityEngine;

    public class AudioService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            ServiceLocator.Register<IAudioService>(this);
        }

        private void Start()
        {
            this.CheckToActiveMusic();
        }
        private void OnDestroy() { ServiceLocator.Unregister<IAudioService>(); }

        private Dictionary<string, List<AudioSource>> playingSources = new();

        public void CheckToActiveMusic()
        {
            this.musicSource.volume = !UserDataService.UserData.IsMusic ? 0:1;
        }
        
        public async void PlaySound(string clipId, float volume = 1f, bool loop = false)
        {
            if(!UserDataService.UserData.IsSound) return;
            var clip = await AddressableHelper.LoadAsset<AudioClip>(clipId);

            if (clip == null) return;
            var newSound = this.sfxSource.Spawn();
            newSound.clip   = clip;
            newSound.loop   = loop;
            newSound.volume = volume;
            newSound.Play();

            if (!this.playingSources.ContainsKey(clipId))
                this.playingSources[clipId] = new List<AudioSource>();

            this.playingSources[clipId].Add(newSound);

            await UniTask.WaitForSeconds(clip.length);

            this.StopSound(clipId, newSound);
        }

        public void StopSound(string clipId, AudioSource target = null)
        {
            if (!this.playingSources.TryGetValue(clipId, out var list)) return;

            if (target != null)
            {
                target.Stop();
                target.Despawn();
                list.Remove(target);
            }
            else
            {
                foreach (var sound in list)
                {
                    sound.Stop();
                    sound.Despawn();
                }

                list.Clear();
            }
        }

        public async void PlayMusic(string clipId, float volume = 1f, bool loop = true)
        {
            var clip = await AddressableHelper.LoadAsset<AudioClip>(clipId);

            if (clip == null) return;
            this.musicSource.clip   = clip;
            this.musicSource.loop   = loop;
            this.musicSource.volume = volume;
            this.musicSource.Play();
        }

        public void StopAllSounds()
        {
            foreach (var sound in this.playingSources.Values.SelectMany(sounds => sounds))
            {
                sound.Stop();
                sound.Despawn();
            }

            this.playingSources.Clear();
            this.musicSource.Stop();
        }

        public void StopMusic() { this.musicSource.Stop(); }
    }
}