using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelInfoDisplayer : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI description;
    [SerializeField]
    private TextMeshProUGUI powerLevel;
    public TextMeshProUGUI recommendedPowerLevel;
    public TextMeshProUGUI weaponChestCount;
    public TextMeshProUGUI attachmentChestCount;
    public Button levelLaunchButton;

    [SerializeField]
    private Image weaponChestImage;
    [SerializeField]
    private Image attachmentChestImage;

    private void Start()
    {
        //We set the display to be empty at start
        EmptyDisplay();
    }

    public void UpdateLevelDisplay(LevelInfo levelInfo)
    {
        if (levelInfo.SceneName == "PlayerHub")
        {
            PlayerHubLevelDisplay(levelInfo);
        }
        else
        {
            LevelDisplay(levelInfo);
        }
    }

    private void EmptyDisplay()
    {
        weaponChestImage.gameObject.SetActive(false);
        attachmentChestImage.gameObject.SetActive(false);
        levelLaunchButton.gameObject.SetActive(false);
        weaponChestCount.gameObject.SetActive(false);
        attachmentChestCount.gameObject.SetActive(false);
        powerLevel.gameObject.SetActive(false);

        levelName.text = "";
        description.text = "";
        recommendedPowerLevel.text = "";
    }

    private void PlayerHubLevelDisplay(LevelInfo playerHubInfo)
    {
        //This is the player hub, we disable the chest sprites, text and button
        weaponChestImage.gameObject.SetActive(false);
        attachmentChestImage.gameObject.SetActive(false);
        levelLaunchButton.gameObject.SetActive(false);
        weaponChestCount.gameObject.SetActive(false);
        attachmentChestCount.gameObject.SetActive(false);
        powerLevel.gameObject.SetActive(true);

        levelName.text = playerHubInfo.LevelName;
        description.text = playerHubInfo.description;
        recommendedPowerLevel.text = playerHubInfo.powerLevel.ToString();
    }

    private void LevelDisplay(LevelInfo levelInfo)
    {
        //Set sprites and button active
        weaponChestImage.gameObject.SetActive(true);
        attachmentChestImage.gameObject.SetActive(true);
        levelLaunchButton.gameObject.SetActive(true);
        weaponChestCount.gameObject.SetActive(true);
        attachmentChestCount.gameObject.SetActive(true);
        powerLevel.gameObject.SetActive(true);

        //Update button function
        levelLaunchButton.onClick.RemoveAllListeners();
        levelLaunchButton.onClick.AddListener(delegate { LaunchLevel(levelInfo.SceneName); });

        //Regular level info
        levelName.text = levelInfo.LevelName;
        description.text = levelInfo.description;
        recommendedPowerLevel.text = levelInfo.powerLevel.ToString();
        weaponChestCount.text = "x" + levelInfo.weaponChestCount;
        attachmentChestCount.text = "x" + levelInfo.attachmentChestCount;
    }

    public void LaunchLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
