using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public HealthBar bossHealthBar;
    public TextMeshProUGUI nameText;

    [HideInInspector]
    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public HealthBar GetHealthBar()
    {
        return bossHealthBar;
    }

    public void Show(Enemy bossEnemy, string bossName)
    {
        bossEnemy.OnDeath += BossDeath;
        nameText.text = bossName;
        animator.SetBool("IsOpen", true);
    }

    public void BossDeath(Enemy bossEnemy)
    {
        animator.SetBool("IsOpen", false);
    }
}
