using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour {

    [Header("Volume")]
    [SerializeField] float globalVolume = 0.25f;
    [SerializeField] float musicVolume = 0.5f;
    [SerializeField] float sfxVolume = 0.75f;

    public static string MUSIC_VOLUME_KEY = "musicVol";
    public static string SFX_VOLUME_KEY = "sfxVol";

    private Vector3 cameraPos;

    public float GlobalVolume { get => globalVolume; set => globalVolume = value; }
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }

    private void Awake() {
        SetUpSingleton();
    }

    private void SetUpSingleton() {
        if (FindObjectsOfType<MusicPlayer>().Length > 1) {
            Destroy(gameObject);
        } else {
            cameraPos = Camera.main.transform.position;
            if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY)) {
                musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
            }
            if (PlayerPrefs.HasKey(SFX_VOLUME_KEY)) {
                sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
            }
            AdjustMusicVolume();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AdjustMusicVolume() {
        GetComponent<AudioSource>().volume = musicVolume * GlobalVolume;
    }

    public void SavePlayerPrefs() {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    public void PlayOneShot(AudioClip clip, float volume) {
        AudioSource.PlayClipAtPoint(clip, cameraPos, volume * sfxVolume * GlobalVolume);
    }
}
