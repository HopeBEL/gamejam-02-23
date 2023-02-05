using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCanva : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject exemple;
    public GameObject input;
    void Start()
    {
        exemple = GameObject.Find("Exemple").gameObject;
        input = GameObject.Find("Input").gameObject;

        exemple.transform.parent.gameObject.SetActive(false);
        input.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Je suis rentré");

        //if (other.gameObject.CompareTag("Player"))
        exemple.transform.parent.gameObject.SetActive(true);
        input.transform.parent.gameObject.SetActive(true);
    }
}
