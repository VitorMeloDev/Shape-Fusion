using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool m_musicEnabled = true;
    public bool m_fxEnabled = true;

    [Range(0,1)]
    public float m_musicVolume = 1.0f;
    [Range(0,1)]
    public float m_fxVolume = 1.0f;

    public AudioClip m_clearRowSound;
    public AudioClip m_errorSound;
    public AudioClip m_moveSound;
    public AudioClip m_dropSound;
    public AudioClip m_gameOverSound;
    public AudioClip m_levelUpVocalClip;

    public AudioSource m_musicSource;
    public AudioClip[] m_musicClips;
    public AudioClip[] m_vocalClips;
    public AudioClip m_gameoverVocalClip;

    [Header("Buttons Icons")]
    public IconToggle m_fxToggle;
    public IconToggle m_musicToggle;
    // Start is called before the first frame update
    void Start()
    {
        PlayBackgroundMusic(GetRandomAudioClip(m_musicClips));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if(!m_musicEnabled || !musicClip || !m_musicSource)
        {
            return;
        }

        m_musicSource.Stop();

        m_musicSource.clip = musicClip;

        m_musicSource.volume = m_musicVolume;

        m_musicSource.loop = true;

        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        if(m_musicSource.isPlaying != m_musicEnabled)
        {
            if(m_musicEnabled)
            {
                PlayBackgroundMusic(GetRandomAudioClip(m_musicClips));
            }
            else
            {
                m_musicSource.Stop();
            }
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();
        m_musicToggle.ToggleIcon(m_musicEnabled);
    }

    public void ToggleFX()
    {
        m_fxEnabled = !m_fxEnabled;
        m_fxToggle.ToggleIcon(m_fxEnabled);
    }

    public AudioClip GetRandomAudioClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
