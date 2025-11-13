using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    // Nếu bạn muốn quay về menu chính thay vì thoát game
    [SerializeField] private string mainMenuSceneName = "_Menu";

    // Nút "Thoát" – dùng trong build thật
    public void QuitGame()
    {
        Debug.Log("Quit Game pressed!");

#if UNITY_EDITOR
        // Dừng play mode trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Thoát game khi build
        Application.Quit();
#endif
    }

    // Nút "Về Menu" – nếu bạn có scene menu
    public void BackToMenu()
    {
        Debug.Log("Return to main menu");
        Time.timeScale = 1f; // resume game time
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
