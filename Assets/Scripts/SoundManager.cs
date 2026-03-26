using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public bool isFXEnabled = true;
    public bool isMusicEnabled = true;
    public float musicVolume;
    public float fxVolume;
    public AudioSource EffectsSource;
    public AudioSource MusicSource;

    public AudioClip menuMusic;
    public AudioClip gameMusiclvl_1;
    public AudioClip gameMusiclvl_2;
    public AudioClip gameMusiclvl_3;
    public AudioClip jumpFX;
    public AudioClip collectedFX;
    public AudioClip finishedFX;
    public AudioClip deadFX;

    public static SoundManager Instance = null;

    private void Awake()
    {
        // Singleton + persistència
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscripció a canvis d'escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Reproduir música de l'escena inicial
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Cridat automàticament cada vegada que es carrega una escena nova
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (System.Array.IndexOf(GameManager.Instance.menuSceneNames, sceneName) >= 0)
            PlayMusic(menuMusic);
        else if (System.Array.IndexOf(GameManager.Instance.gameSceneNames, sceneName) == 0)
        {
            PlayMusic(gameMusiclvl_1);
        }
        else if (System.Array.IndexOf(GameManager.Instance.gameSceneNames, sceneName) == 1)
        {
            PlayMusic(gameMusiclvl_2);
        }
        else if (System.Array.IndexOf(GameManager.Instance.gameSceneNames, sceneName) == 2)
        {
            PlayMusic(gameMusiclvl_3);
        }
            
        // Si l'escena no coincideix amb cap llista, silenci
    }

    // ── API pública (igual que l'original) ───────────────────────────

    public void Play(AudioClip clip)
    {
        if (isFXEnabled)
        {
            EffectsSource.clip = clip;
            EffectsSource.Play();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!isMusicEnabled || clip == null) return;
        if (MusicSource.clip == clip && MusicSource.isPlaying) return; // ja sona
        MusicSource.enabled = true;
        MusicSource.loop = true;
        MusicSource.clip = clip;
        MusicSource.volume = musicVolume;
        MusicSource.Play();
    }

    public void PlayFX(int i)
    {
        if (!isFXEnabled) return;
        EffectsSource.enabled = true;
        EffectsSource.clip = i switch
        {
            0 => jumpFX,
            1 => collectedFX,
            2 => finishedFX,
            3 => deadFX,
            _ => null
        };
        if (EffectsSource.clip != null) EffectsSource.Play();
    }

    public void PlayMusic(int i)
    {
        PlayMusic(i == 0 ? menuMusic : gameMusiclvl_1);
    }

    public void StopMusic() => MusicSource.Stop();

    public void SetMusicVolume(float f)
    {
        musicVolume = f;
        MusicSource.volume = f;
    }

    public void SetFXVolume(float f)
    {
        fxVolume = f;
        EffectsSource.volume = f;
    }

    public void SetEnableFX(bool b)
    {
        isFXEnabled = b;
        if (b) PlayFX(0);
    }

    public void SetEnableMusic(bool b)
    {
        isMusicEnabled = b;
        if (!b) StopMusic();
        else PlayMusicForScene(SceneManager.GetActiveScene().name);
    }
}
