using System;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    private static GameObject _audioObj;
    private static AudioSource _persistentAudioSource;      // main flying sound
    private static AudioSource _themeMusicSource;
    private static AudioSource _tempAudioSource;  // for temporary one-shots

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

    private static void PlayTempSound(string soundName)
    {
        bool isShieldActive = _tempAudioSource.clip == Resources.Load<AudioClip>("shieldUsed") && _tempAudioSource.isPlaying,
            isBoostActive = _tempAudioSource.clip == Resources.Load<AudioClip>("boostUsed") && _tempAudioSource.isPlaying,
            shouldPlay = (!isShieldActive && !isBoostActive) || soundName == "crash";

        if ((isShieldActive || isBoostActive) 
            && soundName is "register" or "success" or "failure" 
                or "boostAcquired" or "shieldAcquired" 
                or "boostUsed" or "shieldUsed")
        {
            var simultaneousAudioSource = _audioObj.AddComponent<AudioSource>();
            simultaneousAudioSource.clip = Resources.Load<AudioClip>(soundName);
            simultaneousAudioSource.Play();
            Destroy(simultaneousAudioSource, simultaneousAudioSource.clip.length);
            return;
        }

        if (!shouldPlay) return;

        _tempAudioSource.clip = Resources.Load<AudioClip>(soundName);
        _tempAudioSource.Play();
    }

    public static void PlaySound(string tag)
    {
        string soundName = tag switch
        {
            "Player" => "register",
            "Target" => "success",
            "Ground" => "failure",
            "Birds" => "chirping",
            "Cloud" => "thunder",
            "GetShield" => "shieldAcquired",
            "UseShield" => "shieldUsed",
            "UseBoost" => "boostUsed",
            "GetBoost" => "boostAcquired",
            _ => tag
        };
        PlayTempSound(soundName);
    }
    
    private static void StopAudio()
    {
        if (_persistentAudioSource)
            _persistentAudioSource.Stop();
        if(_tempAudioSource)
            _tempAudioSource.Stop();
    }

    // Pause continuous flying sound
    public static void PauseAudio()
    {
        if (_persistentAudioSource && _persistentAudioSource.isPlaying)
            _persistentAudioSource.Pause();
        if (_themeMusicSource && _themeMusicSource.isPlaying)
            _themeMusicSource.Pause();
    }

    private static void SetLevelPersistentAudioClip(float volume = 1f)
    {
        switch (LevelManager.currentLevel)
        {
            case Level.FantasyVillage:
                StopAudio();
                _persistentAudioSource.clip = Resources.Load<AudioClip>("engine_toy_plane");
                break;
            case Level.MedievalVillage:
                StopAudio();
                _persistentAudioSource.clip = Resources.Load<AudioClip>("engine_medieval_plane");
                break;
            case Level.FuturisticWorld:
                StopAudio();
                _persistentAudioSource.clip = Resources.Load<AudioClip>("engine_spacecraft");
                break;
        }
        _persistentAudioSource.loop = true;
        _persistentAudioSource.volume = volume;
        _persistentAudioSource.Play();
    }

    private static void SetThemeAudioClip(float volume = 1f)
    {
        StopAudio();
        switch (LevelManager.currentLevel)
        {
            case Level.TitleMenu:
                _themeMusicSource.clip = Resources.Load<AudioClip>("theme_main_menu");
                break;
            case Level.FantasyVillage:
                _themeMusicSource.clip = Resources.Load<AudioClip>("theme_level_1");
                break;
            case Level.MedievalVillage:
                _themeMusicSource.clip = Resources.Load<AudioClip>("theme_level_2");
                break;
            case Level.FuturisticWorld:
                _themeMusicSource.clip = Resources.Load<AudioClip>("theme_level_3");
                break;
        }
        _themeMusicSource.loop = true;
        _themeMusicSource.volume = volume;
        _themeMusicSource.Play();
    }

    // Play flying sound
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
