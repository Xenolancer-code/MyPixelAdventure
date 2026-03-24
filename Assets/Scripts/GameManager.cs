using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject soundManagerPrefab;
    public SoundManager Sound { get; private set; }

    //LEVEL STATES
    public bool isGameOver = false;
    public bool isLevelRestarted = false;
    public bool isLevelCompleted = false;
    public bool isGameCompleted = false;

    [SerializeField] private GameObject mobileControlsPrefab;
    private GameObject mobileControlsInstance;

    [Header("Configuració d'escenes")]
    [Tooltip("Noms exactes de les escenes que usen la música de menú")]
    public string[] menuSceneNames = { "Menu", "MainMenu" };
    [Tooltip("Noms exactes de les escenes que usen la música de joc")]
    public string[] gameSceneNames = { "Game", "Level1", "Level2" };

    private void Awake()
    {
        // ── Singleton GameManager ─────────────────────────────────────
        // Comprova si ja existeix un GameManager a l'escena (per exemple,
        // si s'ha col·locat manualment a més d'una escena)
        if (Instance != null && Instance != this)
        {
            Debug.Log("[GameManager] Duplicat detectat, destruint.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ── Inicialitzar subsistemes ──────────────────────────────────
        InitSoundManager();
    }

    void Start()  {
        //SOUNDMANAGER
        InitSoundManager();
        UpdateMobileControls();
    }

    private bool IsMobile()
    { 
        return true;
    }

    // Llámalo también al cargar cada escena
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMobileControls();
    }

    private void UpdateMobileControls()
    {
        bool isMobile = IsMobile(); // ← canviat
        bool isGameScene = IsGameScene(SceneManager.GetActiveScene().name);

        if (isMobile && isGameScene)
        {
            if (mobileControlsInstance == null)
            {
                Canvas sceneCanvas = FindFirstObjectByType<Canvas>();
                if (sceneCanvas != null)
                {
                    mobileControlsInstance = Instantiate(mobileControlsPrefab, sceneCanvas.transform);
                }
            }
        }
        else
        {
            if (mobileControlsInstance != null)
            {
                Destroy(mobileControlsInstance);
                mobileControlsInstance = null;
            }
        }
    }

    private bool IsGameScene(string sceneName)
    {
        foreach (string name in gameSceneNames)
        {
            if (name == sceneName) return true;
        }
        return false;
    }

    private void InitSoundManager()
    {
        // Comprova si ja n'hi ha un a l'escena (qualsevol escena carregada)
        Sound = FindFirstObjectByType<SoundManager>();

        if (Sound != null)
        {
            Debug.Log("[GameManager] SoundManager ja existeix a l'escena, no s'instancia de nou.");
            return;
        }

        // No n'hi ha cap → instanciar des del prefab
        if (soundManagerPrefab == null)
        {
            Debug.LogError("[GameManager] soundManagerPrefab no assignat al Inspector!");
            return;
        }

        GameObject go = Instantiate(soundManagerPrefab);
        go.name = "SoundManager";
        Sound = go.GetComponent<SoundManager>();

        if (Sound == null)
        {
            Debug.LogError("[GameManager] El prefab SoundManager no té el component SoundManager!");
            return;
        }

        Debug.Log("[GameManager] SoundManager instanciat correctament.");
    }

    void Reset() {
        isGameOver = false;
        isLevelRestarted = false;
        isLevelCompleted = false;
        isGameCompleted = false;
    }

    void OnGUI() {
        if (isGameCompleted || isGameOver) {
            GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
            myButtonStyle.fontSize = 30;
            if ( GUI.Button(new Rect(Screen.width/2-Screen.width/8, Screen.height/2-Screen.height/8, Screen.width/4, Screen.height/4), isGameCompleted?"CONGRATULATIONS!!":"GAMEOVER!!", myButtonStyle)) {
                Reset();
                SceneManager.LoadScene(1);
            }
        }
    }

    public void GameOver() {
        isGameOver = true; 
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CompleteLevel() {
        if (SceneManager.GetActiveScene().name == "Level3"){
            isGameCompleted = true;
        } else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}