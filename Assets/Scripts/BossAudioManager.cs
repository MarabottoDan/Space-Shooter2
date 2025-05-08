using System.Collections;
using UnityEngine;

public class BossAudioManager : MonoBehaviour
{
    [Header("Boss Growls")]
    [SerializeField] private AudioClip _bossBigGrowl;
    [SerializeField] private AudioClip _bossLoudGrowl;
    [SerializeField] private AudioClip _bossFinalGrowl;
    [SerializeField] private AudioClip _bossSpit;
    [SerializeField] private AudioClip _bossShortGrowl;

    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSource; // Reference to audio source used to play clips

    private int _hitCounter = 0; // Tracks how many times the boss has been hit
    private int _nextGrowlHitCount = 0; // Random number of hits before the next short growl
    private bool _canTrackHits = true; // Controls the 5-second cooldown between growl triggers

    private void Start()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        // Start by picking the first random hit count (between 5 and 10)
        ResetNextGrowlHit();
    }

    // Call this when the boss is hit by a projectile
    public void RegisterHit()
    {
        if (!_canTrackHits) return;

        _hitCounter++;

        // If the hit counter reaches the growl threshold, play the short growl
        if (_hitCounter >= _nextGrowlHitCount)
        {
            PlayShortGrowl();
            _hitCounter = 0;
            _canTrackHits = false;
            Invoke(nameof(EnableHitTracking), 5f); // Wait 5 seconds before tracking hits again
        }
    }

    // Re-enable hit tracking after cooldown
    private void EnableHitTracking()
    {
        _canTrackHits = true;
        ResetNextGrowlHit();
    }

    // Randomize the next growl hit count 
    private void ResetNextGrowlHit()
    {
        _nextGrowlHitCount = Random.Range(5, 11);
    }

    // === Specific Event Sounds ===

    public void PlaySpawnGrowl()
    {
        PlaySound(_bossBigGrowl);
    }

    public void PlayHelmetBreakGrowl()
    {
        PlaySound(_bossLoudGrowl);
    }

    public void PlayFinalGrowl()
    {
        PlaySound(_bossFinalGrowl);
    }

    public void PlaySpitGrowl()
    {
        PlaySound(_bossSpit);
    }

    public void PlayShortGrowl()
    {
        PlaySound(_bossShortGrowl);
    }

    // === Utility ===

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip, 2f);
        }
    }
}
