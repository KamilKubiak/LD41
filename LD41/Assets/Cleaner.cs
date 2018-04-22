using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public class Cleaner : MonoBehaviour
{

    public void OnInspectorGUI()
    {
        if (GUILayout.Button("Clean Redundant Tiles"))
        {
            var tiles = GameObject.Find("LevelLayout").GetComponentsInChildren<WalkingArea>().ToList();

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                for (int j = 0; i > tiles.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (tiles[i].transform.position == tiles[j].transform.position)
                    {
                        Destroy(tiles[j]);
                        tiles.RemoveAt(j);
                        j--;
                        i--;
                    }
                }
            }
        }
    }


}
