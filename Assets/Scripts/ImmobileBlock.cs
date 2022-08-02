using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmobileBlock : Block
{
    // Start is called before the first frame update

    private Rigidbody ownBody;
    void Start()
    {
        selectable = true;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        audio = GetComponent<AudioSource>();
        audio.volume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        col = GetComponent<Collider>();
        ownBody = GetComponent<Rigidbody>();
        ownBody.AddTorque(new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)));
    }
 
    // Update is called once per frame
    void Update()
    {
        if (gridPosition != transform.position && !selected && active)
        {
            animateToGridPosition(false);
        }
        if (finalGrouped && active)
        {
            //Debug.Log("Final grouping detected, highlighting");
            active = false;
            //var render = GetComponent<Renderer>();
            //render.material.SetColor("_Color", Color.white);
            DestroyAndAddPointValue();

        }
    }
    protected override void OnMouseDown()
    {
        audio.Play();
        ownBody.AddTorque(new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50)));
    }
    protected override void OnMouseDrag()
    {
        
    }
    protected override void OnMouseUp()
    {
        
    }
    protected override void OnMouseEnter()
    {
        if (gm.hasSelected && gm.active)
        {
            Vector3 placeholder = gm.selectedBlock.GetComponent<Block>().gridPosition;
            gm.selectedBlock.GetComponent<Block>().gridPosition = gridPosition;
            gm.selectedBlock.GetComponent<Block>().moved = true;
            gridPosition = placeholder;

            Debug.Log("Switching inde" + gm.selectedBlock.GetComponent<Block>().color + " with " + this.color);
            board.switchIndexes(this, gm.selectedBlock.GetComponent<Block>());
            board.groupedSoundReady = true;
            board.detectGroups(false);
            animateToGridPosition(true);
        }
    }
}
