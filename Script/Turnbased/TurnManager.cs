using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController player;          // Reference to the player
    public EnemyController enemy;            // Reference to the enemy
    public bool isPlayerTurn = true;        // To track whose turn it is

    private void Start()
    {
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (true) // Infinite loop for turn-based battle
        {
            if (isPlayerTurn)
            {
                // Player's turn
                yield return StartCoroutine(player.TakeTurn());
            }
            else
            {
                // Enemy's turn
                yield return StartCoroutine(enemy.TakeTurn());
            }

            // Optionally wait between turns
            yield return new WaitForSeconds(5f);

            // Switch turn
            isPlayerTurn = !isPlayerTurn;
        }
    }
}
