using System.Collections;
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
    public float testValue;
    public BluePrintTable bluePrintTable;
    public WeaponDataTable weaponDataTable;
    public OrderTable orderTable;
    public Image testImg;
    public Image testImg2;
    public AnimationCurve curve;
    public SpriteAnimation sa;


    /// <summary>
    /// 씬에 있는 모든 버튼의 Navigation을 비활성화
    /// </summary>
    [ContextMenu("DisableNavigationOfButtons")]
    public void DisableNavigationOfButtons()
    {
        var buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (var btn in buttons)
        {
            var nav = btn.navigation;
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;
        }
    }

    /// <summary>
    /// orderCondition 딕셔너리화
    /// </summary>
    [ContextMenu("MoveOrderConditionDatas")]
    public void MoveOrderConditionDatas()
    {
        foreach (var order in orderTable.orderList)
        {
            // if (!string.IsNullOrEmpty(order.orderCondition))
            // {
            //     order.orderConditionDictionary.Add(order.orderCondition, false);
            // }
        }
    }

    /// <summary>
    /// 히어로/빌런 주문 설정
    /// </summary>
    [ContextMenu("SetHeroVillainOrders")]
    public void SetHeroVillainOrders()
    {
        foreach (var order in orderTable.orderList)
        {
            if (order.camp == OrderTable.Camp.Hero || order.camp == OrderTable.Camp.Villain)
            {
                order.orderEnable = false;
                // order.orderCondition = "tendency";
            }
        }
    }

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(StartShopInAnim());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(StartShopOutAnim());
        }
    }

    private IEnumerator StartShopInAnim()
    {
        StartCoroutine(sa.StartAnim());

        while (sa.textureIndex < (sa.textureList.Count / 3))
        {
            yield return null;
        }

        testImg.transform.DOLocalMoveY(-770, 0.5f).SetEase(Ease.InOutQuart);

        while (sa.textureIndex < (sa.textureList.Count * 2 / 3))
        {
            yield return null;
        }

        testImg2.gameObject.SetActive(true);
        testImg2.transform.DOScale(Vector3.one, 0.5f);
    }

    private IEnumerator StartShopOutAnim()
    {
        StartCoroutine(sa.StartAnim(true));

        while (sa.textureIndex >= (sa.textureList.Count * 2 / 3))
        {
            yield return null;
        }

        testImg2.transform.DOScale(Vector3.zero, 0.5f);
        testImg.transform.DOLocalMoveY(-540, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                testImg2.gameObject.SetActive(false);
            });
    }
}
