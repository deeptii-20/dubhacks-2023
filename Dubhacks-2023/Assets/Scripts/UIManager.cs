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
    public GameObject EnemyDialogue;
    public GameObject VillagerDialogue;

    public UIType currUI;
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        GameOverlay.SetActive(true);
        EnemyDialogue.SetActive(false);
        VillagerDialogue.SetActive(false);

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
    public void UpdateGhostsOverlay(int numCapturedGhosts) {
        GameOverlay.transform.Find("Ghosts Text").GetComponent<TMP_Text>().text = "Ghosts: " + numCapturedGhosts;
    }

    public IEnumerator ShowEnemyDialogue(string dialogueText, GameObject enemy, GameObject player) {
        currUI = UIType.EnemyDialogue;
        isPaused = true;

        // wait 0.5 sec, then show dialogue box
        yield return new WaitForSeconds(0.5f);
        EnemyDialogue.transform.Find("Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        EnemyDialogue.SetActive(true);

        // wait for users to select an option -> hide dialogue box and act accordingly
        while (true) {
            if (Input.GetAxis("Submit") > 0) {
                EnemyDialogue.SetActive(false);
                player.GetComponent<PlayerController>().CaptureGhost(enemy);
                isPaused = false;
                break;
            }
            if (Input.GetAxis("Cancel") > 0) {
                EnemyDialogue.SetActive(false);
                player.GetComponent<PlayerController>().KillGhost(enemy);
                isPaused = false;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator ShowVillagerDialogue(string dialogueText) {
        currUI = UIType.VillagerDialogue;
        isPaused = true;

        // wait 0.5 sec, then show dialogue box
        yield return new WaitForSeconds(0.5f);
        VillagerDialogue.transform.Find("Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        VillagerDialogue.SetActive(true);

        // wait for users to select an option -> hide dialogue box
        while (true) {
            if (Input.GetAxis("Submit") > 0) {
                VillagerDialogue.SetActive(false);
                isPaused = false;
                break;
            }
            yield return null;
        }
        yield return null;
    }
}
