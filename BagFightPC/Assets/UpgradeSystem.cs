using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        var upgrade = upgrades[index];
        
        // Загрузка сохраненного уровня
        upgrade.currentLevel = PlayerPrefs.GetInt($"Upgrade_{index}_Level", 0);
        upgrade.slider.value = upgrade.currentLevel;
        
        // Обновляем отображение цены
        UpdateCostText(upgrade);
        
        // Назначаем обработчик кнопки
        upgrade.upgradeButton.onClick.AddListener(() => TryUpgrade(index));
    }

    void TryUpgrade(int index)
    {
        var upgrade = upgrades[index];
        
        // Проверка границ массива
        if (upgrade.currentLevel >= upgrade.costs.Length)
        {
            Debug.Log($"{upgrade.statName}: Максимальный уровень достигнут");
            return;
        }

        int requiredGold = upgrade.costs[upgrade.currentLevel];
        
        // Проверка денег
        if (GoldManager.Instance.Gold < requiredGold)
        {
            Debug.Log($"{upgrade.statName}: Недостаточно золота");
            return;
        }

        // Списание денег
        GoldManager.Instance.AddGold(-requiredGold);
        
        // Улучшение
        upgrade.currentLevel++;
        upgrade.slider.value = upgrade.currentLevel;
        PlayerPrefs.SetInt($"Upgrade_{index}_Level", upgrade.currentLevel);
        
        // Обновление интерфейса
        UpdateCostText(upgrade);
        
        Debug.Log($"{upgrade.statName}: Улучшено до уровня {upgrade.currentLevel}");
    }

    void UpdateCostText(UpgradeData upgrade)
    {
        if (upgrade.currentLevel < upgrade.costs.Length)
        {
            upgrade.costText.text = $"Цена: {upgrade.costs[upgrade.currentLevel]}";
            upgrade.upgradeButton.interactable = true;
        }
        else
        {
            upgrade.costText.text = "Макс. уровень";
            upgrade.upgradeButton.interactable = false;
        }
    }
}