using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleExplosion : MonoBehaviour
{
    private List<GameObject> triangles = new List<GameObject>();
    public IEnumerator SplitMesh(bool destroy)
    {
        triangles = new List<GameObject>();
        if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null)
        {
            yield return null;
        }

        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }

        Mesh M = new Mesh();
        if (GetComponent<MeshFilter>())
        {
            M = GetComponent<MeshFilter>().mesh;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }

        Material[] materials = new Material[0];
        if (GetComponent<MeshRenderer>())
        {
            materials = GetComponent<MeshRenderer>().materials;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {

            int[] indices = M.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                GameObject GO = new GameObject("Triangle " + (i / 3));
                
                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;
                GO.AddComponent<MeshRenderer>().material = materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
                Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
                GO.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(300, 500), explosionPos, 5);
                triangles.Add(GO);
                
            }
            
        }




        GetComponent<Renderer>().enabled = false;
        //for(int i = 0; i < triangles.Count; i++){
            //StartCoroutine(lerpTo(0.1f, triangles[i], new Vector3(0, -20, 0), 10.0f));
        //}
        yield return new WaitForSeconds(10);
        
        if (destroy == true)
        {
            Destroy(gameObject);
        }

    }
    IEnumerator lerpTo(float startDelay,GameObject go, Vector3 end, float speed)
    {
        Debug.Log("animating triangle");
        yield return new WaitForSeconds(startDelay);
        Rigidbody r = go.GetComponent<Rigidbody>();
        //r.useGravity = false;
        r.isKinematic = true;
        Vector3 start = go.transform.position;
        Debug.Log("Registered position: "+start);
        float startTime = Time.time;
        float dist = Vector3.Distance(start, end);
        while (go.transform.position != end)
        {
            yield return new WaitForEndOfFrame();
            float distCovered = (Time.time - startTime) * speed;
            float fraction = distCovered / dist;
            go.transform.position = Vector3.Lerp(start, end, fraction);
            Debug.Log(start+" "+end+" "+go.transform.position);
            //Debug.Log(gameObject.transform.position == end);
        }
        Debug.Log("exited animation loop");
        Destroy(go, 5 + Random.Range(0.0f, 5.0f));
    }


}
