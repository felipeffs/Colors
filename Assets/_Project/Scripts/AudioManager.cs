using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<Audio> audioList;

    [Header("Audio Sources")]
    [SerializeField] private Dictionary<Sound, AudioSource> audioSourceDictionary = new Dictionary<Sound, AudioSource>();

    public enum Sound
    {
        Swap,
        Die,
        Walk,
        Jump
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
                }
            }

        }
    }

}

[System.Serializable]
struct Audio
{
    public AudioManager.Sound sound;
    public AudioClip audioClip;
}
