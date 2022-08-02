using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetGlobalVolume : MonoBehaviour
{
    public Slider s;
    // Start is called before the first frame update
    void Start()
    {
        s.value = PlayerPrefs.GetFloat("masterVolume", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setNewVolumeLevel()
    {
        Debug.Log("global volume changed");
        PlayerPrefs.SetFloat("masterVolume", s.value);
        PlayerPrefs.Save();
    }
}
