using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static AbilityTable;
using static RequestTable;
using static UnityEngine.Networking.UnityWebRequest;

/// <summary>
/// 퍼즐 플레이에 관련된 기능을 관리
/// </summary>
public class PuzzleMgr : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public List<TextAsset> csvList;
    public GridLayoutGroup puzzleGrid;
    public RectTransform chipPanelRectTr;
    public EventSystem es;
    public Canvas canvas;
    public RawImage dragImg;
    public RawImage NotEnoughCredit;//추가
    public int chipCountX;
    public int chipCountY;
    public float chipSize;
    public PuzzleFrame puzzleFrame;
    public List<Texture> textureList = new List<Texture>();
    public int totalCost = 0; //추가
    public Text ReturnCost; //추가

    public List<ChipObj> chipList = new List<ChipObj>();
    public Dictionary<int, int> chipInventory = new Dictionary<int, int>();
    public Button makingDone;
    public Button Reset; //추가
    public GameSceneMgr mgr2;
    public RequiredAbilityObject requiredAbilityObject;
    public Transform requiredAbilityTextParent;
    public Action OnMakingDone;
    public Action OnReset;//추가
    [HideInInspector]
    public bool isTutorial = true;

    private RectTransform puzzleGridRectTr;
    private RectTransform dragImgRectTr;
    private RawImage background;
    private Puzzle currPuzzle;
    private ChipObj[,] chipFrameTable;
    private ChipObj currentSelectedChip;
    private CanvasScaler canvasScaler;
    private GraphicRaycaster ray;
    private PointerEventData ped;
    private bool isFromPuzzle;
    private Vector3 originDragImgRectTr;
    private List<PuzzleFrame> puzzleFrameList = new List<PuzzleFrame>();
    private Dictionary<string, RequiredAbilityObject> requiredAbilityObjectDic = new Dictionary<string, RequiredAbilityObject>();
    private Dictionary<string, int> currentChipDataDic = new Dictionary<string, int>();
    private List<GameObject> instantiatedChipList = new List<GameObject>();
    
    private List<string> satisfiedRequest = new List<string>();

    public class Puzzle
    {
        public int x;
        public int y;
        public List<Ability> requiredChipAbilityList = new List<Ability>();
        public PuzzleFrameData[,] frameDataTable;
    }

    [System.Serializable]
    public class PuzzleFrameData
    {
        public int patternNum = 0;
        public int x;
        public int y;
        public ChipObj chip;

        public PuzzleFrameData(int patternNum, int x, int y)
        {
            this.patternNum = patternNum;
            this.x = x;
            this.y = y;
        }
    }

    void Awake()
    {
        makingDone.onClick.AddListener(OnClickMakingDone);
        Reset.onClick.AddListener(OnClickReset);// 추가

        puzzleGridRectTr = puzzleGrid.GetComponent<RectTransform>();
        dragImgRectTr = dragImg.GetComponent<RectTransform>();
        background = puzzleGrid.GetComponent<RawImage>();
        ray = canvas.GetComponent<GraphicRaycaster>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        ped = new PointerEventData(es);
        //여기써도 되는지 물어보기
        NotEnoughCredit.gameObject.SetActive(false);
        // 테스트코드
        // StringBuilder sb = new StringBuilder();
        // int i = 0;
        // foreach (var chipframe in chipFrameTable)
        // {
        //     if (chipframe != null)
        //     {
        //         sb.Append(chipframe.chipKey).Append(", ");
        //     }
        //     else
        //     {
        //         sb.Append("0, ");
        //     }
        //     i++;
        //     if (i == 4)
        //     {
        //         sb.Append("\n");
        //         i = 0;
        //     }
        // }
        // Debug.Log(sb.ToString());
        // 테스트코드
    }

    void Update()
    {
        if (currentSelectedChip != null)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                var angle = dragImgRectTr.localEulerAngles;
                var pos = dragImgRectTr.localPosition;
                if (angle.z < 1)
                {
                    angle.z = 270;
                    pos.x = pos.x + (dragImgRectTr.sizeDelta.x + dragImgRectTr.sizeDelta.y) / 2;
                    pos.y = pos.y + (-dragImgRectTr.sizeDelta.y + dragImgRectTr.sizeDelta.x) / 2;
                }
                else if (angle.z < 91)
                {
                    angle.z = 0;
                    pos.x = pos.x + (dragImgRectTr.sizeDelta.y - dragImgRectTr.sizeDelta.x) / 2;
                    pos.y = pos.y + (dragImgRectTr.sizeDelta.x + dragImgRectTr.sizeDelta.y) / 2;
                }
                else if (angle.z < 181)
                {
                    angle.z = 90;
                    pos.x = pos.x + (-dragImgRectTr.sizeDelta.x - dragImgRectTr.sizeDelta.y) / 2;
                    pos.y = pos.y + (dragImgRectTr.sizeDelta.y - dragImgRectTr.sizeDelta.x) / 2;
                }
                else if (angle.z < 271)
                {
                    angle.z = 180;
                    pos.x = pos.x + (-dragImgRectTr.sizeDelta.y + dragImgRectTr.sizeDelta.x) / 2;
                    pos.y = pos.y + (-dragImgRectTr.sizeDelta.x - dragImgRectTr.sizeDelta.y) / 2;
                }
                dragImgRectTr.localEulerAngles = angle;
                dragImgRectTr.localPosition = pos;

                currentSelectedChip.RotateRight();
            }
        }
        
    }

    public void StartPuzzle()
    {
        // TODO: 청사진 정보에 맞게 chipSize, currPuzzle 세팅하기
        chipSize = 108;
        currPuzzle = GetPuzzle(0); // TODO: 청사진에 맞는 데이터 불러오기
        // currPuzzle.requiredChipAbilityList.Add(new ChipAbility("durability", 1));
        // currPuzzle.requiredChipAbilityList.Add(new ChipAbility("weight", 1));
        // currPuzzle.requiredChipAbilityList.Add(new ChipAbility("attack", 1));
        InstantiateRequiredAbilityText();
        SetPuzzle();
        AddChipToInventory_ForTest();
        RefreshChipPanel();
    }

    public void OnClickMakingDone()
    {
        if (CreditCheck()) {
            CommonTool.In.OpenConfirmPanel("제작을 완료하시겠습니까?", ExecuteMakingDone, CloseCompleteMaking);
        }
        else {
            NoCreditUI();
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        ped.position = eventData.pressPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        es.RaycastAll(ped, results);
        if (results.Count > 0)
        {
            ChipObj targetChip = null;
            foreach (var result in results)
            {
                if (result.gameObject.name.Contains("PuzzleFrame"))
                {
                    isFromPuzzle = true;
                    targetChip = result.gameObject.GetComponent<PuzzleFrame>().pfd.chip;
                    break;
                }
            }

            if (targetChip == null)
            {
                targetChip = results[0].gameObject.GetComponent<ChipObj>();
            }

            if (targetChip != null && targetChip.chipCount > 0)
            {
                if (!isFromPuzzle)
                {
                    targetChip.SaveOriginRow();
                }
                dragImg.texture = targetChip.image.texture;
                if (targetChip.originRow.Length == targetChip.rowNum)
                {
                    dragImgRectTr.sizeDelta = new Vector2(targetChip.rowNum, targetChip.colNum) * chipSize;
                }
                else
                {
                    dragImgRectTr.sizeDelta = new Vector2(targetChip.colNum, targetChip.rowNum) * chipSize;
                }

                var offset = new Vector3(canvasScaler.referenceResolution.x, canvasScaler.referenceResolution.y, 0) / 2;
                dragImgRectTr.localPosition = (Vector3)eventData.position - offset;
                dragImgRectTr.localEulerAngles = targetChip.rectTr.localEulerAngles;
                var angle = dragImgRectTr.localEulerAngles;
                var pos = originDragImgRectTr = dragImgRectTr.localPosition;
                if (angle.z < 1)
                {
                    pos.x = pos.x + (-dragImgRectTr.sizeDelta.x) / 2;
                    pos.y = pos.y + (+dragImgRectTr.sizeDelta.y) / 2;
                }
                else if (angle.z < 91)
                {
                    pos.x = pos.x + (-dragImgRectTr.sizeDelta.y) / 2;
                    pos.y = pos.y + (-dragImgRectTr.sizeDelta.x) / 2;
                }
                else if (angle.z < 181)
                {
                    pos.x = pos.x + (dragImgRectTr.sizeDelta.x) / 2;
                    pos.y = pos.y + (-dragImgRectTr.sizeDelta.y) / 2;
                }
                else if (angle.z < 271)
                {
                    pos.x = pos.x + (dragImgRectTr.sizeDelta.y) / 2;
                    pos.y = pos.y + (dragImgRectTr.sizeDelta.x) / 2;
                }
                dragImgRectTr.localPosition = pos;

                currentSelectedChip = targetChip;
                dragImg.gameObject.SetActive(true);
                targetChip.chipCount--;
                if (targetChip.chipCount <= 0)
                {
                    targetChip.gameObject.SetActive(false);
                }
                else
                {
                    targetChip.count.text = targetChip.chipCount.ToString();
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (currentSelectedChip != null)
        {
            dragImgRectTr.localPosition += (Vector3)eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        var angle = dragImgRectTr.localEulerAngles;
        if (currentSelectedChip != null)
        {
            dragImg.gameObject.SetActive(false);
            bool success = false;

            var x = (currentSelectedChip.rowNum - 1) / 2;
            var offset = (new Vector2(currentSelectedChip.rowNum - 1, currentSelectedChip.colNum - 1) * chipSize) / 2;
            ped.position = Input.mousePosition + new Vector3(-offset.x, offset.y, 0);
            List<RaycastResult> results = new List<RaycastResult>();
            es.RaycastAll(ped, results);
            if (results.Count > 0)
            {
                bool isChipPanel = false;
                foreach (var result in results)
                {
                    if (result.gameObject.name.Contains("ChipPanel"))
                    {
                        isChipPanel = true;
                        break;
                    }
                }

                if (isChipPanel)
                {
                    if (isFromPuzzle)
                    {
                        foreach (var c in chipFrameTable)
                        {
                            if (c.chipKey.Equals(currentSelectedChip.chipKey))
                            {
                                c.chipCount++;
                                c.gameObject.SetActive(true);
                                c.count.text = c.chipCount.ToString();
                                c.ResetCol();
                                break;
                            }
                        }

                        if (currentSelectedChip.chipCount <= 0)
                        {
                            DestroyImmediate(currentSelectedChip.gameObject);
                        }
                        var result = GetWeaponPowerResult();
                        success = true;
                        //<by_honeydora>
                        CreditAdd(-50);
                    }
                }
                else
                {
                    PuzzleFrame frame = null;
                    foreach (var result in results)
                    {
                        frame = result.gameObject.GetComponent<PuzzleFrame>();
                        if (frame != null) break;
                    }

                    if (frame != null)
                    {
                        var fittableFrames = GetFittableFrameList(currPuzzle.frameDataTable, currentSelectedChip, frame.pfd.x, frame.pfd.y);
                        if (fittableFrames != null)
                        {
                            var chipInstance = Instantiate(currentSelectedChip, chipPanelRectTr.transform);
                            chipInstance.name = currentSelectedChip.name;
                            chipInstance.gameObject.SetActive(true);
                            for (int i = 0; i < fittableFrames.Count; i++)
                            {
                                fittableFrames[i].chip = chipInstance;
                            }
                            chipInstance.countPlate.SetActive(false);
                            chipInstance.transform.parent = frame.transform;
                            if (chipInstance.originRow.Length == currentSelectedChip.rowNum)
                            {
                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.rowNum, chipInstance.colNum) * chipSize;
                            }
                            else
                            {
                                chipInstance.rectTr.sizeDelta = new Vector2(chipInstance.colNum, chipInstance.rowNum) * chipSize;
                            }
                            chipInstance.rectTr.anchoredPosition = Vector3.zero;
                            chipInstance.chipCount = 1;

                            chipInstance.rectTr.localEulerAngles = angle;
                            var pos = chipInstance.rectTr.localPosition;
                            if (angle.z < 1)
                            {
                            }
                            else if (angle.z < 91)
                            {
                                pos.y -= dragImgRectTr.rect.height * (chipInstance.colNum * 1f / chipInstance.rowNum);
                            }
                            else if (angle.z < 181)
                            {
                                pos.y -= dragImgRectTr.rect.height;
                                pos.x += dragImgRectTr.rect.width;
                            }
                            else if (angle.z < 271)
                            {
                                pos.x += dragImgRectTr.rect.width * (chipInstance.rowNum * 1f / chipInstance.colNum);
                            }
                            chipInstance.rectTr.localPosition = pos;

                            if (currentSelectedChip.chipCount <= 0 && isFromPuzzle)
                            {
                                DestroyImmediate(currentSelectedChip.gameObject);
                            }
                            var result = GetWeaponPowerResult();
                            success = true;
                            //<by_honeydora>
                            CreditAdd(+50);
                        }
                    }
                }
            }
            if (!success)
            {
                currentSelectedChip.chipCount++;
                if (currentSelectedChip.chipCount > 0)
                {
                    currentSelectedChip.gameObject.SetActive(true);
                }
                currentSelectedChip.count.text = currentSelectedChip.chipCount.ToString();
                currentSelectedChip.ResetCol();
            }
            currentSelectedChip = null;
        }
        isFromPuzzle = false;
    }

    private void SetPuzzle()
    {
        var width = puzzleGridRectTr.rect.width / currPuzzle.x;
        var height = puzzleGridRectTr.rect.height / currPuzzle.y;
        var size = width < height ? width : height;
        puzzleGrid.cellSize = new Vector2(size, size);
        puzzleGrid.constraintCount = currPuzzle.x;
        InstantiateFrames(currPuzzle.frameDataTable);
        background.texture = textureList[0];
    }

    private Puzzle GetPuzzle(int index)
    {
        Puzzle puzzle = new Puzzle();
        var lines = csvList[index].text.Split('\n');
        var lineList = new List<string>();
        foreach (var line in lines)
        {
            var trimLine = line.Trim();
            if (!string.IsNullOrEmpty(trimLine))
            {
                lineList.Add(trimLine);
            }
        }
        puzzle.y = lineList.Count;
        puzzle.x = lineList[0].Split(',').Length;
        puzzle.frameDataTable = new PuzzleFrameData[puzzle.y, puzzle.x];
        for (int i = 0; i < lineList.Count; i++)
        {
            var elements = lineList[i].Split(',');
            if (i == 0) puzzle.x = elements.Length;

            for (int j = 0; j < elements.Length; j++)
            {
                int targetNum = 0;
                if (!Int32.TryParse(elements[j], out targetNum))
                {
                    Debug.Log("퍼즐조각정보 로드 실패. puzzle" + index + ": " + i + ", " + j);
                    return null;
                }
                puzzle.frameDataTable[i, j] = new PuzzleFrameData(targetNum, j, i);
            }
        }

        return puzzle;
    }

    private void InstantiateFrames(PuzzleFrameData[,] frameDataTable)
    {
        foreach (var frameData in frameDataTable)
        {
            var frame = Instantiate(puzzleFrame, puzzleGrid.transform);
            frame.SetPuzzleFrameData(frameData);
            puzzleFrameList.Add(frame);
            if (frame.pfd.patternNum == 0)
            {
                var color = frame.image.color;
                color.a = 0;
                frame.image.color = color;
                continue;
            }
            frame.image.texture = textureList[frameData.patternNum];
        }
    }

    private void AddChipToInventory_ForTest()
    {
        // TODO: 현재 보유중인 칩에 맞게 칩이 추가될 수 있도록 수정
        // chipInventory.Add(1, 1);
        // chipInventory.Add(2, 1);
        // chipInventory.Add(3, 2);
        // chipInventory.Add(4, 1);
        // chipInventory.Add(5, 1);
        // chipInventory.Add(6, 1);
        // chipInventory.Add(7, 2);

        int[] chip_arr = new int[] { 8, 9, 10, 11 };
        int initialCount = 3; // 초기 개수 설정

        foreach (int key in chip_arr) {
            chipInventory[key] = initialCount; 
        }
    }

    private void RefreshChipPanel()
    {
        var sizeX = chipSize * chipCountX;
        var sizeY = chipSize * chipCountY;
        chipPanelRectTr.sizeDelta = new Vector2(sizeX, sizeY);

        chipFrameTable = new ChipObj[chipCountY, chipCountX];
        for (int i = 0; i < chipCountY; i++)
        {
            for (int j = 0; j < chipCountX; j++)
            {
                chipFrameTable[i, j] = new ChipObj();
            }
        }

        foreach (var key in chipInventory.Keys)
        {
            var chip = GetChipPrefab(key);
            if (chip)
            {
                var chipInstance = Instantiate(chip, chipPanelRectTr.transform);
                chipInstance.name = chip.name;
                instantiatedChipList.Add(chipInstance.gameObject);
                SetChipToPanel(chipInstance, chipInventory[key]);
            }
        }
    }

    private ChipObj GetChipPrefab(int key)
    {
        return chipList.Find(x => x.chipKey.Equals(key));
    }

    private void SetChipToPanel(ChipObj chip, int chipCount)
    {
        for (int i = 0; i < chipCountY; i++)
        {
            for (int j = 0; j < chipCountX; j++)
            {
                if (chipFrameTable[i, j] != null) continue;
                chipFrameTable[i, j] = chip;
                chip.count.text = chipCount.ToString();
                chip.chipCount = chipCount;
                chip.rectTr.sizeDelta = new Vector2(chipSize, chipSize);
                chip.rectTr.anchoredPosition = new Vector3(chipSize * j, -chipSize * i, 0);
                return;
            }
        }
        Destroy(chip.gameObject);
    }

    private List<PuzzleFrameData> GetFittableFrameList(PuzzleFrameData[,] targetTable, ChipObj chip, int x, int y)
    {
        List<PuzzleFrameData> pfdList = new List<PuzzleFrameData>();
        var table = chip.row;
        for (int i = 0; i < table.Length; i++)
        {
            for (int j = 0; j < table[i].col.Length; j++)
            {
                if (!table[i].col[j])
                {
                    continue;
                }

                if (targetTable[y + j, x + i].patternNum == 0)
                {
                    return null;
                }

                if (x + i >= currPuzzle.x || y + j >= currPuzzle.y)
                {
                    return null;
                }

                if (targetTable[y + j, x + i].chip != null && targetTable[y + j, x + i].chip.chipCount != 0)
                {
                    return null;
                }

                pfdList.Add(targetTable[y + j, x + i]);
            }
        }
        return pfdList;
    }

    private void InstantiateRequiredAbilityText()
    {
        foreach (var ability in GameMgr.In.currentBluePrint.requiredChipAbilityList)
        {
            var obj = Instantiate(requiredAbilityObject, requiredAbilityTextParent);
            obj.text.text = "0/" + ability.count;
            requiredAbilityObjectDic.Add(ability.abilityKey, obj);
        }
    }

    private bool GetWeaponPowerResult()
    {
        currentChipDataDic.Clear();
        foreach (var puzzleFrame in puzzleFrameList)
        {
            if (puzzleFrame.transform.childCount > 0)
            {
                var child = puzzleFrame.transform.GetChild(0);
                if (child)
                {
                    var chip = child.GetComponent<ChipObj>();
                    if (chip)
                    {
                        foreach (var ability in chip.chipAbilityList)
                        {
                            if (!currentChipDataDic.ContainsKey(ability.abilityKey))
                            {
                                currentChipDataDic.Add(ability.abilityKey, ability.count);
                                continue;
                            }
                            currentChipDataDic[ability.abilityKey] += ability.count;
                        }
                    }
                }
            }
        }

        bool success = true;
        foreach (var key in requiredAbilityObjectDic.Keys)
        {
            var splittedData = requiredAbilityObjectDic[key].text.text.Split('/');
            int targetNum = -1;
            if (!Int32.TryParse(splittedData[1], out targetNum))
            {
                Debug.Log("파싱 실패: " + targetNum);
                return false;
            }
            int result = 0;
            if (currentChipDataDic.ContainsKey(key))
            {
                result = currentChipDataDic[key];
                if (result < targetNum)
                {
                    success = false;
                }
            }
            else
            {
                result = 0;
                success = false;
            }
            requiredAbilityObjectDic[key].text.text = result + "/" + splittedData[1];
        }

        return success;
    }

    // private ChipObj GetChipAtPos(Vector2 pos)
    // {
    //     pos /= chipSize;
    //     return chipFrameTable[(int)-pos.y, (int)pos.x];
    // }

    [ContextMenu("LogPuzzleFrameDatas")]
    private void LogPuzzleFrameDatas()
    {
        StringBuilder sb2 = new StringBuilder();
        int i = 0;
        foreach (var frame in currPuzzle.frameDataTable)
        {
            if (frame != null && frame.chip != null)
            {
                sb2.Append(frame.chip.chipKey).Append(", ");
            }
            else
            {
                sb2.Append("0, ");
            }
            i++;
            if (i == 8)
            {
                sb2.Append("\n");
                i = 0;
            }
        }
        Debug.Log(sb2.ToString());
    }

    /// <추가기능>
    /// 제작중.
    /// </by_hondora>
    //초기화 로직--

    public void OnClickReset()
    {
        mgr2.popupChatPanel.SetActive(false);

        if (OnReset != null) {
            OnReset.Invoke();
            OnReset = null;
        }
        //퍼즐 리스타트
        RemakePuzzle();
    }

    private void RemakePuzzle()
    {
        ClearPuzzleGrid();
        StartPuzzle();
    }

    private void ClearPuzzleGrid()
    {
        CreditReset();
        //필요능력과 퍼즐 제거
        foreach (var key in requiredAbilityObjectDic.Keys) {
            Destroy(requiredAbilityObjectDic[key].gameObject);
        }
        requiredAbilityObjectDic.Clear();

        foreach (var obj in puzzleFrameList) {
            Destroy(obj.gameObject);
        }
        puzzleFrameList.Clear();
        foreach (var obj in instantiatedChipList) {
            Destroy(obj);
        }
        chipInventory.Clear();
        
    }

    //필요 크레딧!--

    //크레딧 체크하는 함수
    private bool CreditCheck()
    {
        int currentCredits = GameMgr.In.credit;//추가
        return currentCredits >= CreditNeed() ? true : false;
    }
    //필요 크레딧 계산 

    //현재는 Drag Drob할때마다 추가됨
    //CreditAdd(칩셋cost) 또는 칩셋 계산해서 가격 Add하면됨
    //TODO: 기획 듣고 칩셋 가격 관련 로직 바꿀필요!
    private void CreditAdd(int value)
    {
        totalCost += value;
        if(totalCost <= 0) {
            totalCost = 0;
        }
        CreditText();
    }

    // totalCost 초기화 함수
    private void CreditReset()
    {
        totalCost = 0;
        CreditText();
    }

    // 필요 크레딧을 반환 함수
    private int CreditNeed()
    {
        return totalCost;
    }

    private void CreditText()
    {
        ReturnCost.text = " " + CreditNeed().ToString();
        if (CreditCheck()) {
            ReturnCost.color = Color.white;
        }
        else {
            ReturnCost.color = Color.red;
        }
        
    }

    //돈 부족 UI
    IEnumerator ShowNoCreditUI()
    {
        NotEnoughCredit.gameObject.SetActive(true); // UI 활성화
        //TODO: audio 삐 - 소리 추가 필요
        yield return new WaitForSeconds(1); // 1초 대기
        NotEnoughCredit.gameObject.SetActive(false); // UI 비활성화
    }

    private void NoCreditUI()
    {
        if (canvas.gameObject.activeInHierarchy) {
            StartCoroutine(ShowNoCreditUI()); 
        }
        else {
            canvas.gameObject.SetActive(true);
            StartCoroutine(ShowNoCreditUI());
        }
    }

    //취소: 필요없을듯
    private void CloseCompleteMaking()
    {
    }

    // 진짜 제작 완료 로직 실행
    private void ExecuteMakingDone()
    {
        bool isPuzzleSuccess = GetWeaponPowerResult();
        mgr2.orderState = isPuzzleSuccess ? GameSceneMgr.OrderState.Succeed : GameSceneMgr.OrderState.Failed;

        foreach (var key in requiredAbilityObjectDic.Keys) {
            Destroy(requiredAbilityObjectDic[key].gameObject);
        }
        requiredAbilityObjectDic.Clear();

        foreach (var obj in puzzleFrameList) {
            Destroy(obj.gameObject);
        }

        foreach (var obj in instantiatedChipList) {
            Destroy(obj);
        }

        puzzleFrameList.Clear();
        chipInventory.Clear();
        isTutorial = false;

        if (OnMakingDone != null) {
            OnMakingDone.Invoke();
            OnMakingDone = null;
        }


        GameMgr.In.credit -= totalCost;
        mgr2.goldText.text = GameMgr.In.credit.ToString();
        totalCost =0;
        NotEnoughCredit.gameObject.SetActive(false);
        mgr2.popupChatPanel.SetActive(false);
    }
}
