using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static ChipTable;
using static WeaponDataTable;

/// <summary>
/// 여러가지 개발시에 테스트용으로 사용되는 기능들을 모아 둔 스크립트
/// </summary>
public class Test : MonoBehaviour
{
    public float width;
    public float height;
    public float posX;
    public float posY;
    public BluePrintTable bluePrintTable;
    public WeaponDataTable weaponDataTable;
    public Image testImg;
    public AnimationCurve curve;


    /// <summary>
    /// 화면 포커스 기능 테스트
    /// </summary>
    [ContextMenu("SetFocus")]
    public void SetFocus()
    {
        CommonTool.In.SetFocus(new Vector2(posX, posY), new Vector2(width, height));
    }

    /// <summary>
    /// 테이블 데이터 복제
    /// </summary>
    [ContextMenu("CopyBluePrintData")]
    public void CopyBluePrintData()
    {
        foreach (var bp in bluePrintTable.bluePrintList)
        {
            BluePrintCategory category = new BluePrintCategory();
            category.categoryKey = bp.bluePrintKey.Replace("b_", "c_");
            BluePrint newBp = new BluePrint();
            newBp.bluePrintKey = bp.bluePrintKey;
            newBp.name = bp.name;
            foreach (var ability in bp.requiredChipAbilityList)
            {
                ChipAbility newAbility = new ChipAbility();
                newAbility.abilityKey = ability.abilityKey;
                newAbility.count = ability.count;
                newBp.requiredChipAbilityList.Add(newAbility);
            }
            category.bluePrintList.Add(newBp);
            weaponDataTable.bluePrintCategoryList.Add(category);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            testImg.transform.DOLocalMoveY(200, 1).SetEase(Ease.InExpo);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            testImg.transform.DOLocalMoveY(-200, 1).SetEase(Ease.InExpo);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            testImg.transform.DOLocalMoveY(200, 1).SetEase(Ease.InCubic);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            testImg.transform.DOLocalMoveY(-200, 1).SetEase(Ease.InCubic);
        }
    }
}
