using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isContact;
    Player player;

    private void Start()
    {
        player = Player.playerInstance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player != null)
        {
            if (other.gameObject == player.gameObject)
            {
                isContact = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (player != null)
        {
            if (other.gameObject == player.gameObject)
            {
                isContact = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            if (other.gameObject == player.gameObject)
            {
                isContact = false;
            }
        }
    }
}
