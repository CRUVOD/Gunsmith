using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    //Hides cursor if true
    public bool ReplaceCursor;
    //The image of the new crosshair
    public SpriteRenderer crosshairSprite;
    //Cooldown UI
    public Image coolDownCircle;

    //Cooldown UI variables
    private float coolDownTime;
    private float coolDownTimer;
    private bool inCoolDown;

    private void Start()
    {
        SetCursor();

        // Display no cooldown circle at the start
        coolDownCircle.fillAmount = 0;
    }

    private void Update()
    {
        HandleCrosshair();
        HandleCoolDown();
    }

    private void SetCursor()
    {
        if (ReplaceCursor)
        {
            Cursor.visible = false;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void HandleCoolDown()
    {
        if (inCoolDown)
        {
            coolDownTimer -= Time.deltaTime;
            coolDownCircle.fillAmount = 1 - (coolDownTimer / coolDownTime);

            if (coolDownTimer <= 0)
            {
                //Exit cooldown
                ResetCoolDownUI();
            }
        }
    }

    private void HandleCrosshair()
    {
        if (ReplaceCursor)
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cursorPos;
        }
    }

    /// <summary>
    /// Start a new cooldown display on the circle, just input time in float
    /// </summary>
    /// <param name="time"></param>
    public void StartCoolDownUI(float time)
    {
        coolDownTime = time;
        coolDownTimer = time;
        inCoolDown = true;
        coolDownCircle.fillAmount = 1;
    }

    /// <summary>
    /// Show a cooldown, it can start in the middle of a longer cooldown
    /// </summary>
    /// <param name="time"></param>
    /// <param name="totalTime"></param>
    public void SetCoolDownUI(float time, float totalTime)
    {
        coolDownTimer = time;
        coolDownTimer = totalTime;
        inCoolDown = true;
        coolDownCircle.fillAmount = 1;
    }

    /// <summary>
    /// Stop counting down the cooldown
    /// </summary>
    public void ResetCoolDownUI()
    {
        coolDownCircle.fillAmount = 0;
        inCoolDown = false;
    }
}
