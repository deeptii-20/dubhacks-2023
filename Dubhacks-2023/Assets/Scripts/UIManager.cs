using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIType {
    Default = 0,
    EnemyDialogue = 1,
    VillagerDialogue = 2,
}

public class UIManager : MonoBehaviour
{
    public GameObject GameOverlay;
    public GameObject TwoResponseDialogue;
    public GameObject OneResponseDialogue;

    public UIType currUI;
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        GameOverlay.SetActive(true);
        TwoResponseDialogue.SetActive(false);
        OneResponseDialogue.SetActive(false);

        currUI = UIType.Default;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void UpdateHealthOverlay(float currHealth, bool isDrain) {
        if (isDrain) {
            // TODO: update UI with drain animation
            GameOverlay.transform.Find("Health Text").GetComponent<TMP_Text>().text = "Health: " + currHealth;
        } else {
            // TODO: update UI with non-drain animation
            GameOverlay.transform.Find("Health Text").GetComponent<TMP_Text>().text = "Health: " + currHealth;
        }
    }

    public IEnumerator ShowTwoResponseDialogue(string dialogueText, GameObject speaker, GameObject player) {
        currUI = UIType.EnemyDialogue;
        isPaused = true;

        // wait 0.5 sec, then show dialogue box
        yield return new WaitForSeconds(0.5f);
        TwoResponseDialogue.transform.Find("Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        TwoResponseDialogue.SetActive(true);

        // wait for users to select an option -> hide dialogue box and act accordingly
        while (true) {
            if (Input.GetAxis("Submit") > 0) {
                TwoResponseDialogue.SetActive(false);
                InteractYesResponse(player, speaker);
                isPaused = false;
                break;
            }
            if (Input.GetAxis("Cancel") > 0) {
                TwoResponseDialogue.SetActive(false);
                InteractNoResponse(player, speaker);
                isPaused = false;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator ShowOneResponseDialogue(string dialogueText, GameObject speaker, GameObject player) {
        currUI = UIType.VillagerDialogue;
        isPaused = true;

        // wait 0.5 sec, then show dialogue box
        yield return new WaitForSeconds(0.5f);
        OneResponseDialogue.transform.Find("Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        OneResponseDialogue.SetActive(true);

        // wait for users to select an option -> hide dialogue box
        while (true) {
            if (Input.GetAxis("Submit") > 0) {
                OneResponseDialogue.SetActive(false);
                InteractResponse(player, speaker);
                break;
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
        
    }

    public void InteractNoResponse(GameObject player, GameObject speaker) {
        switch(speaker.tag) {
            case "Enemy":
                // kill the ghost
                speaker.GetComponent<GhostController>().Die();
                break;
            case "Villager":
                // bite the villager
                player.GetComponent<PlayerController>().KillVillager(speaker);
                break;
            default:
                // do nothing
                break;
        }
    }

    public void InteractResponse(GameObject player, GameObject speaker) {
        switch(speaker.tag) {
            case "Enemy":
                // do nothing
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
            default:
                // do nothing
                break;
        }
        isPaused = false;
    }
}
