using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenOnClick : MonoBehaviour
{
    public GameObject target;
    public bool OpenOrClose = false;

    void Start()
    {
        Button button = GetComponent<Button>();

        if(button != null){
            button.onClick.AddListener(Openorclose);
        }
    }

    void Openorclose()
    {
        if(OpenOrClose){
            target.SetActive(false);
        }
        else{
            target.SetActive(true);
        }
    }
}
