using System.Collections.Generic;
using UnityEngine;

// Singleton audio manager. Caches clips in a dictionary and uses
// dictionary lookup for tag-to-sound mapping.
public class AudioSourceManager : Singleton<AudioSourceManager>
{
    private static GameObject _audioObj;
    private static AudioSource _persistentAudioSource;
    private static AudioSource _themeMusicSource;
    private static AudioSource _tempAudioSource;

    private static readonly Dictionary<string, AudioClip> ClipCache = new();

    private static readonly Dictionary<string, string> TagToSound = new()
    {
        { "Player", "register" },
        { "Target", "success" },
        { "Ground", "failure" },
        { "Birds", "chirping" },
        { "Cloud", "thunder" },
        { "GetShield", "shieldAcquired" },
        { "UseShield", "shieldUsed" },
        { "UseBoost", "boostUsed" },
        { "GetBoost", "boostAcquired" }
    };

    void Start()
    {
        _audioObj = new GameObject("AudioObject");
        _persistentAudioSource = _audioObj.AddComponent<AudioSource>();
        _themeMusicSource = _audioObj.AddComponent<AudioSource>();
        SetThemeAudioClip(.75f);
        if (LevelManager.currentLevel == Level.TitleMenu) return;
        SetLevelPersistentAudioClip();
        _tempAudioSource = _audioObj.AddComponent<AudioSource>();
    }

    private static AudioClip GetClip(string clipName)
    {
        if (ClipCache.TryGetValue(clipName, out var cached))
            return cached;

        var clip = Resources.Load<AudioClip>(clipName);
        if (clip != null)
            ClipCache[clipName] = clip;

        return clip;
    }

    private static void PlayTempSound(string soundName)
    {
        var shieldClip = GetClip("shieldUsed");
        var boostClip = GetClip("boostUsed");

        bool isShieldActive = _tempAudioSource.clip == shieldClip && _tempAudioSource.isPlaying;
        bool isBoostActive = _tempAudioSource.clip == boostClip && _tempAudioSource.isPlaying;
        bool shouldPlay = (!isShieldActive && !isBoostActive) || soundName == "crash";

        if ((isShieldActive || isBoostActive)
            && soundName is "register" or "success" or "failure"
                or "boostAcquired" or "shieldAcquired"
                or "boostUsed" or "shieldUsed")
        {
            var simultaneousAudioSource = _audioObj.AddComponent<AudioSource>();
            simultaneousAudioSource.clip = GetClip(soundName);
            simultaneousAudioSource.Play();
            Destroy(simultaneousAudioSource, simultaneousAudioSource.clip.length);
            return;
        }

        if (!shouldPlay) return;

        _tempAudioSource.clip = GetClip(soundName);
        _tempAudioSource.Play();
    }

    public static void PlaySound(string tag)
    {
        string soundName = TagToSound.TryGetValue(tag, out var mapped) ? mapped : tag;
        PlayTempSound(soundName);
    }

    private static void StopAudio()
    {
        if (_persistentAudioSource)
            _persistentAudioSource.Stop();
        if (_tempAudioSource)
            _tempAudioSource.Stop();
    }

    public static void PauseAudio()
    {
        if (_persistentAudioSource && _persistentAudioSource.isPlaying)
            _persistentAudioSource.Pause();
        if (_themeMusicSource && _themeMusicSource.isPlaying)
            _themeMusicSource.Pause();
    }

    private static void SetLevelPersistentAudioClip(float volume = 1f)
    {
        StopAudio();

        string clipName = LevelManager.currentLevel switch
        {
            Level.FantasyVillage => "engine_toy_plane",
            Level.MedievalVillage => "engine_medieval_plane",
            Level.FuturisticWorld => "engine_spacecraft",
            _ => null
        };

        if (clipName == null) return;

        _persistentAudioSource.clip = GetClip(clipName);
        _persistentAudioSource.loop = true;
        _persistentAudioSource.volume = volume;
        _persistentAudioSource.Play();
    }

    private static void SetThemeAudioClip(float volume = 1f)
    {
        StopAudio();

        string clipName = LevelManager.currentLevel switch
        {
            Level.TitleMenu => "theme_main_menu",
            Level.FantasyVillage => "theme_level_1",
            Level.MedievalVillage => "theme_level_2",
            Level.FuturisticWorld => "theme_level_3",
            _ => null
        };

        if (clipName == null) return;

        _themeMusicSource.clip = GetClip(clipName);
        _themeMusicSource.loop = true;
        _themeMusicSource.volume = volume;
        _themeMusicSource.Play();
    }

    public static void PlayPersistentAudio()
    {
        if (_persistentAudioSource)
            _persistentAudioSource.Play();
        if (_themeMusicSource && !_themeMusicSource.isPlaying)
            _themeMusicSource.Play();
    }

    public static void ResumeAudio()
    {
        if (_persistentAudioSource && !_persistentAudioSource.isPlaying)
            _persistentAudioSource.UnPause();
        if (_themeMusicSource && !_themeMusicSource.isPlaying)
            _themeMusicSource.UnPause();
    }
}
