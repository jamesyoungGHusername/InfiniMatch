using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public List<MoveSequence> movesToBeExecuted;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator executeSequence(MoveSequence m)
    {
        yield return new WaitForSeconds(0f);
    }
}
public class MoveSequence
{
    private int seqLength;
    private List<Vector3> vertexList;
    private GameObject objToBeMoved;
    private int maxLength;
    public MoveSequence(GameObject targeting)
    {
        this.objToBeMoved = targeting;
        maxLength = 10;
    }
    public MoveSequence(List<Vector3> vertexList,GameObject targeting)
    {
        this.objToBeMoved = targeting;
        this.vertexList = vertexList;
        seqLength = vertexList.Count;
        maxLength = seqLength;
    }
    public MoveSequence(MoveSequence toBeCopied)
    {
        seqLength = toBeCopied.GetSequenceLength();
        vertexList = toBeCopied.GetFullVertexList();
        objToBeMoved = toBeCopied.GetTargetObject();
    }
    public void AddVertex(Vector3 v)
    {
        if (seqLength + 1 < maxLength)
        {
            vertexList.Add(v);
            seqLength = vertexList.Count;
        }
    }
    public void increaseMaxSequenceLength(int to)
    {
        if (to < seqLength)
        {
            maxLength = to;
        }
    }
    public GameObject GetTargetObject()
    {
        return objToBeMoved;
    }
    public Vector3 GetVertexAt(int index)
    {
        return vertexList[index];
    }
    public List<Vector3> GetFullVertexList()
    {
        return vertexList;
    }
    public int GetSequenceLength()
    {
        return seqLength;
    }
}

