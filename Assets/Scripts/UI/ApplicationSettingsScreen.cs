using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ApplicationSettingsScreen : MonoBehaviour
    {
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxSlider;

        // Start is called before the first frame update
        void Start()
        {
            musicToggle.isOn = false; // 
            sfxToggle.isOn = false; //
            musicSlider.value = 0; //
            sfxSlider.value = 0; //

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}