using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//"pretty much MonoBehaviour but with a lot of networking functionality added from Mirror"
public class Player : NetworkBehaviour
{
    [SyncVar(hook = "OnHiCountChange")]int hiCount = 0;
    public float mousePosZ = 10f;

    void HandleMovement() {
        //field provided by the NetworkBehaviour
        //returns true if the object represents the player on the local machine
        //on ne veut détecter les mouvements que du joueur sur la machine locale
        NetworkIdentity id = gameObject.GetComponent<NetworkIdentity>();
        if (isLocalPlayer && id.netId == 1) {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0);
            transform.position = transform.position + movement;
        }
        else if (isLocalPlayer && id.netId >= 2) {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mousePosZ);
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);  
            Debug.Log(transform.position);
        }
    }

    void Update() {
        HandleMovement();

        if (isLocalPlayer && Input.GetKeyDown("x")) {
            Debug.Log("Sending hi to server");
            hi();
        }

        if (isServer && transform.position.y > 50) {
            TooHigh();
        }
    }

    //Called on client but runs on server
    [Command]
    void hi() {
        Debug.Log("Received hi from client");
        hiCount++;
        ReplyHi();
    }

    //Called on the server but runs on clients
    [ClientRpc]
    void TooHigh() {
        Debug.Log("Too high");
    }

    //When this method is called on the server, it'll run on the
    //client associated with this object
    [TargetRpc]
    void ReplyHi() {
        Debug.Log("Received hi from server");
    }

    void OnHiCountChange(int oldCount, int newCount) {
        Debug.Log("We had " + oldCount + " hi but now we have " + newCount + "hi");
    }
}

