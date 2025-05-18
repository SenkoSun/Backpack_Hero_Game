using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    
    [Header("Основное меню")]
    public GameObject Background;
    public GameObject PlayButton;
    public GameObject UpgradeButton;
    public GameObject ExitButton;

    [Header("Меню улучшений")]
    public GameObject UpgradesBackground;
    public GameObject UpgradesPanel;
    public GameObject BackButton;

    [Header("Сброс прогресса")]
    public bool resetOnStart = true; // Включить/выключить автосброс

    void Start()
    {
        if (resetOnStart)
        {
            // 1. Полная очистка
            PlayerPrefs.DeleteAll();
            
            // 2. Принудительная инициализация
            PlayerPrefs.SetInt("ForceResetDone", 1); // Маркер сброса
            
            // 3. Немедленное сохранение
            PlayerPrefs.Save();
            
            // 4. Очистка кэша Unity
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
            #endif
            
            Debug.Log("Прогресс гарантированно сброшен!");
        }
    }

    // Универсальный сброс всего прогресса
    public void ResetAllProgress()
    {
        // 1. Полностью очищаем все сохранения
        PlayerPrefs.DeleteAll();
        
        // 2. Специально сбрасываем ключ золота (если используете GoldManager)
        PlayerPrefs.SetInt("PlayerGold", 0);
        
        // 3. Принудительно сохраняем
        PlayerPrefs.Save();
        
        Debug.Log("ВСЕ сохранения сброшены!");
    }
    public void OpenUpgrades()
    {
        Background.SetActive(false);
        PlayButton.SetActive(false);
        UpgradeButton.SetActive(false);
        ExitButton.SetActive(false);

        UpgradesBackground.SetActive(true);
        UpgradesPanel.SetActive(true);
        BackButton.SetActive(true);
    }

    public void CloseUpgrades()
    {
        Background.SetActive(true);
        PlayButton.SetActive(true);
        UpgradeButton.SetActive(true);
        ExitButton.SetActive(true);

        UpgradesBackground.SetActive(false);
        UpgradesPanel.SetActive(false);
        BackButton.SetActive(false);
    }
}