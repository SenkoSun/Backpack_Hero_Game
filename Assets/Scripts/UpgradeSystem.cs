using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class SimpleUpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeData
    {
        public string statName; // Название для отладки
        public Slider slider;
        public TMP_Text costText;
        public Button upgradeButton;
        public int[] costs;
        [HideInInspector] public int currentLevel;
    }

    public UpgradeData[] upgrades;

    void Start()
    {
        // Инициализация всех улучшений
        for (int i = 0; i < upgrades.Length; i++)
        {
            SetupUpgrade(i);
        }
    }

    void SetupUpgrade(int index)
    {
        string[] names_characteristic = { "attack", "defense", "attack_speed", "crit_damage", "crit_chance" };
        var upgrade = upgrades[index];
        
        // Загрузка сохраненного уровня
        upgrade.currentLevel = PlayerPrefs.GetInt($"Upgrade_{index}_Level", 0);
        PlayerStats.charactiristis[names_characteristic[index]] = upgrade.currentLevel;
        Debug.Log($"{names_characteristic[index]} - {upgrade.statName}: {upgrade.currentLevel}");
        upgrade.slider.value = upgrade.currentLevel;
        
        
        // Обновляем отображение цены
        UpdateCostText(upgrade);
        
        // Назначаем обработчик кнопки
        upgrade.upgradeButton.onClick.AddListener(() => TryUpgrade(index));
    }

    void TryUpgrade(int index)
    {
        string[] names_characteristic = { "attack", "defense", "attack_speed", "crit_damage", "crit_chance" };
        var upgrade = upgrades[index];
        
        // Проверка границ массива
        if (upgrade.currentLevel >= upgrade.costs.Length)
        {
            Debug.Log($"{upgrade.statName}: Max. level reached");
            return;
        }

        int requiredGold = upgrade.costs[upgrade.currentLevel];
        
        // Проверка денег
        if (GoldManager.Instance.Gold < requiredGold)
        {
            Debug.Log($"{upgrade.statName}: Not enough gold");
            return;
        }

        // Списание денег
        GoldManager.Instance.AddGold(-requiredGold);
        
        // Улучшение
        upgrade.currentLevel++;
        upgrade.slider.value = upgrade.currentLevel;
        PlayerStats.charactiristis[names_characteristic[index]] = upgrade.currentLevel;
        PlayerPrefs.SetInt($"Upgrade_{index}_Level", upgrade.currentLevel);
        
        // Обновление интерфейса
        UpdateCostText(upgrade);
        
        Debug.Log($"{upgrade.statName}: Upgrade level {upgrade.currentLevel}");
    }

    void UpdateCostText(UpgradeData upgrade)
    {        
        if (upgrade.currentLevel < upgrade.costs.Length)
        {
            upgrade.costText.text = $"Cost: {upgrade.costs[upgrade.currentLevel]}";
            upgrade.upgradeButton.interactable = true;
        }
        else
        {
            upgrade.costText.text = "Max. level";
            upgrade.upgradeButton.interactable = false;
        }
    }
}