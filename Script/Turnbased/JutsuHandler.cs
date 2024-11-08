using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class JutsuHandler
{
    [SerializeField] private string name;         // Name of the jutsu.
    [SerializeField] private string type;         // Ninjutsu, Genjutsu, Taijutsu.
    [SerializeField] private int power;           // How much damage it deals.
    [SerializeField] private int chakraCost;      // Chakra cost.
    [SerializeField] private float defenceBoost;    // Increase defence.
    [SerializeField] private string effect;       // Description of the effect (burns, stuns, etc.)
    [SerializeField] private bool isChakraRecharge; // Checks if the ability can recharge chakra.

    [Header("Power Booster")]
    [SerializeField] private bool isBooster;     // Shows if a jutsu has been applied.
    [SerializeField] private float boostAmount;   // How much power is being applied after boost.

    [Header("Debuffs")]
    [SerializeField] private bool isStunner;        // Does the ability cause stuns.
    [SerializeField] private int stunRounds;     // How many rounds the opponent skip.
    [SerializeField] private bool isConfusion;        // Does the ability cause confusion.
    [SerializeField] private int confusionRounds;     // How many rounds the opponent skip.

    public string Name { get => name; set => name = value; }
    public string Type { get => type; set => type = value; }
    public int Power { get => power; set => power = value; }
    public int ChakraCost { get => chakraCost; set => chakraCost = value; }
    public float DefenceBoost { get => defenceBoost; set => defenceBoost = value; }
    public string Effect { get => effect; set => effect = value; }
    public bool IsChakraRecharge { get => isChakraRecharge; set => isChakraRecharge = value; }

    // Power Booster
    public bool IsBooster { get => isBooster; set => isBooster = value; }
    public float BoostAmount { get => boostAmount; set => boostAmount = value; }

    // Debuffs
    public bool IsStunner { get => isStunner; set => isStunner = value; }
    public int StunRounds { get => stunRounds; set => stunRounds = value; }
    public bool IsConfusion { get => isConfusion; set => isConfusion = value; }
    public int ConfusionRounds { get => confusionRounds; set => confusionRounds = value; }

    public JutsuHandler(string name, string type, int power, int chakraCost, float defenceBoost, string effect, bool isChakraRecharge, bool isBoosted, float boostAmount, bool isStunner, int stunRounds, bool isConfusion, int confusionRounds)
    {
        this.name = name;
        this.type = type;
        this.power = power;
        this.chakraCost = chakraCost;
        this.defenceBoost = defenceBoost;
        this.effect = effect;
        this.isChakraRecharge = isChakraRecharge;

        // Power booster
        this.isBooster = isBoosted;
        this.boostAmount = boostAmount;

        // Debuffs
        this.isStunner = isStunner;
        this.stunRounds = stunRounds;
        this.isConfusion = isConfusion;
        this.confusionRounds = confusionRounds;
    }

    public void Execute(float damageBoost)
    {
        int boostedPower = Mathf.RoundToInt(Power * damageBoost);

        // Logic for executing the Jutsu, applying damage or effects
        Debug.Log($"{Name} used! Damage: {Power}. Defence Boost: {DefenceBoost}. Damage Boost: {BoostAmount}. Effect: {Effect}.");
    }
}
