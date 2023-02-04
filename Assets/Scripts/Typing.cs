using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Typing : MonoBehaviour
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

    public string[] words = new string[10];
    private string word;
    private int index = 1;
    private string text = "";
    private bool error = false;
    private bool finish = false;

    public TMP_Text targetWord;
    public TMP_Text inputWord;

    public void TypingAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed || ctx.canceled || finish)
            return;

        foreach (Letters kcode in Enum.GetValues(typeof(Letters)))
        {
            if (Input.GetKey((KeyCode)kcode))
            {
                int last = text.Length - 1;
                if (kcode == Letters.BackSpace && last > 0)
                {
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

                if (error)
                    return;

                last = text.Length - 1;
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
        inputWord.text = text;
    }
    IEnumerator Next()
    {
        yield return new WaitForSeconds(.5f);
        finish = false;
        if(index<words.Length)
            word = words[index++];
        targetWord.text = word;
        text = "";
        inputWord.text = text;

    }

    private void Start()
    {
        word = words[0];
        targetWord.text = word;
    }
}
