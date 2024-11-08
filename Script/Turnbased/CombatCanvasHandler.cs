using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatCanvasHandler : MonoBehaviour
{
    [SerializeField] GameObject startFightButtons;
    [SerializeField] GameObject jutsusButtons;

    [SerializeField] private TMP_Text effectText;

    [SerializeField] PlayerController PC;
    [SerializeField] TurnManager turnManager;

    // Start is called before the first frame update
    void Start()
    {
        startFightButtons.SetActive(true);
        jutsusButtons.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void fight()
    {
        startFightButtons.SetActive(false);
        jutsusButtons.SetActive(true);
    }

    public void Back()
    {
        startFightButtons.SetActive(true);
        jutsusButtons.SetActive(false);
    }

    // Use Jutsu
    public void Jutsu1()
    {
        if (turnManager.isPlayerTurn == true)
        {
            PC.UseJutsu(0);
        }
    }
    public void Jutsu2()
    {
        if (turnManager.isPlayerTurn == true)
        {
            PC.UseJutsu(1);
        }
    }
    public void Jutsu3()
    {
        if (turnManager.isPlayerTurn == true)
        {
            PC.UseJutsu(2);
        }
    }

    // Must be level 5+
    public void Jutsu4()
    {
        if (turnManager.isPlayerTurn == true)
        {
            if (PC.currentLevel >= 5)
            {
                PC.UseJutsu(3);
            }
        }
    }

    // Must be level 25+
    public void Jutsu5()
    {
        if (turnManager.isPlayerTurn == true)
        {
            if (PC.currentLevel >= 25)
            {
                PC.UseJutsu(4);
            }
        }
    }

    // Must be level 50+
    public void Jutsu6()
    {
        if (turnManager.isPlayerTurn == true)
        {
            if (PC.currentLevel >= 50)
            {
                PC.UseJutsu(5);
            }
        }
    }
}
