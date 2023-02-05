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
    private PlayerInput playerInput;
    // public MouseAnim MA;
    NetworkIdentity id;
    public float jumpForce = 10f;
    public float speed = 5f;
    private Behaviour[] componentsToDisable = new Behaviour[2];
    private Rigidbody2D playerRb;

    private bool isFacingRight = true;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    Vector3 move;
    public GameObject triggerCanva;

    private Animator animator;
    private void Awake()
    {
        playerControls = new PlayerInputAction();
    }

    private void Start()
    {  
        playerInput = gameObject.GetComponent<PlayerInput>();
        id = gameObject.GetComponent<NetworkIdentity>();
        componentsToDisable[0] = gameObject.GetComponent<PlayerInput>();
        componentsToDisable[1] = gameObject.GetComponent<Player>();
        Debug.Log("Id client : " + id.netId);
        //Donne le Sprite2D de Diana au joueur 1
        if (id.netId == 1)
        {
            Destroy(gameObject.GetComponent<Light>());
            animator = gameObject.GetComponent<Animator>();
            gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite1;
            playerRb = gameObject.GetComponent<Rigidbody2D>();
        }
        //Donne le Sprite2D d'un papillon au joueur 2
        else if (id.netId >= 2)
        {
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            Destroy(gameObject.GetComponent<Animator>());
            gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite2;
            Cursor.visible = false;
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

    void Update() {
        if (isLocalPlayer && id.netId == 1)
        {
        //Debug.Log(IsGrounded());
        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
        }
        }
    }

    void HandleMovement()
    {
        //field provided by the NetworkBehaviour
        //returns true if the object represents the player on the local machine
        //on ne veut détecter les mouvements que du joueur sur la machine locale

        //Joueur 1
        if (isLocalPlayer && id.netId == 1)
        {
            animator.SetBool("isWalking", true);
            move = playerControls.Player.Move.ReadValue<Vector2>();

            if (move.x == 0) {
                animator.SetBool("isWalking", false);
            }
            //move = new Vector3(move.x * 0.1f, move.y * 0.1f, 0);
           //Debug.Log("Move : " + move);
            playerRb.velocity =new Vector2(move.x * speed, playerRb.velocity.y);
            //transform.position = transform.position + move;


            FlipPlayer();
        }
        //Joueur 2
        else if (isLocalPlayer && id.netId >= 2)
        {
            //Le joueur 2 joue à la souris
            Vector3 move = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            move.z = -8;
            transform.position = move;
            // MA.OnMouseOver();
            // MA.OnMouseExit();
            //Debug.Log(transform.position);
        }
    }

    public bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void FlipPlayer() {
        if (isFacingRight && move.x < 0 || !isFacingRight && move.x > 0) {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    void FixedUpdate()
    {
        HandleMovement();

        //Pour passer au touche du Typing mode
        //Seul le joueur 1 peut déclencher cet état
        if (Input.GetKeyDown(KeyCode.Escape) && id.netId == 1)
        {
            // playerInput.enabled = true;
            // enabled = false;
            CallRpcImmobile();
        }
    }


    [Command]
    public void CallRpcImmobile() {
        RpcImmobile();
        //TargetEnable(id.connectionToClient);
    }

    [ClientRpc]
    public void RpcImmobile() {
            this.playerInput.enabled = true;
            Debug.Log("Player : je me désactive..");
            this.enabled = false;
            //for (int i = 0; i < componentsToDisable.Length; i++) {
             //   componentsToDisable[i].enabled = false;//!componentsToDisable[i].enabled;
        }
        
    }


