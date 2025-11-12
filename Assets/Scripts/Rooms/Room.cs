using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    private Vector3[] initialPosition;

    private void Awake()
    {
        // Lưu vị trí ban đầu của kẻ địch
        initialPosition = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                initialPosition[i] = enemies[i].transform.position;
        }

        // Deactivate các room khác trừ room đầu
        if (transform.GetSiblingIndex() != 0)
            ActivateRoom(false);
    }

    public void ActivateRoom(bool _status)
{
    //Activate/deactivate enemies
    for (int i = 0; i < enemies.Length; i++)
    {
        if (enemies[i] != null)
        {
            enemies[i].SetActive(_status);
            enemies[i].transform.position = initialPosition[i];
        }
    }
}

}
