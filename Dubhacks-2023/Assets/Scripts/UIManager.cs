using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIType {
    Default = 0,
    EnemyDialogue = 1,
    VillagerDialogue = 2,
    GameOver = 3,
}

public class UIManager : MonoBehaviour
{
    // in-game
    public GameObject GameOverlay;
    public GameObject TwoResponseDialogue;
    public GameObject OneResponseDialogue;

    // post-game
    public GameObject GameOverScreen;
    public string winText = "Victory";
    public string loseText = "You Lost...";

    public string winFlavorText = "You have brought peace to the village.";
    public string noVillagersFlavorText = "It seems that all of the villagers went missing somehow";
    public string angryFlavorText = "You were caught killing villagers";


    // ui state
    public UIType currUI;
    public bool isPaused;
    public float baseInputCooldown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GameOverlay.SetActive(true);
        TwoResponseDialogue.SetActive(false);
        OneResponseDialogue.SetActive(false);
        GameOverScreen.SetActive(false);

        currUI = UIType.Default;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {  

    }
    
    public void UpdateHealthOverlay(float currHealth, float baseHealth, bool isDrain) {
        float healthPercentage = currHealth / baseHealth;
        // TODO: diff animations for drain and no drain
        GameObject baseHealthBar = GameOverlay.transform.Find("Base Health Bar").gameObject;
        GameObject currHealthBar = GameOverlay.transform.Find("Curr Health Bar").gameObject;
        currHealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthPercentage * baseHealthBar.GetComponent<RectTransform>().sizeDelta.x, baseHealthBar.GetComponent<RectTransform>().sizeDelta.y);
    }

    public IEnumerator ShowGameOverScreen(bool isWin, string flavorText) {
        GameOverScreen.transform.Find("Game Over Text").gameObject.GetComponent<TMP_Text>().text = isWin ? winText : loseText;
        GameOverScreen.transform.Find("Flavor Text").gameObject.GetComponent<TMP_Text>().text = flavorText;

        while (true) {
            GameOverlay.SetActive(false);
            OneResponseDialogue.SetActive(false);
            TwoResponseDialogue.SetActive(false);
            GameOverScreen.SetActive(true);
            isPaused = true;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator ShowTwoResponseDialogue(string dialogueText, GameObject speaker, GameObject player) {
        currUI = UIType.EnemyDialogue;
        isPaused = true;

        // wait 0.5 sec, then show dialogue box
        yield return new WaitForSeconds(0.5f);
        TwoResponseDialogue.transform.Find("Background/Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        TwoResponseDialogue.SetActive(true);
        GameOverlay.SetActive(false);

        // wait for users to select an option -> hide dialogue box and act accordingly
        while (true) {
            isPaused = true;
            if (Input.GetAxis("Submit") > 0) {
                TwoResponseDialogue.SetActive(false);
                GameOverlay.SetActive(true);
                InteractYesResponse(player, speaker);
                isPaused = false;
                break;
            }
            if (Input.GetAxis("Cancel") > 0) {
                TwoResponseDialogue.SetActive(false);
                GameOverlay.SetActive(true);
                InteractNoResponse(player, speaker);
                isPaused = false;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator ShowOneResponseDialogue(string[] dialogueText, GameObject speaker, GameObject player) {
        currUI = UIType.VillagerDialogue;
        isPaused = true;

        // wait 0.5 sec, then show dialogue box
        yield return new WaitForSeconds(0.5f);
        OneResponseDialogue.transform.Find("Background/Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText[0];
        OneResponseDialogue.SetActive(true);
        GameOverlay.SetActive(false);

        // wait for users to continue
        int dialogueIdx = 0;
        while (true) {
            isPaused = true;
            if (Input.GetButtonDown("Submit")) {
                // len 2 -> from 0 to < 1
                if (dialogueIdx < dialogueText.Length - 1) {
                    // if there's another dialogue, show it
                    dialogueIdx++;
                    OneResponseDialogue.transform.Find("Background/Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText[dialogueIdx];
                } else {
                    // otherwise close the dialogue box
                    OneResponseDialogue.SetActive(false);
                    GameOverlay.SetActive(true);
                    InteractResponse(player, speaker);
                    isPaused = false;
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
        yield return null;
    }

    public void InteractYesResponse(GameObject player, GameObject speaker) {
        switch(speaker.tag) {
            case "Enemy":
                // capture the ghost
                player.GetComponent<PlayerController>().CaptureGhost(speaker);
                break;
            case "Villager":
                // bite the villager
                player.GetComponent<PlayerController>().KillVillager(speaker);
                break;
            default:
                // do nothing
                break;
        }
        isPaused = false;
    }

    public void InteractNoResponse(GameObject player, GameObject speaker) {
        switch(speaker.tag) {
            case "Enemy":
                // do nothing
                break;
            case "Villager":
                // bite the villager
                player.GetComponent<PlayerController>().KillVillager(speaker);
                break;
        }
        isPaused = false;
    }
    public void InteractResponse(GameObject player, GameObject speaker) {
        switch(speaker.tag) {
            case "Enemy":
                // do nothing
                StartCoroutine(
                    ShowTwoResponseDialogue(
                        "Bring the ghost to the cemetery?",
                        speaker,
                        player
                ));
                break;
            case "Villager":
                // if villager is suspicious or player health is low, bring up bite prompt
                if (speaker.GetComponent<VillagerController>().vstate == VillagerState.Suspicious || 
                    player.GetComponent<PlayerController>().currHealth <= player.GetComponent<PlayerController>().baseHealth * 0.5f) {
                    StartCoroutine(
                        ShowTwoResponseDialogue(
                            "Bite the villager to replenish health?",
                            speaker,
                            player
                    ));
                }
                break;
            case "OldMan": 
                // if angry, game over (lose)
                if (speaker.GetComponent<OldManController>().currState == OldManState.Angry) {
                    StartCoroutine(ShowGameOverScreen(false, angryFlavorText));
                }
                // if no villagers, game over (lose)
                else if (speaker.GetComponent<OldManController>().numVillagers <= 0) {
                    StartCoroutine(ShowGameOverScreen(false, noVillagersFlavorText));
                }
                // if old man state = win, show game over screen
                else if (speaker.GetComponent<OldManController>().currState == OldManState.Win) {
                    StartCoroutine(ShowGameOverScreen(true, winFlavorText));
                }
                // if all ghosts captured, show win text
                else if (speaker.GetComponent<OldManController>().numCapturedGhosts >= speaker.GetComponent<OldManController>().totalNumGhosts) {
                    speaker.GetComponent<OldManController>().currState = OldManState.Win;
                    StartCoroutine(ShowOneResponseDialogue(
                        speaker.GetComponent<OldManController>().winMonument,
                        speaker,
                        player
                    ));
                }
                break;
        }
        isPaused = false;
    }
}
