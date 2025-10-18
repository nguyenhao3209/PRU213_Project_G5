using UnityEngine;
using UnityEngine.SceneManagement; // để load scene

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        // Load Level1 (hoặc tên scene game của mày)
        SceneManager.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        // Ví dụ: load level đã lưu từ PlayerPrefs
        int level = PlayerPrefs.GetInt("SavedLevel", 1);
        SceneManager.LoadScene("Level" + level);
    }

    public void OpenOptions()
    {
        Debug.Log("Options Menu opened!");
        // Hiện panel options (âm thanh, music) ở đây
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit game!");
    }
}
