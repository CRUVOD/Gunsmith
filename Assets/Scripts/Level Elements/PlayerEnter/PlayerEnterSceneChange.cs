using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEnterSceneChange : MonoBehaviour
{
    public string SceneName;
    private bool hasBeenTriggered;
    private Player player;

    private void Awake()
    {
        hasBeenTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !hasBeenTriggered)
        {
            player = collision.gameObject.GetComponent<Player>();
            hasBeenTriggered = true;
            SceneManager.LoadScene(SceneName);
        }
    }
}
