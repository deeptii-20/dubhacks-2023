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
    public Dictionary<UIType, GameObject> UI;

    public UIType currUI;
    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        GameOverlay.SetActive(true);
        EnemyDialogue.SetActive(false);
        VillagerDialogue.SetActive(false);

        UI = new Dictionary<UIType, GameObject>()
        {
            { UIType.Default, GameOverlay },
            { UIType.EnemyDialogue, EnemyDialogue },
            { UIType.VillagerDialogue, VillagerDialogue },
        };
        currUI = UIType.Default;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowEnemyDialogue(string dialogueText, GameObject enemy) {
        currUI = UIType.EnemyDialogue;
        StartCoroutine(ShowDialogue(dialogueText, enemy));
    }

    IEnumerator ShowDialogue(string dialogueText, GameObject enemy) {
        // wait 1.5 sec
        yield return new WaitForSeconds(1.0f);

        // show dialogue box
        UI[currUI].transform.Find("Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        UI[currUI].SetActive(true);
        isPaused = true;

        // wait for users to select an option -> hide dialogue box and act accordingly
        while (true) {
            if (Input.GetAxis("Positive") > 0) {
                UI[currUI].SetActive(false);
                // play capture animation
                break;
            }
             if (Input.GetAxis("Negative") > 0) {
                UI[currUI].SetActive(false);
                Destroy(enemy);
                break;
            }
            yield return null;
        }
        yield return null;
    }
}
