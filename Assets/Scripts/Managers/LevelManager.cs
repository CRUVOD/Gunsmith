using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The level manager will handle enemy spawning, level changing, enemy groups, rooms
/// player respawns, and will keep references to the player and enemies in the level
/// The level manager is different in each scene, it doesnt carry over from scene
/// to scene like audiomanager
/// </summary>
public class LevelManager : MonoBehaviour, ExtendedEventListener<GameEvent>
{
    public static LevelManager instance;

    [Header("Basic Fields")]
    public Player player;

    [Header("Player Death")]
    /// the delay, in seconds, before displaying the death screen once the player is dead
    [Tooltip("the delay, in seconds, before displaying the death screen once the player is dead")]
    public float DelayBeforeDeathScreen = 1f;

    [Header("Game Level Management")]
    /// Doors to lock the player into each room
    public GameObject Doors;

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

        InitialiseLevelManager();
    }

    private void InitialiseLevelManager()
    {
        if (player == null)
        {
            Debug.Log("ayo we don't have a player rn?");
        }
    }

    internal void CharacterPause(bool state)
    {
        if (state)
        {
            player.ConditionState = CharacterStates.CharacterConditions.Paused;
        }
        else
        {
            //Change to revert to previous state later
            player.ConditionState = CharacterStates.CharacterConditions.Normal;
        }
    }

    /// <summary>
    /// Triggers the death screen display after a short delay
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PlayerDead()
    {
        yield return new WaitForSeconds(DelayBeforeDeathScreen);

        UIManager.instance.SetDeathScreen(true);
    }

    /// <summary>
    /// Reset the level
    /// </summary>
    protected virtual void ResetLevel()
    {
        UIManager.instance.SetDeathScreen(false);
        GameManager.instance.Reset();
        SceneManager.LoadScene("Pre-Built Level Test");
    }

    /// <summary>
    /// To enable or disable the doors in the level
    /// </summary>
    /// <param name="active"></param>
    public void ActivateDoors(bool active)
    {
        Doors.SetActive(active);
    }

    #region Events

    /// <summary>
    /// Catches GameEvents and acts on them
    /// </summary>
    /// <param name="GameEvent">GameEvent event.</param>
    public virtual void OnExtendedEvent(GameEvent GameEvent)
    {
    }

    /// <summary>
    /// OnEnable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.ExtendedEventStartListening<GameEvent>();
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.ExtendedEventStartListening<GameEvent>();
    }

    #endregion
}
