using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public GameObject levelCompleteUI;
    public string nextLevelName;
    public string menuSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("PLAYER REACHED GOAL");
            Time.timeScale = 0f;
            levelCompleteUI.SetActive(true);
        }
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }
}
