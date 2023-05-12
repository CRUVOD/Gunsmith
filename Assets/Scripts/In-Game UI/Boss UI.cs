using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public HealthBar bossHealthBar;
    public TextMeshProUGUI nameText;

    public HealthBar GetHealthBar()
    {
        return bossHealthBar;
    }

    public void Show(Enemy bossEnemy, string bossName)
    {
        bossEnemy.OnDeath += BossDeath;
        nameText.text = bossName;
    }

    public void BossDeath(Enemy bossEnemy)
    {

    }
}
