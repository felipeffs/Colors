using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<Audio> audioList;

    [Header("Audio Sources")]
    [SerializeField] private Dictionary<Sound, AudioSource> audioSourceDictionary = new Dictionary<Sound, AudioSource>();

    private float volume_sound;

    protected override void SingletonAwake()
    {
        print("AAAA");
        volume_sound = (float)PlayerPrefs.GetInt("volume_sound") / 100;
        print(PlayerPrefs.GetInt("volume_sound"));
    }

    public enum Sound
    {
        None,
        Swap,
        Die,
        Walk,
        Jump,
        Landing
    }

    public void PlayAudio(Sound sound)
    {
        if (audioSourceDictionary.ContainsKey(sound))
            audioSourceDictionary[sound].Play();
        else
        {
            foreach (var audio in audioList)
            {
                if (audio.sound == sound)
                {
                    var newAudioSource = gameObject.AddComponent<AudioSource>();
                    newAudioSource.clip = audio.audioClip;
                    audioSourceDictionary.Add(sound, newAudioSource);
                    audioSourceDictionary[sound].Play();

                    if (sound == Sound.Walk)
                    {
                        audioSourceDictionary[sound].volume = .3f * volume_sound;
                        audioSourceDictionary[sound].loop = true;
                    }

                    if (sound == Sound.Swap)
                    {
                        audioSourceDictionary[sound].volume = .6f * volume_sound;
                    }

                    if (sound == Sound.Landing || sound == Sound.Jump)
                    {
                        audioSourceDictionary[sound].volume = .3f * volume_sound;
                    }
                }
            }

        }
    }

    public void StopAudio(Sound sound)
    {
        if (audioSourceDictionary.ContainsKey(sound))
            audioSourceDictionary[sound].Stop();
    }

}

[System.Serializable]
struct Audio
{
    public AudioManager.Sound sound;
    public AudioClip audioClip;
}
