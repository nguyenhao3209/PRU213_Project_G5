using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerFinal : MonoBehaviour
{
    public AlucardController player; // Kéo Alucard vào đây
    public GalamothAI boss;          // Kéo Galamoth vào đây
    public Transform respawnPoint;   // Kéo RespawnPoint vào đây

    public float respawnDelay = 3f; // Thời gian chờ cho animation chết

    // Hàm này sẽ được Alucard gọi khi chết
    public void StartRespawn()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        // 1. Chờ cho animation chết của Alucard chạy xong
        yield return new WaitForSeconds(respawnDelay);

        // 2. Yêu cầu Boss tự reset
        if (boss != null)
        {
            boss.ResetBoss();
        }

        // 3. Yêu cầu Player tự reset và di chuyển đến điểm hồi sinh
        if (player != null)
        {
            player.ResetPlayer(respawnPoint.position);
        }
    }
}
