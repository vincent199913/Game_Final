using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour
{
    static SelectCharacter selectRecorder;
    public int character;
    
    void Awake()
    {
        if(selectRecorder == null)
        {
            selectRecorder = this;
            DontDestroyOnLoad(this);
            character = 1;
        }
        else if(this != selectRecorder)
        {
            Destroy(gameObject);
        }
    }
}
