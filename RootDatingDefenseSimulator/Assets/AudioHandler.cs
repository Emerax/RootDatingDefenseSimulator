using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    public float transitionLineCombat = 0.45f;
    public float transitionLineDating = 0.35f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0,0,1,1);

    public AudioMixer TDAudioMixer;
    public AudioMixer DSAudioMixer;
    private float TDDefaultVolume;
    private float DSDefaultVolume;

    private void Awake()
    {
        float readTDVolume;
        TDAudioMixer.GetFloat("MasterVolume", out readTDVolume);
        TDDefaultVolume = Mathf.Pow(10, (readTDVolume / 20));

        float readDSVolume;
        DSAudioMixer.GetFloat("MasterVolume", out readDSVolume);
        DSDefaultVolume = Mathf.Pow(10, (readDSVolume / 20));
    }

    private void Update()
    {
        float screenHeight = Screen.height;
        Vector2 mousePos = Input.mousePosition;

        float screenHeightPercent = mousePos.y / screenHeight;

        float transitionT = (screenHeightPercent - transitionLineDating) / 
            (transitionLineCombat- transitionLineDating);
        transitionT = Mathf.Clamp(transitionT, 0, 1);

        float volumeT = transitionCurve.Evaluate(transitionT);
        float combatMusicVolume = Mathf.Clamp(volumeT, 0.0001f, 1);
        float datingVolumeMusic = Mathf.Clamp(1-volumeT, 0.0001f, 1);

        TDAudioMixer.SetFloat("MasterVolume", Mathf.Log10(combatMusicVolume) * 20);
        DSAudioMixer.SetFloat("MasterVolume", Mathf.Log10(datingVolumeMusic) * 20);
    }
}
