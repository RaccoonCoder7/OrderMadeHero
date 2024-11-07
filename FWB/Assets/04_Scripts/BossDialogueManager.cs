using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class DialogueEntry
{
    public string chapter;
    public int bossIndex;
    public int puzzleIndex;
    public string result;
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

public class BossDialogueManager : MonoBehaviour
{
    public TextAsset dialogueDataFile;
    private DialogueData dialogueData;
    public BossBattleManager battleManager;
    public Text teamDialogue;
    public Text bossDialogue;

    void Awake()
    {
        LoadDialogues();
    }

    void LoadDialogues()
    {
        if (dialogueDataFile != null)
        {
            dialogueData = JsonUtility.FromJson<DialogueData>(dialogueDataFile.text);
        }
        else
        {
            Debug.LogError("Dialogue data file is missing or not assigned.");
        }
    }

    public IEnumerator ShowDialogue(string chapter, int bossIndex, int puzzleIndex, string result)
    {
        var entry = dialogueData.dialogues.Find(d => d.chapter == chapter && d.bossIndex == bossIndex && d.puzzleIndex == puzzleIndex && d.result == result);

        if (entry != null)
        {
            battleManager.isGamePlaying = false;
            battleManager.ToggleCanvasInteractable(false);

            foreach (var line in entry.dialogueLines)
            {
                battleManager.ShowDialogueBox(line.speaker);

                if (line.speaker == "Team")
                {
                    teamDialogue.text = line.text;
                }
                else if (line.speaker == "Boss")
                {
                    bossDialogue.text = line.text;
                }

                yield return new WaitForSeconds(2);
            }

            yield return new WaitForSeconds(1);
            battleManager.HideDialogueBox();
            battleManager.ToggleCanvasInteractable(true);
        }
        else
        {
            Debug.LogError($"No dialogue entry found for chapter {chapter}, bossIndex {bossIndex}, puzzleIndex {puzzleIndex}, result {result}");
        }
    }

    public IEnumerator ShowDialogueAndContinue(string chapter, int bossIndex, int puzzleIndex, string result, Action onDialogueComplete)
    {
        yield return ShowDialogue(chapter, bossIndex, puzzleIndex, result);
        onDialogueComplete?.Invoke();
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<DialogueEntry> dialogues;
    }
}
