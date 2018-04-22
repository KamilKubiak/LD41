using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class StartButton : MonoBehaviour,IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerPrefs.SetInt("hp", 100);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    
}
