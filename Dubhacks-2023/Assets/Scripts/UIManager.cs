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
        StartCoroutine(ShowDialogue(dialogueText));
    }

    IEnumerator ShowDialogue(string dialogueText) {
        UI[currUI].transform.Find("Dialogue Text").gameObject.GetComponent<TMP_Text>().text = dialogueText;
        UI[currUI].SetActive(true);
        isPaused = true;

        // wait for users to select an option
        while (true) {
            if (Input.GetAxis("Yes") > 0) {
                Debug.Log("Selected yes");
                break;
            }
            if (Input.GetAxis("No") > 0) {
                Debug.Log("Selected no");
                break;
            }
        }
        yield return null;
    }
}
