using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source (для SFX)")]
    public AudioSource sfxSource;

    [Header("Отдельный AudioSource для шагов")]
    public AudioSource footstepSource;

    [Header("Отдельный AudioSource для музыки у босса")]
    public AudioSource bossMusicSource;

    [Header("Звуки")]
    public AudioClip swordSwingClip;
    public AudioClip hitEnemyClip;
    public AudioClip parryClip;
    public AudioClip footstepClip;
    public AudioClip dashClip;
    public AudioClip bossMusicClip;
    public AudioClip healClip;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySwordSwing()
    {
        if (sfxSource != null && swordSwingClip != null)
        {
            sfxSource.PlayOneShot(swordSwingClip);
        }
    }

    public void PlayHitEnemy()
    {
        if (sfxSource != null && hitEnemyClip != null)
        {
            sfxSource.PlayOneShot(hitEnemyClip);
        }
    }

    public void PlayParry()
    {
        if (sfxSource != null && parryClip != null)
        {
            sfxSource.PlayOneShot(parryClip);
        }
    }

    public void PlayFootstep()
    {
        if (footstepSource != null && footstepClip != null && !footstepSource.isPlaying)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
            footstepSource.Play();
        }
    }

    public void StopFootstep()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }
    public void PlayDash()
    {
        if (sfxSource != null && dashClip != null)
        {
            sfxSource.PlayOneShot(dashClip);
        }
    }

    public void PlayHeal()
    {
        if (sfxSource != null && healClip != null)
        {
            sfxSource.PlayOneShot(healClip);
        }
    }

    public void PlayBossMusic()
    {
        if (bossMusicSource != null && bossMusicClip != null)
        {
            bossMusicSource.clip = bossMusicClip;
            bossMusicSource.loop = true;
            bossMusicSource.Play();
        }
    }


    public void StopBossMusic()
    {
        if (bossMusicSource != null && bossMusicSource.isPlaying)
        {
            bossMusicSource.Stop();
        }
    }
}


