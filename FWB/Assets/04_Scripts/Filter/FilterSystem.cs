using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterSystem : MonoBehaviour
{
    // 필터 옵션 창
    [System.Serializable]
    public class FilterOption
    {
        public string abilityKey; // ChipTable의 abilityKey와 동일해야함!
        public GameObject optionPrefab; // abilityKey에 맞는 prefab_button
        public bool isActive = false; // 옵션 on/off 체크

    }

    public List<FilterOption> filterOptions; // 게임 내에서 사용할 필터 옵션 목록
    public PuzzleMgr puzzleMgr;
    public GameObject defaultPrefab; // 기본 이미지 default prefab
    public int totalOptions = 12; // 전체 필터 옵션의 수(필요시 조정해주세요)
    private List<string> activeFilters = new List<string>();


    void Start()
    {
        InitializeFilterOptions();
    }

    // 필터 옵션 초기화 함수
    void InitializeFilterOptions()
    {
        for (int i = 0; i < totalOptions; i++) {
            if (i < filterOptions.Count) {
                CreateOption(filterOptions[i]);
            }
            else {
                CreateDefaultOption();
            }
        }
    }

    // 정의된 필터 옵션 생성 함수
    void CreateOption(FilterOption filterOption)
    {
        GameObject optionInstance = Instantiate(filterOption.optionPrefab, transform);
        Button button = optionInstance.GetComponent<Button>(); 
        Image image = optionInstance.GetComponent<Image>(); 

        image.color = filterOption.isActive ? Color.white : Color.gray;

        button.onClick.AddListener(() => {
            ToggleFilter(filterOption, button, image);
        });
    }

    public void ToggleFilter(FilterOption filterOption, Button button, Image image)
    {
        filterOption.isActive = !filterOption.isActive;
        image.color = filterOption.isActive ? Color.white : Color.gray;

        if (filterOption.isActive) {
            if (!activeFilters.Contains(filterOption.abilityKey)) {
                activeFilters.Add(filterOption.abilityKey);
            }
        }
        else {
            activeFilters.Remove(filterOption.abilityKey);
        }

        puzzleMgr.FilterChipsByAbilities(activeFilters);
    }

    // 기본 이미지 옵션 생성 함수
    void CreateDefaultOption()
    {
        Instantiate(defaultPrefab, transform);
    }

    
}
