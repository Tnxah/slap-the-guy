using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> damageSounds;

    [SerializeField]
    private List<AudioClip> punchSounds;

    [SerializeField]
    private List<AudioClip> fartSounds;

    [SerializeField]
    private AudioSource audioSource;

    private Random rnd = new Random();

    public void PlayDamageSound()
    {
        audioSource.clip = damageSounds[rnd.Next(damageSounds.Count)];
        audioSource.Play();
    }

    public void PlayPunchSound() 
    {
        audioSource.clip = punchSounds[rnd.Next(punchSounds.Count)];
        audioSource.Play();
    }

    public void PlayFartSound()
    {
        audioSource.clip = fartSounds[rnd.Next(fartSounds.Count)];
        audioSource.Play();
    }
}
