using TMPro;
using UnityEngine;

public class GoldManager : MonoBehaviour {
    public static GoldManager Instance;
    public int Gold;
    public TMP_Text GoldText;

    void Awake() {
        Instance = this;
    }

    void Update() {
        GoldText.text = Gold.ToString();
    }

    public void AddGold(int amount) {
        Gold += amount;
    }
    void Start() {
    AddGold(100000); // Тест: +100 золота
}
    
}
