using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    private Queue<Sentence> sentences;
    private DialogueTrigger currentDialogue;
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
        sentences = new Queue<Sentence>();
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

    public void StartDialogue(DialogueTrigger dialogueTrigger)
    {
        dialogueBox.Show();
        currentDialogue = dialogueTrigger;
        player.IgnoreInput(true);
        currentDialogue.Setup();

        if (currentDialogue.dialogue.potraits[0] == null)
        {
            currentPotrait = defaultPotrait;
        }
        else
        {
            currentPotrait = currentDialogue.dialogue.potraits[0];
        }

        sentences.Clear();
        foreach (Sentence sentence in dialogueTrigger.dialogue.sentences)
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
            ReleaseCurrentDialogue();
            return;
        }

        Sentence sentence = sentences.Dequeue();

        if (sentence.voice > currentDialogue.dialogue.names.Length || sentence.voice > currentDialogue.dialogue.potraits.Length)
        {
            //If voice number is greater than the saved amount of names or potraits, skip this sentence
            Debug.Log("Dialogue voice index out of range");
            return;
        }

        dialogueBox.SetText(currentDialogue.dialogue.names[sentence.voice], sentence.content, currentDialogue.dialogue.potraits[sentence.voice]);
    }

    public void ReleaseCurrentDialogue()
    {
        currentDialogue.Release();
    }

    public void EndDialogue()
    {
        dialogueBox.Hide();
        UIManager.instance.ToggleInGameUI(true);
        player.IgnoreInput(false);
    }
}
