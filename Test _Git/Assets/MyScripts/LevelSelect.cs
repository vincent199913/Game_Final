using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelSelect : MonoBehaviour
{
    private bool isInRange;
    public UnityEvent levelSelection;

    public Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        isInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInRange)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("select level");
                levelSelection.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            ani.SetBool("inRange", isInRange);
            Debug.Log("player is in range.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            ani.SetBool("inRange", isInRange);
            Debug.Log("player is out of range.");
        }
    }
}
