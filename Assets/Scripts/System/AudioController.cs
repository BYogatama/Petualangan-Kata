using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioFX
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup mixer;

    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public class Music
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup mixer;
    public bool playOnAwake;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioController : MonoBehaviour {

    #region Unity Singleton

    private static AudioController _instance;
    
    public static AudioController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioController>();
                if (_instance == null)
                {
                    GameObject gameObject = Instantiate(Resources.Load("System_Prefabs/AudioManager")) as GameObject;
                    _instance = gameObject.GetComponent<AudioController>();
                    DontDestroyOnLoad(gameObject);
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Public Prorperties
    
    public bool IsReady { get; private set; }

    #endregion


    [Header("Audio FX & UI")]
    public AudioFX[] audioFX;

    [Header("Musics")]
    public Music[] musics;
        
	void Awake () {

        IsReady = true;

        foreach (AudioFX aFx in audioFX)
        {
            aFx.source = gameObject.AddComponent<AudioSource>();
            aFx.source.clip = aFx.clip;
            aFx.source.outputAudioMixerGroup = aFx.mixer;

            aFx.source.playOnAwake = false;
            aFx.source.loop = false;
        }

        foreach (Music music in musics)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.clip;
            music.source.outputAudioMixerGroup = music.mixer;

            music.source.playOnAwake = music.playOnAwake;
            music.source.loop = music.loop;
        }
    }

    public void PlayFX(string name)
    {
        AudioFX fx = System.Array.Find(audioFX, aFx => aFx.name == name);
        fx.source.Play();
    }

    public void PlayMusic(string name)
    {
        Music m = System.Array.Find(musics, music => music.name == name);
        m.source.Play();
    }

    public void MuteMusic(string name, bool mute)
    {
        Music m = System.Array.Find(musics, music => music.name == name);
        m.source.mute = mute;
    }

    public void StopMusic(string name)
    {
        Music m = System.Array.Find(musics, music => music.name == name);
        m.source.Stop();
    }
}