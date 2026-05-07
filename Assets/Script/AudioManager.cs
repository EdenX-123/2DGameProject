using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static AudioManager instance;
    [Header("BGM")]
    public AudioClip bgmClip;
    private AudioSource bgmSource;

    [Header("UI Sound Effects")]
    public AudioClip buttonClickSFX;

    [Header("Player Sound Effects")]
    public AudioClip attackSFX;
    public AudioClip jumpSFX;
    public AudioClip takeDamageSFX;

    [Header("Checkpoint Sound Effects")]
    public AudioClip checkpointSFX;
    private AudioSource sfxSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 创建两个 AudioSource：一个 BGM 一个 SFX
        AudioSource[] sources = GetComponents<AudioSource>();
        bgmSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        sfxSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    void Start()
    {
        PlayBGM();
    }

    // ========== BGM ==========
    public void PlayBGM()
    {
        if (bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();
    public void SetBGMVolume(float volume) => bgmSource.volume = volume;

    // ========== SFX ==========
    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()  => PlaySFX(buttonClickSFX);
    public void PlayAttack()       => PlaySFX(attackSFX);
    public void PlayJump()         => PlaySFX(jumpSFX);
    public void PlayTakeDamage()   => PlaySFX(takeDamageSFX);
    public void PlayCheckpoint()   => PlaySFX(checkpointSFX);

}
