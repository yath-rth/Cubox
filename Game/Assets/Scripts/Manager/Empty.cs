using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : MonoBehaviour
{
    public GameObject obj;

    private void Start() {
        GetComponent<Animator>().Play("Start", 0);
    }

    public void startGame(){
        UIManager.uiManagerInstance.Resume();
        UIManager.uiManagerInstance.ButtonOpen(obj);
        UIManager.uiManagerInstance.ButtonClose(this.gameObject);
    }
}
