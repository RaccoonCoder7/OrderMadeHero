using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterSystem : MonoBehaviour
{
    // ���� �ɼ� â
    [System.Serializable]
    public class FilterOption
    {
        public string abilityKey; // ChipTable�� abilityKey�� �����ؾ���!
        public GameObject optionPrefab; // abilityKey�� �´� prefab_button
        public bool isActive = false; // �ɼ� on/off üũ

    }

    public List<FilterOption> filterOptions; // ���� ������ ����� ���� �ɼ� ���
    public PuzzleMgr puzzleMgr;
    public GameObject defaultPrefab; // �⺻ �̹��� default prefab
    public int totalOptions = 12; // ��ü ���� �ɼ��� ��(�ʿ�� �������ּ���)
    private List<string> activeFilters = new List<string>();


    void Start()
    {
        InitializeFilterOptions();
    }

    // ���� �ɼ� �ʱ�ȭ �Լ�
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

    // ���ǵ� ���� �ɼ� ���� �Լ�
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

        // puzzleMgr.FilterChipsByAbilities(activeFilters);
    }

    // �⺻ �̹��� �ɼ� ���� �Լ�
    void CreateDefaultOption()
    {
        Instantiate(defaultPrefab, transform);
    }

    
}
