//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;
//[CustomEditor(typeof(Transform))]
//public class Cleaner : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        if (GUILayout.Button("Clean Redundant Tiles"))
//        {
//            var tiles = GameObject.Find("LevelLayout").GetComponentsInChildren<WalkingArea>().ToList();
//            Debug.Log(tiles.Count);
//            for (int i = tiles.Count - 1; i >= 0; i--)
//            {
//                Debug.Log(tiles.Count);
//                for (int j = 0; j < tiles.Count; j++)
//                {
//                    Debug.Log(i + " " + j);
//                    if (j == i)
//                    {
//                        continue;
//                    }
//                    if (((Vector2)(tiles[i].transform.position - tiles[j].transform.position)).magnitude <0.01f)
//                    {
//                        Debug.Log("destroying: " + tiles[j].gameObject.name);
//                        DestroyImmediate(tiles[j].gameObject);
//                        tiles.RemoveAt(j);
                        
//                        j--;
//                        i--;
//                    }
//                }
//            }
//        }
//    }


//}
