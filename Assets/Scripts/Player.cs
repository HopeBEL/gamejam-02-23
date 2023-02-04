using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//"pretty much MonoBehaviour but with a lot of networking functionality added from Mirror"
public class Player : NetworkBehaviour
{
    private PlayerInputAction playerControls;
    public Sprite playerSprite1;
    public Sprite playerSprite2;
    public PlayerInput playerInput;

    NetworkIdentity id;

    private void Awake()
    {
        playerControls = new PlayerInputAction();

    }

    private void Start()
    {
        id = gameObject.GetComponent<NetworkIdentity>();
        Debug.Log("Id client : " + id.netId);
        if (id.netId == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite1;
        }
        else if (id.netId >= 2)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite2;
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void HandleMovement()
    {
        //field provided by the NetworkBehaviour
        //returns true if the object represents the player on the local machine
        //on ne veut détecter les mouvements que du joueur sur la machine locale

        if (isLocalPlayer && id.netId == 1)
        {
            Vector3 move = playerControls.Player.Move.ReadValue<Vector2>();
            move = new Vector3(move.x * 0.1f, move.y * 0.1f, 0);
            transform.position = transform.position + move;
        }
        else if (isLocalPlayer && id.netId >= 2)
        {
            Vector3 move = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            move.z = 0;
            transform.position = move;
            Debug.Log(transform.position);
        }
    }

    void Update()
    {
        HandleMovement();

        //Pour passer au touche du Typing mode
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerInput.enabled = true;
            enabled = false;
        }
    }
}

