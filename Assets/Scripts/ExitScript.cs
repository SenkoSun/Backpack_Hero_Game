using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Остановка в редакторе
        #else
        Application.Quit(); // Закрытие билда игры
        #endif
    }
}