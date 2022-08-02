using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrebuiltBoard : Board
{
    // Start is called before the first frame update
    private BlockType[] available;
    public BlockType[] blockSequence;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        numAnimating = 0;
        Debug.Log("in board start");
        blocks = new GameObject[r, c];
        workingList = new List<GameObject>();
        groups = new List<List<GameObject>>();
        xOffset = (c * 1.3f) / 2 - 0.65f;
        yOffset = (r * 1.3f) / 2 - 0.65f;
        available = new BlockType[blockList.Count];
        for(int i = 0; i < blockList.Count; i++)
        {
            available[i] = blockList[i].GetComponent<Block>().color;
        }
        
        

        
        InstantiateBlocksFrom(blockSequence, r, c);
        if (gm.isTutorial)
        {
            blocks[0, 3].GetComponent<Block>().selectable = true;
            blocks[0, 3].GetComponent<Block>().setQUpdate(true);
        }
    }

    public void updateAllBlocks()
    {
        for (int i=0; i < r; i++){
            for(int j = 0; j < c; j++)
            {
                blocks[i, j].GetComponent<Block>().selectable = true;
            }
        }
        firstMoveCompleted = true;
        Debug.Log(blocks[4, 3].GetComponent<Block>().color);
        Debug.Log(gm.selectedBlock.GetComponent<Block>().color);
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
    public void InstantiateBlocksFrom(BlockType[] sequence,int r,int c)
    {
        var count = 0;
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                blocks[i, j] = InstantiateBlockOfType(sequence[count], i, j);
                count++;
            }
        }
    }
    public GameObject InstantiateBlockOfType(BlockType t,int i,int j)
    {
        GameObject block = Instantiate(blockList[indexOf(t)]);
        float y = i * 1.3f;
        float x = j * 1.3f;
        block.GetComponent<Block>().board = this;
        block.transform.position = new Vector3(x - xOffset, y - yOffset, 0);
        block.GetComponent<Block>().i = i;
        block.GetComponent<Block>().j = j;
        block.GetComponent<Block>().Setup();
        block.GetComponent<Block>().selectable = false;
        return block;
    }
    public int indexOf(BlockType t)
    {
        var index = 0;
        for (int i = 0; i < blockList.Count; i++)
        {
            if (blockList[i].GetComponent<Block>().color == t)
            {
                index = i;
            }
        }
        return index;
    }
    
    
}
