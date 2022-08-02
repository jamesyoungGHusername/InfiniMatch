using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameManager gm;
    public List<GameObject> blockList;
    
    [HideInInspector]public GameObject[,] blocks;
    [HideInInspector] public float boardWidth;
    [HideInInspector] public float boardHeight;
    public int r, c;
    protected float xOffset;
    protected float yOffset;
    public int score;
    protected List<GameObject> workingList;
    protected List<List<GameObject>> groups;
    [HideInInspector] public bool readyForInput = true;
    protected bool hasGroups;
    [HideInInspector] public int numAnimating;
    public bool isZen;
    [HideInInspector] public bool soundReady = true;
    [HideInInspector] public bool groupedSoundReady = true;
    protected AudioSource a;
    public GameObject audioManager;
    protected AudioSource click;
    [HideInInspector] public bool firstMoveCompleted=false;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<AudioSource>();
        click = audioManager.GetComponent<AudioSource>();
        click.volume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        a.volume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        numAnimating = 0;
        Debug.Log("in board start");
        blocks = new GameObject[r, c];
        workingList = new List<GameObject>();
        groups = new List<List<GameObject>>();
        xOffset = (c * 1.3f)/2-0.65f;
        yOffset = (r * 1.3f)/2-0.65f;
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++){
                
                  blocks[i, j] = InstantiateRandomBlockFor(i, j);
                    
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (score >= gm.ScoreGoal && !gm.won)
        {
            //Debug.Log("Score above goal, informing manager.");
            readyForInput = false;
            gm.won = true;
        }
        else
        {
            if (numAnimating > 0)
            {
                readyForInput = false;
            }
            else
            {
                readyForInput = true;
            }
        }
    }
    public GameObject InstantiateRandomBlockFor(int i, int j)
    {
        GameObject block = Instantiate(blockList[Random.Range(0, blockList.Count)]);
        float y = i * 1.3f;
        float x = j * 1.3f;
        block.GetComponent<Block>().board = this;
        
        block.transform.position = new Vector3(x-xOffset, y-yOffset, 0);
        block.GetComponent<Block>().i = i;
        block.GetComponent<Block>().j = j;
        block.GetComponent<Block>().Setup();
        
        return block;
        
    }
    public void EndTurn()
    {
        readyForInput = false;
        detectGroups(true);
        StartCoroutine(repopulateBoard());
    }
    IEnumerator repopulateBoard()
    {
        yield return new WaitForSeconds(0.5f);
        hasGroups = false;
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                if (blocks[i, j].GetComponent<Block>().finalGrouped)
                {
                    var temp = blocks[i, j];
                    Destroy(temp);
                    blocks[i, j] = InstatiateFallingBlockFor(i, j);
                    
                }
            }
        }

        detectGroups(false);
        if (hasGroups)
        {
            yield return new WaitUntil(() => numAnimating == 0);
            soundReady = true;
            groupedSoundReady = true;
            EndTurn();
        }
        else
        {
            combo = 0;
            
            if (gm.active)
            {
                yield return new WaitUntil(() => numAnimating == 0);
                readyForInput = true;
                gm.startTimer();
                soundReady = true;
            }
            else
            {
                gm.readyForEndScreen = true;
            }
        }
        
    }
    public IEnumerator displayAndFadePointText(int points,Vector3 at,float duration) 
    {
        yield return new WaitForSeconds(0.0f);
        float d = duration;
        


    }
    public GameObject InstatiateFallingBlockFor(int i, int j)
    {
        GameObject block = Instantiate(blockList[Random.Range(0, blockList.Count)]);
        float y = i * 1.3f;
        float x = j * 1.3f;
        block.GetComponent<Block>().board = this;
        block.GetComponent<Block>().gridPosition = new Vector3(x - xOffset, y - yOffset, 0);
        block.GetComponent<Block>().i = i;
        block.GetComponent<Block>().j = j;
        block.GetComponent<Transform>().position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), -20);
        block.GetComponent<Block>().animateToGridPosition(false);
        return block;
    }
    public void switchIndexes(Block blockA,Block blockB)
    {
        var iPlaceholder = blockA.i;
        var jPlaceholder = blockA.j;
        var blockPlaceholder = blocks[iPlaceholder,jPlaceholder];
        blocks[iPlaceholder, jPlaceholder] = blocks[blockB.i, blockB.j];
        blocks[blockB.i, blockB.j] = blockPlaceholder;
        blockA.i = blockB.i;
        blockA.j = blockB.j;
        blockB.i = iPlaceholder;
        blockB.j = jPlaceholder;
        
    }
    private int lastGroupCount = 0;
    private int combo = 0;
    protected AudioSource b;
    public void detectGroups(bool mark)
    {
        click = audioManager.GetComponent<AudioSource>();
        click.volume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        a = GetComponent<AudioSource>();
        
        a.volume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
        
        groups.Clear();
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                if (!blocks[i, j].GetComponent<Block>().grouped)
                {
                    floodFill(i, j, blocks[i, j].GetComponent<Block>().color);
                    if (workingList.Count >= 4)
                    {   
                        groups.Add(workingList);
                        //Debug.Log("" + workingList[workingList.Count-1].ToString());
                        //Debug.Log("" + groups[groups.Count - 1][groups[groups.Count - 1].Count-1].ToString());
                        
                        if (mark)
                        {
                            combo += 1;
                        }
                        else if(workingList[0].transform.localScale.x!=1.3f)
                        {
                            b = workingList[0].GetComponent<AudioSource>();
                            Debug.Log("Audio source from "+workingList[0].GetComponent<Block>().color +" selected to be played.");
                        }
                        

                        Debug.Log("Group of " + workingList.Count + " " + workingList[0].GetComponent<Block>().color+" identified.");
                        
                        for (int k = 0; k < workingList.Count; k++)
                        {

                            hasGroups = true;
                            if (mark)
                            {
                                
                                workingList[k].GetComponent<Block>().finalGrouped = true;
                                workingList[k].GetComponent<Block>().pointMultiplier = workingList.Count-3;
                            }
                            else
                            {
                                workingList[k].GetComponent<Block>().workingGrouped = true;
                                if (!workingList[k].GetComponent<Block>().selected)
                                {
                                    workingList[k].GetComponent<Block>().transform.localScale = new Vector3(1.3f, 1.3f, 1f);
                                    workingList[k].GetComponent<Rigidbody>().freezeRotation = true;
                                    
                                }
                                
                            }
                        }
                        //Debug.Log(lastGroupCount + " groups previously on the board");
                        
                        
                        
                    }
                    else
                    {
                        
                        for (int k = 0; k < workingList.Count; k++)
                        {
                            workingList[k].GetComponent<Block>().workingGrouped = false;
                            if (!workingList[k].GetComponent<Block>().selected)
                            {
                                if (workingList[k].transform.localScale.y==1.3f) { 
                                    b = workingList[k].GetComponent<AudioSource>();
                                }
                                workingList[k].GetComponent<Block>().transform.localScale = new Vector3(1f, 1f, 1f);
                                workingList[k].GetComponent<Rigidbody>().freezeRotation = false;
                            }
                        }
                    }
                    workingList.Clear();
                }
            }
        }
        if (!mark)
        { 
            //Debug.Log(""+mark);
            Debug.Log(groups.Count + " groups currently on the board");
            if (groups.Count > 0 && groups.Count!=lastGroupCount && gm.hasSelected && groupedSoundReady)
            {
                //TO DO figure out how to fix audio bug. Audio played needs to match new block group type
                //Debug.Log("Last logged group is " + groups[groups.Count - 1][groups[groups.Count - 1].Count - 1].ToString()) ;
                
                b.pitch = ((groups.Count+1)/3.0f);
                b.Play();
                groupedSoundReady = false;
            }
            else if(gm.hasSelected && groupedSoundReady)
            { 
                click.Play();
            }
            lastGroupCount = groups.Count;
        }
        else
        {
            lastGroupCount = 0;
        }
        
        if (combo > 0 && mark && soundReady)
        {
            Debug.Log("Playing combo sound");
            a.pitch = 2.0f;
            a.Play();
            soundReady = false;
        }
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                if (blocks[i, j].GetComponent<Block>().finalGrouped)
                {
                    
                    blocks[i, j].GetComponent<Block>().pointMultiplier = blocks[i, j].GetComponent<Block>().pointMultiplier * combo;
                    
                }
              
                blocks[i, j].GetComponent<Block>().grouped = false;
            }
        }
        
        

    }
    
    
    void floodFill(int rPos, int cPos, BlockType target)
    {
        Debug.Log("Target Color is" + target);
        Debug.Log("Checking" +rPos+","+cPos);
        
        if (rPos > r - 1 || cPos > c - 1)
        {
            Debug.Log("Tile off screen");
            return;
        }
        if (rPos < 0 || cPos < 0)
        {
            Debug.Log("Tile off screen");
            return;
        }
        if(blocks[rPos, cPos] == null)
        {
            Debug.Log("Tile not yet created");
            return;
        }
        //Debug.Log("Block is" + blocks[rPos, cPos].GetComponent<Block>().color);
        if (blocks[rPos,cPos].GetComponent<Block>().color != target)
        {
            //Debug.Log("Color doesnt match;");
            return;
        }
        if (blocks[rPos,cPos].GetComponent<Block>().grouped)
        {
            return;
        }
        //Debug.Log(blocks[rPos, cPos].GetComponent<Block>().color+" block matches.");
        workingList.Add(blocks[rPos, cPos]);
        blocks[rPos, cPos].GetComponent<Block>().grouped = true;
        floodFill(rPos: rPos - 1, cPos: cPos, target);
        floodFill(rPos: rPos + 1, cPos: cPos, target);
        floodFill(rPos: rPos, cPos: cPos - 1, target);
        floodFill(rPos: rPos, cPos: cPos + 1, target);
    }
    public int getScore()
    {
        return score;
    }
}
public enum BlockType
{
    Red,
    Blue,
    Purple,
    Green,
    Orange,
    Pink,
    Yellow,
    Immobile
}
