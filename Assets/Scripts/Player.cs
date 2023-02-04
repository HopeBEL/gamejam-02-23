using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//"pretty much MonoBehaviour but with a lot of networking functionality added from Mirror"
public class Player : NetworkBehaviour
{   
    private PlayerActions playerControls;
    public Sprite playerSprite1;
    public Sprite playerSprite2;

    [SyncVar(hook = "OnHiCountChange")]int hiCount = 0;
    public float mousePosZ = 10f;
    NetworkIdentity id;

    private void Awake() {
        playerControls = new PlayerActions();
        
    }

    private void Start() {
        id = gameObject.GetComponent<NetworkIdentity>();
        Debug.Log("Id client : " + id.netId);
        if (id.netId == 1) {
            gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite1;
        }
        else if (id.netId >= 2) {
            gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite2;
        }
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }
    void HandleMovement() {
        //field provided by the NetworkBehaviour
        //returns true if the object represents the player on the local machine
        //on ne veut détecter les mouvements que du joueur sur la machine locale
        
        if (isLocalPlayer && id.netId == 1) {
            Vector3 move = playerControls.Player1.Move.ReadValue<Vector2>();
            move = new Vector3(move.x * 0.1f, move.y * 0.1f, 0);
            transform.position = transform.position + move;
        }
        else if (isLocalPlayer && id.netId >= 2) {
            Vector3 move =Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());  
            move.z = 0;
            transform.position = move;
            Debug.Log(transform.position);
        }
    }

    void Update() {
        HandleMovement();

        /*if (isLocalPlayer && Input.GetKeyDown("x")) {
            Debug.Log("Sending hi to server");
            hi();
        }

        if (isServer && transform.position.y > 50) {
            TooHigh();
        }*/
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

