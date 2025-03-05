using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{

    [SerializeField] Vector3 rotSpeed = new Vector3(0, 30 ,0);
    float furthestWallPoint;
    [SerializeField] MapManager mapManager;
    bool dissolveWalls = false;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        furthestWallPoint = mapManager.GetMapSize();
        dissolveWalls = true;
    }
    
    void Update()
    {
        transform.parent.Rotate(rotSpeed * Time.deltaTime);
        
        
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(!dissolveWalls) return;
        if(other.name == "WallFull" || other.name == "WallBottomHole" || other.name == "WallTopHole" || other.name == "WallGlass" || other.name == "WallDestructable")
        {
            if(other.transform.position.x > (furthestWallPoint * 2 - 2) || other.transform.position.x < (-furthestWallPoint * 2 + 2))
            {
                return;
            }
            if(other.transform.position.z > (furthestWallPoint * 2 - 2) || other.transform.position.z < (-furthestWallPoint * 2 + 2))
            {
                return;
            }
            if(other.TryGetComponent(out MeshRenderer mesh))
            {
                if(other.TryGetComponent(out Collider col))
                {
                    StartCoroutine(PingPongMesh(mesh, col));
                }
            }
        }
        
    }
    
    IEnumerator PingPongMesh(MeshRenderer mesh, Collider col)
    {
        float dissolveDuration = 0;
        
        while(dissolveDuration <= 1)
        {
            dissolveDuration += Time.deltaTime;
            mesh.material.SetFloat("_Dissolve", dissolveDuration);
            yield return null;
        }
        mesh.material.SetFloat("_Dissolve", 1);
        col.enabled = false;
        
        /* float waitForReturnDuration = 3;
        yield return new WaitForSeconds(waitForReturnDuration);
        
        float enableDuration = 0;
        while(enableDuration <= 1)
        {
            enableDuration += Time.deltaTime;
            mesh.material.SetFloat("_Dissolve", enableDuration);
            yield return null;
        }
        col.enabled = true;
        mesh.material.SetFloat("_Dissolve", 1); */
        
    }
    
}
