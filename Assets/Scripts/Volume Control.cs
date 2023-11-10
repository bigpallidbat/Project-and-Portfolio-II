using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle volumeToggle;
    [SerializeField] float mult = 30f;
    private bool disableToggle;

    void Awake()
    {
        volumeSlider.onValueChanged.AddListener(HandleSliderValueChanged);
        volumeToggle.onValueChanged.AddListener(HandleTogglechanges);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, volumeSlider.value);
        PlayerPrefs.Save();
    }

    private void HandleTogglechanges(bool enable)
    {
        if (disableToggle) return;

        if (enable)
        {
            volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
        }
        else
        {
            volumeSlider.value = volumeSlider.minValue;
        }
    }

    private void HandleSliderValueChanged(float val)
    {
        audioMixer.SetFloat(volumeParameter, Mathf.Log10(val) * mult);
        disableToggle = true;
        volumeToggle.isOn = volumeSlider.value > volumeSlider.minValue;
        disableToggle = false;
    }

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
    }


}
