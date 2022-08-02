using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrink: MonoBehaviour
{
    // Start is called before the first frame update
    public float duration;
    public float endingScale;
    public Transform t;
    private float timeRemaining;
    void Start()
    {
        t = this.transform;
        timeRemaining = duration;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        
    }
}
