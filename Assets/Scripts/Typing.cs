using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.UI;

public class Typing : NetworkBehaviour
{
    private enum Letters
    {
        A = KeyCode.A,
        B = KeyCode.B,
        C = KeyCode.C,
        D = KeyCode.D,
        E = KeyCode.E,
        F = KeyCode.F,
        G = KeyCode.G,
        H = KeyCode.H,
        I = KeyCode.I,
        J = KeyCode.J,
        K = KeyCode.K,
        L = KeyCode.L,
        M = KeyCode.M,
        N = KeyCode.N,
        O = KeyCode.O,
        P = KeyCode.P,
        Q = KeyCode.Q,
        R = KeyCode.R,
        S = KeyCode.S,
        T = KeyCode.T,
        U = KeyCode.U,
        V = KeyCode.V,
        W = KeyCode.W,
        X = KeyCode.X,
        Y = KeyCode.Y,
        Z = KeyCode.Z,
        BackSpace = KeyCode.Backspace
    }
    private GameObject dirLight;
    NetworkIdentity id;
    public string[] words = new string[10];
    // [SyncVar(hook = "DisplayTargetWord")]
    private string word = "";
    private int index = 1;
    //[SyncVar(hook = "DisplayInputText")]
    private string text = "";
    private bool error = false;
    private bool finish = false;

    GameObject input;
    GameObject exemple;
    public TMP_Text targetWord;
    public TMP_Text inputWord;
    public Player player;
    public PlayerInput playerInput;
    public void TypingAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed || ctx.canceled || finish)
            return;

        foreach (Letters kcode in Enum.GetValues(typeof(Letters)))
        {
            if (Input.GetKey((KeyCode)kcode))
            {
                int last = text.Length - 1;
                if (kcode == Letters.BackSpace)
                {
                    if (last < 0)
                        break;
                    if (error)
                    {
                        text = text.Remove(text.IndexOf('<'));
                        error= false;
                    }
                    else
                        text = text.Remove(last);
                }
                else if(!error)
                    text += kcode.ToString();

                last = text.Length - 1;
                if (error || last < 0)
                    break;

                if (text[last] != word[last])
                {
                    char c = text[last];
                    text = text.Remove(last);
                    text += $"<color=#ff0000>{c}</color>";
                    error= true;
                }

                if (text == word)
                {
                    finish = true;
                    text = $"<color=#00bf00>{text}</color>";
                    StartCoroutine(Next());
                }
            }
        }
        // DisplayInputText(inputWord.text, text);
        //inputWord.text = text;
        CallRpcInput(text);
    }
    IEnumerator Next()
    {
        yield return new WaitForSeconds(.5f);
        finish = false;
        if (index < words.Length)
            word = words[index++];
        else
        {
            player.enabled = true;
            playerInput.enabled = false;
            CallRpcHide();
            CallRpcLight();
        }
        //targetWord.text = word;
        CallRpcTargetWord(word);
        text = "";
        //inputWord.text = text;
        CallRpcInput(text);
    }
    
    //Le client envoie l'input au serveur
    [Command]
    public void CallRpcInput(String input) {
        RpcChangeInput(input);
    }

    //Le client envoie le nouveau mot au serveur
    [Command]
    public void CallRpcTargetWord(String word) {
        RpcChangeTargetWord(word);
    }

    //Le serveur dit à tous les clients de changer l'input
    [ClientRpc]
    public void RpcChangeInput(String input) {
        inputWord.text = input;
    }

    //Le serveur dit à tous les clients de changer le mot
    [ClientRpc]
    public void RpcChangeTargetWord(String word) {
        targetWord.text = word;
    }

    private void Awake() {

        dirLight = GameObject.Find("LightEmpty").gameObject;
        dirLight.gameObject.GetComponent<Light>().enabled = false;
        id = GetComponent<NetworkIdentity>();
        
        word = words[0];
        exemple = GameObject.Find("Exemple").gameObject;
        input = GameObject.Find("Input").gameObject;
        targetWord = exemple.GetComponent<TextMeshProUGUI>();
        inputWord = input.GetComponent<TextMeshProUGUI>();
        targetWord.text = word;

            // exemple.transform.parent.gameObject.SetActive(false);
            // input.transform.parent.gameObject.SetActive(false);
        
    }

    private void Start()
    {
        
        // word = words[0];
        // GameObject exemple = GameObject.Find("Exemple").gameObject;
        // GameObject input = GameObject.Find("Input").gameObject;
        // targetWord = exemple.GetComponent<TextMeshProUGUI>();
        // inputWord = input.GetComponent<TextMeshProUGUI>();
        // targetWord.text = word;
        CallRpcHide();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Laaaaaaaaaaaaaaaa");
        CallRpcDisplay();
        player.CallRpcImmobile();
    } 
    
    [Command]
    public void CallRpcDisplay() {
        RpcDisplay();
    }

    [Command]
    public void CallRpcHide() {
        RpcHide();
    }

    [Command]
    public void CallRpcLight() {
        RpcLight();
    }

    [ClientRpc]
    public void RpcDisplay() {
        exemple.transform.parent.gameObject.GetComponent<Image>().enabled = true;
        exemple.transform.gameObject.GetComponent<TMPro.TextMeshProUGUI>().enabled = true;
        input.transform.parent.gameObject.GetComponent<Image>().enabled = true;
    }

    [ClientRpc]
    public void RpcHide() {
        exemple.transform.parent.gameObject.GetComponent<Image>().enabled = false;
        exemple.transform.gameObject.GetComponent<TMPro.TextMeshProUGUI>().enabled = false;
        input.transform.parent.gameObject.GetComponent<Image>().enabled = false;
    }

    [ClientRpc]
    public void RpcLight() {
        dirLight.gameObject.GetComponent<Light>().enabled = true;
    }
}
