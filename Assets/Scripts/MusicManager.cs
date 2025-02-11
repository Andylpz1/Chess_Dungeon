using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;

    public AudioClip mainMenuMusic;
    public AudioClip battleMusic;
    public AudioClip victoryMusic;

    void Awake()
    {
        // 确保只有一个实例，并在场景间不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 场景切换时不销毁音乐管理器
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic(mainMenuMusic);  // 默认播放主菜单音乐
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusicSource.clip == clip) 
        {
            Debug.Log("Already playing this music. Skipping.");
            return;
        }

        backgroundMusicSource.Stop();  // 停止当前音乐
        backgroundMusicSource.clip = clip;

        if (backgroundMusicSource.clip == null)
        {
            Debug.LogError("Music clip is null. Ensure the AudioClip is assigned correctly.");
            return;
        }

        backgroundMusicSource.Play();
        Debug.Log($"Playing background music: {clip.name}");
    }


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);  // 播放短音效
    }
}
