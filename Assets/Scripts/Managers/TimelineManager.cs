using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// The timeline manager will handle all timelines in a scene
/// The manager will stop player inputs and AI behaviour once a timeline starts playing,
/// and resumes their inputs when a timeline is done playing.
/// </summary>
public class TimelineManager : MonoBehaviour
{
    [HideInInspector]
    public static TimelineManager instance;

    private Player player;
    //Plays the first timeline onLoad/Awake
    public bool playFirstOnAwake;
    public TimelineReference[] timelineList;

    private Dictionary<string, TimelineReference> timelines;

    private void Awake()
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

    private void Start()
    {
        FindPlayer();
        ConstructTimelineDictionary();
        //play the first timeline on awake
        if (timelineList.Length > 0 && playFirstOnAwake)
        {
            PlayTimeline(0);
        }
    }

    private void FindPlayer()
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

    /// <summary>
    /// Generates the dictionary to refer to the timeline assets
    /// </summary>
    private void ConstructTimelineDictionary()
    {
        timelines = new Dictionary<string, TimelineReference>();

        foreach(TimelineReference timelineReference in timelineList)
        {
            timelines.Add(timelineReference.ID, timelineReference);
        }
    }

    /// <summary>
    /// Plays the timeline with the supplied ID from the dictionary
    /// </summary>
    /// <param name="id"></param>
    public void PlayTimeline(string id)
    {
        //Hide UI
        UIManager.instance.ToggleInGameUI(false);
        if (timelines.ContainsKey(id))
        {
            timelines[id].timelineDirector.Play();
            //Stops the previous hold if neccessary
            StopAllCoroutines();
            //Start new hold
            if (timelines[id].triggerDialogue)
            {
                StartCoroutine(HoldPlayerAndAI((float) timelines[id].timelineDirector.duration, timelines[id].dialogue));
            }
            else
            {
                StartCoroutine(HoldPlayerAndAI((float) timelines[id].timelineDirector.duration));
            }
        }
        else
        {
            Debug.Log(id + " timeline does not exist");
        }
    }

    /// <summary>
    /// Access the timeline array directly, and play the timline at the index
    /// </summary>
    /// <param name="index"></param>
    public void PlayTimeline(int index)
    {
        //Hide UI
        UIManager.instance.ToggleInGameUI(false);

        if (index < timelineList.Length)
        {
            timelineList[index].timelineDirector.Play();
            //Stops the previous hold if neccessary
            StopAllCoroutines();
            //Start new hold
            if (timelineList[index].triggerDialogue)
            {
                StartCoroutine(HoldPlayerAndAI((float)timelineList[index].timelineDirector.duration, timelineList[index].dialogue));
            }
            else
            {
                StartCoroutine(HoldPlayerAndAI((float)timelineList[index].timelineDirector.duration));
            }
        }
        else
        {
            Debug.Log("Timeline index" + index + " out of range");
        }
    }

    /// <summary>
    /// Holds the player and AI until timeline completes
    /// </summary>
    /// <returns></returns>
    private IEnumerator HoldPlayerAndAI(float duration)
    {
        player.IgnoreInput(true);
        yield return new WaitForSeconds(duration);
        player.IgnoreInput(false);
    }

    /// <summary>
    /// Holds the player and AI until timeline completes but with dialogue trigger
    /// </summary>
    /// <returns></returns>
    private IEnumerator HoldPlayerAndAI(float duration, DialogueTrigger dialogue)
    {
        player.IgnoreInput(true);
        yield return new WaitForSeconds(duration);
        player.IgnoreInput(false);
        dialogue.TriggerDialogue();
    }
}
