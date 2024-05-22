using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverModeSampleSceneManager : MonoBehaviour
{
    public PuzzleMgr puzzleMgr;
    public RectTransform gageRectTr;
    public float maxPosX;
    public float maxTime;
    public int maxPuzzleCnt;
    public GameObject particleParentObj;
    public GameObject normalParticleParentObj;
    public GameObject resultParticleParentObj;
    public GameObject greenParticleParentObj;
    public GameObject yellowParticleParentObj;
    public GameObject redParticleParentObj;
    public GameObject whiteParticleParentObj;
    public GameObject blueParticleParentObj;
    public GameObject purpleParticleParentObj;

    private Image gageImage;
    private bool isPuzzleCompleted;
    private bool isGamePlaying;
    private int succeedPuzzleCnt;
    private Coroutine resultParticleRoutine;
    private List<WeaponDataTable.BluePrint> orderableBlueprintList = new List<WeaponDataTable.BluePrint>();

    private IEnumerator Start()
    {
        gageImage = gageRectTr.GetComponent<Image>();

        SetTableDatasForFeverMode();

        foreach (var bpc in GameMgr.In.weaponDataTable.bluePrintCategoryList)
        {
            foreach (var bp in bpc.bluePrintList)
            {
                if (bp.createEnable) orderableBlueprintList.Add(bp);
            }
        }

        float gageWidth = gageRectTr.sizeDelta.x;
        float minPosX = maxPosX - Mathf.Abs(gageWidth);
        float currentPercent = 1;
        Vector2 currentPos = gageRectTr.anchoredPosition;
        currentPos.x = maxPosX;
        gageRectTr.anchoredPosition = currentPos;

        var feverModeRoutine = StartCoroutine(StartFeverMode());

        while (currentPos.x >= minPosX)
        {
            if (!isGamePlaying)
            {
                yield break;
            }

            float percentAtThisFrame = Time.deltaTime / maxTime;
            currentPos.x -= percentAtThisFrame * gageWidth;
            currentPercent -= percentAtThisFrame;
            gageRectTr.anchoredPosition = currentPos;

            if (currentPercent > 0.5f)
            {
                if (gageImage.color != Color.green)
                {
                    gageImage.color = Color.green;
                    greenParticleParentObj.SetActive(true);
                    yellowParticleParentObj.SetActive(false);
                    redParticleParentObj.SetActive(false);
                }
            }
            else if (currentPercent < 0.15f)
            {
                if (gageImage.color != Color.red)
                {
                    gageImage.color = Color.red;
                    greenParticleParentObj.SetActive(false);
                    yellowParticleParentObj.SetActive(false);
                    redParticleParentObj.SetActive(true);
                }
            }
            else
            {
                if (gageImage.color != Color.yellow)
                {
                    gageImage.color = Color.yellow;
                    greenParticleParentObj.SetActive(false);
                    yellowParticleParentObj.SetActive(true);
                    redParticleParentObj.SetActive(false);
                }
            }

            yield return null;
        }

        StopCoroutine(feverModeRoutine);
        EndFeverMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            puzzleMgr.OnClickMakingDone();
        }

        // TODO: 칩셋 드래그 상태로 만들기
        // if (Input.GetKeyDown(KeyCode.Q))
        // {

        // }
    }

    private void SetTableDatasForFeverMode()
    {
        var chipList = GameMgr.In.chipTable.chipList;
        foreach (var chip in chipList)
        {
            chip.createEnable = true;
        }

        var bpcList = GameMgr.In.weaponDataTable.bluePrintCategoryList;
        foreach (var bpc in bpcList)
        {
            if (!(bpc.categoryKey.Equals("t_sword") || bpc.categoryKey.Equals("t_blunt"))) continue;

            foreach (var bp in bpc.bluePrintList)
            {
                bp.createEnable = true;
            }
        }
    }

    private void EndFeverMode()
    {
        puzzleMgr.isFeverMode = false;
        isGamePlaying = false;

        particleParentObj.SetActive(false);
        CommonTool.In.OpenAlertPanel("완성한 퍼즐 갯수: " + succeedPuzzleCnt);
        succeedPuzzleCnt = 0;
    }

    private void StartNewFeverModePuzzle()
    {
        var index = UnityEngine.Random.Range(0, orderableBlueprintList.Count);

        var key = orderableBlueprintList[index].bluePrintKey;
        GameMgr.In.currentBluePrint = GameMgr.In.GetWeapon(key);
        puzzleMgr.StartFeverModePuzzle();
    }

    private void OnFeverModeMakingDone(int result)
    {
        isPuzzleCompleted = true;

        normalParticleParentObj.SetActive(false);
        resultParticleParentObj.SetActive(true);
        switch (result)
        {
            case 1:
                whiteParticleParentObj.SetActive(true);
                blueParticleParentObj.SetActive(false);
                purpleParticleParentObj.SetActive(false);
                break;
            case 2:
                whiteParticleParentObj.SetActive(false);
                blueParticleParentObj.SetActive(true);
                purpleParticleParentObj.SetActive(false);
                succeedPuzzleCnt++;
                break;
            case 3:
                whiteParticleParentObj.SetActive(false);
                blueParticleParentObj.SetActive(false);
                purpleParticleParentObj.SetActive(true);
                succeedPuzzleCnt++;
                break;
        }

        if (resultParticleRoutine != null)
        {
            StopCoroutine(resultParticleRoutine);
        }
        resultParticleRoutine = StartCoroutine(ReturnToNormalParticles());
    }

    private IEnumerator StartFeverMode()
    {
        puzzleMgr.isFeverMode = true;
        puzzleMgr.isTutorial = false;
        isGamePlaying = true;

        particleParentObj.SetActive(true);

        for (int i = 0; i < maxPuzzleCnt; i++)
        {
            StartNewFeverModePuzzle();
            puzzleMgr.OnMakingDone += OnFeverModeMakingDone;

            isPuzzleCompleted = false;
            while (!isPuzzleCompleted)
            {
                if (!isGamePlaying)
                {
                    yield break;
                }
                yield return null;
            }
        }

        EndFeverMode();
    }

    private IEnumerator ReturnToNormalParticles()
    {
        yield return new WaitForSeconds(1);
        normalParticleParentObj.SetActive(true);
        resultParticleParentObj.SetActive(false);
        resultParticleRoutine = null;
    }

    [ContextMenu("Test")]
    void Test()
    {
        Debug.Log(gageRectTr.anchoredPosition.x);
        Debug.Log(gageRectTr.anchoredPosition.y);
    }

    [ContextMenu("Test2")]
    void Test2()
    {
        float gageWidth = gageRectTr.sizeDelta.x;
        float minPosX = maxPosX - Mathf.Abs(gageWidth);
        Vector3 currentPos = gageRectTr.localPosition;
        currentPos.x = maxPosX;
        gageRectTr.localPosition = currentPos;
    }
}
