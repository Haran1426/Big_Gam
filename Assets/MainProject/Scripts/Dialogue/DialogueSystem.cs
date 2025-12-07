using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    public GameObject dialogueUI;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public GameObject[] optionButtons;

    public PropData currentProp;
    public DialogueData currentDialogue;
    public int currentDialogueIndex = 0;

    private bool isWritingText = false;
    public bool isDialogueActive = false;
    private Coroutine typingCoroutine;

    private bool isRandomOneShot = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isDialogueActive) return;
        if (currentDialogue == null) return;
        if (currentDialogue.dialogueType == DialogueType.Branch) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isWritingText)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                }

                isWritingText = false;
                dialogueText.text = currentDialogue.dialogueText;
            }
            else
            {
                if (isRandomOneShot)
                {
                    GuageManager.Instance.AddGuageValue(GuageType.Evironment, currentDialogue.evironmentGuageChange);
                    GuageManager.Instance.AddGuageValue(GuageType.Economy, currentDialogue.economyGuageChange);
                    GuageManager.Instance.AddGuageValue(GuageType.Happiness, currentDialogue.happinessGuageChange);
                    GuageManager.Instance.AddGuageValue(GuageType.Security, currentDialogue.securityGaugeChange);

                    GuageManager.Instance.UpdateGuageUI();

                    isDialogueActive = false;
                    dialogueUI.SetActive(false);
                    isRandomOneShot = false;
                }
                else
                {
                    NextDialogue();
                }
            }
        }
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;

        GuageManager.Instance.AddGuageValue(GuageType.Evironment, currentDialogue.evironmentGuageChange);
        GuageManager.Instance.AddGuageValue(GuageType.Economy, currentDialogue.economyGuageChange);
        GuageManager.Instance.AddGuageValue(GuageType.Happiness, currentDialogue.happinessGuageChange);
        GuageManager.Instance.AddGuageValue(GuageType.Security, currentDialogue.securityGaugeChange);

        GuageManager.Instance.UpdateGuageUI();

        if (currentProp != null && currentDialogueIndex < currentProp.dialogues.Length)
        {
            DialogueData dialogue = currentProp.dialogues[currentDialogueIndex];
            StartDialogue(dialogue);
        }
        else
        {
            isDialogueActive = false;
            dialogueUI.SetActive(false);
        }
    }

    public void StartDialogue(PropData propData)
    {
        isDialogueActive = true;
        dialogueUI.SetActive(true);

        currentProp = propData;
        currentDialogueIndex = 0;

        DialogueData dialogue = propData.dialogues[currentDialogueIndex];
        StartDialogue(dialogue);
    }

    public void StartDialogue(DialogueData dialogue)
    {
        isRandomOneShot = false;

        if (dialogue.dialogueType == DialogueType.Random)
        {
            if (currentProp == null || currentProp.dialogues == null || currentProp.dialogues.Length <= 1)
            {
                return;
            }

            int randomIndex = Random.Range(1, currentProp.dialogues.Length);
            DialogueData randomDialogue = currentProp.dialogues[randomIndex];

            isRandomOneShot = true;

            isDialogueActive = true;
            dialogueUI.SetActive(true);

            currentDialogue = randomDialogue;
            characterNameText.text = randomDialogue.characterName;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            foreach (var btn in optionButtons)
            {
                btn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                btn.SetActive(false);
            }

            typingCoroutine = StartCoroutine(Co_WriteText(randomDialogue.dialogueText));
            return;
        }

        isDialogueActive = true;
        dialogueUI.SetActive(true);

        currentDialogue = dialogue;
        characterNameText.text = dialogue.characterName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        foreach (var btn in optionButtons)
        {
            btn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            btn.SetActive(false);
        }

        if (dialogue.dialogueType == DialogueType.Normal)
        {
            typingCoroutine = StartCoroutine(Co_WriteText(dialogue.dialogueText));
        }
        else if (dialogue.dialogueType == DialogueType.Branch)
        {
            typingCoroutine = StartCoroutine(Co_WriteText(dialogue.dialogueText));

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < dialogue.branches.Length)
                {
                    optionButtons[i].SetActive(true);
                    TMP_Text optionText = optionButtons[i].GetComponentInChildren<TMP_Text>();
                    optionText.text = dialogue.branches[i].optionText;

                    int index = i;
                    var button = optionButtons[i].GetComponent<UnityEngine.UI.Button>();
                    button.onClick.AddListener(() =>
                    {
                        foreach (GameObject btn in optionButtons)
                        {
                            btn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                            btn.SetActive(false);
                        }

                        StartDialogue(dialogue.branches[index].nextDialogue);
                    });
                }
                else
                {
                    optionButtons[i].SetActive(false);
                }
            }
        }
    }

    public IEnumerator Co_WriteText(string fullText)
    {
        isWritingText = true;
        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isWritingText = false;
        typingCoroutine = null;
    }
}
