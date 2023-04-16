using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    private Queue<string> sentences;
    private Dialogue currentDialogue;
    private Sprite currentPotrait;

    public DialogueBox dialogueBox;
    public Sprite defaultPotrait;

    private Player player;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetPlayer();
        sentences = new Queue<string>();
    }

    private void GetPlayer()
    {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        if (searchResults.Length == 1)
        {
            Player result;
            if (searchResults[0].TryGetComponent<Player>(out result))
            {
                player = result;
                return;
            }
            else
            {
                Debug.Log("Failed to get player component");
            }
        }
        else if (searchResults.Length > 1)
        {
            Debug.LogWarning("More than one player?");
        }
        else
        {
            Debug.LogWarning("The player is missing ;-;");
        }
    }

    public void StartDialogue(Dialogue dialogue, Sprite potrait)
    {
        dialogueBox.Show();
        
        currentDialogue = dialogue;
        player.IgnoreInput(true);

        if (potrait == null)
        {
            currentPotrait = defaultPotrait;
        }
        else
        {
            currentPotrait = potrait;
        }

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
        UIManager.instance.ToggleInGameUI(false);
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueBox.SetText(currentDialogue.name, sentence, currentPotrait);
    }

    public void EndDialogue()
    {
        dialogueBox.Hide();
        UIManager.instance.ToggleInGameUI(true);
        player.IgnoreInput(false);
    }
}
