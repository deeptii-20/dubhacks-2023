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

    public IEnumerator ShowEnemyDialogue(string dialogueText, GameObject enemy) {
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
                // play capture animation
                break;
            }
            if (Input.GetAxis("Cancel") > 0) {
                EnemyDialogue.SetActive(false);
                enemy.GetComponent<GhostController>().Die();
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator ShowVillagerDialogue(string dialogueText, GameObject villager) {
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
                break;
            }
            yield return null;
        }
        yield return null;
    }
}
