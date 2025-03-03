using System.Collections;
using System.Collections.Generic;
using _game.Scripts;
using UnityEngine;
using UnityUtils;

public class NonRandomizedArea : MonoBehaviour
{
    [SerializeField] Vector3 boxSize = new Vector3(18, 100, 18);
    List<Transform> wallsToDisable = new List<Transform>();
    List<Transform> floorsToDisable = new List<Transform>();

    void OnEnable()
    {
        StartCoroutine(CleanUp());
        
    }
    
    IEnumerator CleanUp()
    {
        yield return new WaitForSeconds(0.3f);
        Collider[] cols = Physics.OverlapBox(transform.position, boxSize/2, Quaternion.identity, Physics.AllLayers);
        for (int i = 0; i < cols.Length; i++)
        {
           /* if(cols[i].TryGetComponent(out MazeNode mn))
           {
               wallsToDisable.Add(mn);
               mn.LockWall();
           } */
           if(cols[i].TryGetComponent(out SolidMapObject smo))
           {
               Destroy(smo.gameObject);
           }
           if(cols[i].name == "WallFull" || cols[i].name == "WallBottomHole" || cols[i].name == "WallTopHole" || cols[i].name == "WallGlass" || cols[i].name == "WallDestructable")
           {
                GameObject wallToDisable = cols[i].transform.parent.gameObject;
                //Debug.Log("disabling this wall, is it null: " + (wallToDisable == null).ToString(), wallToDisable);
                wallsToDisable.Add(wallToDisable.transform);
                
           }
           if(cols[i].name == "AcidFloor" || cols[i].name == "LavaFloor" || cols[i].name == "GlassFloor" || cols[i].name == "DestructableFloor")
           {
                GameObject floorToDisable = cols[i].gameObject;
                //Debug.Log("disabling this wall, is it null: " + (wallToDisable == null).ToString(), wallToDisable);
                floorsToDisable.Add(floorToDisable.transform);
                
           }
           
           foreach (var wall in wallsToDisable)
           {
                Debug.Log("Disabled wall ",wall);
                wall.gameObject.SetActive(false);
           }
           
           foreach (var floor in floorsToDisable)
           {
                Debug.Log("Disabled floor ",floor);
                floor.gameObject.SetActive(false);
                for (int j = 0; j < floor.transform.parent.childCount; j++)
                {
                    Transform child = floor.parent.transform.GetChild(j);
                    if(child.name == "FloorBlock")
                    {
                        Debug.Log("enabled block floor ",child);
                        child.gameObject.SetActive(true);
                    }
                }
           }
           
           
        }
    }
    
}
