using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ToDo block search and sound effect for creating a group, find break VFX for getting rid of blocks
public class Block : MonoBehaviour
{
    public GameManager gm;
    public Board board;
    protected Vector3 pntOnScreen;
    protected Vector3 offset;
    
    public Vector3 gridPosition;
    protected bool animating;
    [HideInInspector] public bool selected;
    protected float animationSpeed=30.0f;
    protected Collider col;
    protected Touch lastTouchedBy;
    public BlockType color;
    [HideInInspector] public int i, j;
    [HideInInspector] public bool grouped;
    [HideInInspector] public bool finalGrouped;
    public int pointValue = 1;
    public int pointMultiplier = 1;
    protected bool active = true;
    public bool selectable = true;
    [HideInInspector] public bool moved = false;
    [HideInInspector] public bool workingGrouped;
    protected AudioSource audio;
    public bool destructable=true;
    protected bool qUpdate = false;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        audio = GetComponent<AudioSource>();
        audio.volume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        col = GetComponent<Collider>();
    }
    public void setQUpdate(bool b)
    {
        qUpdate = b;
    }
    public bool getQUpdate()
    {
        return qUpdate;
    }
    public virtual void Setup()
    {
        gridPosition = gameObject.transform.position;
    }
  
    void setBlockType(BlockType b)
    {
        color = b;
    }
    // Update is called once per frame
    void Update()
    {
        
        if (gridPosition != transform.position && !selected &&active)
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
    protected virtual void OnMouseDown()
    {
        if (selectable)
        {
            
            Debug.Log("mouseDownCalled");
            if (!gm.hasSelected && gm.active && board.GetComponent<Board>().readyForInput)
            {
                Debug.Log("all conditions met to select block");
                gm.selectedBlock = gameObject;
                gm.hasSelected = true;
                selected = true;
                gm.stopTimer();
                transform.localScale = new Vector3(1.5f, 1.5f, 0.2f);
                pntOnScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pntOnScreen.z));
            }
            else
            {
                Debug.Log("Block could not be selected");
                Debug.Log("gm block already selected: " + gm.hasSelected);
                Debug.Log("board ready for input " + board.GetComponent<Board>().readyForInput);
            }
        }
        
    }
    
   

   protected virtual void OnMouseDrag()
   {
        if (gm.active && board.readyForInput && selected)
        {
            //Debug.Log("mouse drag Called");
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pntOnScreen.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) ;
            transform.position = curPosition;
        }
   }

    protected virtual void OnMouseUp()
    {
        board.readyForInput = true;
        // Debug.Log("mouse up Called");
        if (gm.active && selected)
        {
            selected = false;
            gm.hasSelected = false;
            transform.localScale = new Vector3(1, 1, 1);
            if (!moved)
            {
                board.readyForInput = true;
                gm.startTimer();
            }
            else
            {
                board.EndTurn();
            }
            if (qUpdate)
            {
                PrebuiltBoard b = (PrebuiltBoard)board;
                b.updateAllBlocks();
                qUpdate = false;
            }
        }
  
    }
    

    protected virtual void OnMouseEnter()

    {
        Debug.Log("over "+gameObject.name);

       
        if (!selected && gm.hasSelected && gm.active)
        {
            Vector3 placeholder = gm.selectedBlock.GetComponent<Block>().gridPosition;
            gm.selectedBlock.GetComponent<Block>().gridPosition = gridPosition;
            gm.selectedBlock.GetComponent<Block>().moved = true;
            gridPosition = placeholder;
            
            Debug.Log("Switching inde" + gm.selectedBlock.GetComponent<Block>().color + " with "+this.color);
            board.switchIndexes(this, gm.selectedBlock.GetComponent<Block>());
            board.groupedSoundReady = true;
            board.detectGroups(false);
            animateToGridPosition(true);
        }
    }
    public void animateToGridPosition(bool withSound)
    {
        if (!animating)
        {
            if (withSound)
            {
                //audioManager.GetComponent<AudioSource>().Play();
            }
            float a = Vector3.Distance(gameObject.transform.position, gridPosition);
            StartCoroutine(lerpTo(gameObject.transform.position, gridPosition, Time.time, a, animationSpeed));
        }
   
    }
    public void playSound()
    {
        audio.Play();
    }
    protected IEnumerator lerpTo(Vector3 start,Vector3 end,float startTime,float totalDistance,float speed)
    {
        animating = true;
        board.numAnimating += 1;
        gridPosition = end;

        while (gameObject.transform.position != end)
        {
            yield return new WaitForEndOfFrame();
            float distCovered = (Time.time - startTime) * speed;
            float fraction = distCovered / totalDistance;
            transform.position = Vector3.Lerp(start, end, fraction);
            //Debug.Log(gameObject.transform.position == end);
        }
        
        board.numAnimating -= 1;
        animating = false;
      
    }
    protected void DestroyAndAddPointValue()
    {
        //Debug.Log("Adding "+2+"*"+pointMultiplier+" points");
        //Debug.Log("which is " +(2 * pointMultiplier / 100f)+ " seconds");
        board.score += pointValue * pointMultiplier;
        //StartCoroutine(board.displayAndFadePointText(pointValue * pointMultiplier,this.transform.position));
        //gm.barAnimatingValue = gm.timeRemaining;
        if (destructable)
        {
            gameObject.AddComponent<TriangleExplosion>();
            StartCoroutine(gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
        }
        //Rigidbody r = GetComponent<Rigidbody>();
        ///r.AddForce(new Vector3(0, 0, 10), ForceMode.Impulse);
        //r.AddTorque(new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)));

    }
   

}
