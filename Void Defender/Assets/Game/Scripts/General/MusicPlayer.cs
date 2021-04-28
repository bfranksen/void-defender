using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour {

    [Header("Volume")]
    [SerializeField] float globalVolume = 0.25f;
    [SerializeField] float musicVolume = 0.5f;
    [SerializeField] float sfxVolume = 0.75f;

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
            if (PlayerPrefsController.HasMusicVolume()) {
                musicVolume = PlayerPrefsController.GetMusicVolume();
            }
            if (PlayerPrefsController.HasSfxVolume()) {
                sfxVolume = PlayerPrefsController.GetSfxVolume();
            }
            AdjustMusicVolume();
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AdjustMusicVolume() {
        GetComponent<AudioSource>().volume = musicVolume * GlobalVolume;
    }

    public void SavePlayerPrefs() {
        PlayerPrefsController.SetMusicVolume(musicVolume);
        PlayerPrefsController.SetSfxVolume(sfxVolume);
    }

    public void PlayOneShot(AudioClip clip, float volume) {
        AudioSource.PlayClipAtPoint(clip, cameraPos, volume * sfxVolume * GlobalVolume);
    }
}
