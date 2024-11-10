using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Personal Info")]
    [SerializeField] private string myName;
    [SerializeField] private TMP_Text myNameText;
    [SerializeField] private string myPronoun;

    [Header("Level")]
    public int currentLevel;
    [SerializeField] private TMP_Text levelText;

    [Header("Chakra")]
    public float currentChakra;
    public float maxChakra;
    public Slider chakraBar;

    private float damageBoost = 1.0f;

    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    public Slider healthBar;

    [Header("Defence")]
    public float defence;

    [Header("Is Stunned, Asleep, Confused, Ect")]
    public int stunRoundsRemaining = 0;
    public int confusionRoundsRemaining = 0;

    [Header("Resistance")]
    [Range(0, 100)]
    public int stunResistance;
    [Range(0, 100)]
    public int confusionResistance;

    [Header("Targeted Opponent")]
    [SerializeField] private PlayerController targetPlayer;

    [Header("Typewriter")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private float typingSpeed = 0.01f;

    [Header("Jutus")]
    [SerializeField] public List<JutsuHandler> jutsuList;

    // Start is called before the first frame update
    void Start()
    {
        if (jutsuList == null || jutsuList.Count == 0)
        {
            Debug.LogWarning("No jutsus assigned to this enemy in the editor.");
        }

        myNameText.text = myName;

        currentChakra = maxChakra;
        chakraBar.maxValue = maxChakra;
        chakraBar.value = maxChakra;

        maxHealth += (currentLevel * 1.5f);
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;

        levelText.text = "Lvl: " + currentLevel;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = currentHealth;
        chakraBar.value = currentChakra;
    }

    public IEnumerator TakeTurn()
    {
        if (stunRoundsRemaining > 0)
        {
            if (stunRoundsRemaining >= 2)
            {
                yield return StartCoroutine(DisplayMessage($"{myName} is stunned for {stunRoundsRemaining} turns and skips this turn."));
                stunRoundsRemaining--;
            }
            else if (stunRoundsRemaining == 1)
            {
                yield return StartCoroutine(DisplayMessage($"{myName} is stunned for {stunRoundsRemaining} turn and skips this turn."));
                stunRoundsRemaining--;
            }
            yield return new WaitForSeconds(2.5f);
            yield break;
        }
        if (confusionRoundsRemaining > 0)
        {
            if (confusionRoundsRemaining >= 2)
            {
                yield return StartCoroutine(DisplayMessage($"{myName} is confused for {confusionRoundsRemaining} turns and skips this turn."));
                confusionRoundsRemaining--;
            }
            else if (confusionRoundsRemaining == 1)
            {
                yield return StartCoroutine(DisplayMessage($"{myName} is confused for {confusionRoundsRemaining} turn and skips this turn."));
                confusionRoundsRemaining--;
            }
            yield return new WaitForSeconds(2.5f);
            yield break;
        }

        List<int> usableJutsuIndices = new List<int>();

        for (int i = 0; i < jutsuList.Count; i++)
        {
            if (currentChakra >= jutsuList[i].ChakraCost && !jutsuList[i].IsChakraRecharge)
            {
                usableJutsuIndices.Add(i);
            }
        }

        if (usableJutsuIndices.Count > 0)
        {
            int jutsuIndex = usableJutsuIndices[Random.Range(0, usableJutsuIndices.Count)];
            UseJutsu(jutsuIndex);
            yield return StartCoroutine(DisplayMessage($"{myName} uses {jutsuList[jutsuIndex].Name}! {myPronoun} {jutsuList[jutsuIndex].Effect}!"));
        }
        else
        {
            List<int> rechargeJutsuIndices = new List<int>();
            for (int i = 0; i < jutsuList.Count; i++)
            {
                if (jutsuList[i].IsChakraRecharge)
                {
                    rechargeJutsuIndices.Add(i);
                }
            }

            if (rechargeJutsuIndices.Count > 0)
            {
                int rechargeJutsuIndex = rechargeJutsuIndices[Random.Range(0, rechargeJutsuIndices.Count)];
                UseJutsu(rechargeJutsuIndex);
                yield return StartCoroutine(DisplayMessage($"{myName} recharges chakra to max!"));
            }
        }

        if (currentHealth <= 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(2.5f);

    }

    public void UseJutsu(int index)
    {
        if (index < 0 || index >= jutsuList.Count)
        {
            Debug.LogError("Invalid jutsu index.");
            return;
        }

        JutsuHandler selectedJutsu = jutsuList[index];

        if (currentChakra >= selectedJutsu.ChakraCost)
        {
            currentChakra -= selectedJutsu.ChakraCost;

            // Check if this jutsu is a power-boosting jutsu
            if (selectedJutsu.IsBooster)
            {
                foreach (var jutsu in jutsuList)
                {
                    if (jutsu != selectedJutsu) // Exclude the boosting jutsu itself
                    {
                        jutsu.Power = Mathf.RoundToInt(jutsu.Power * selectedJutsu.BoostAmount);
                    }
                }
                Debug.Log($"{selectedJutsu.Name} used! All other jutsus boosted by {selectedJutsu.BoostAmount}.");
            }
            else if (targetPlayer != null) // If not a boost jutsu, treat as damage jutsu
            {
                int boostedPower = Mathf.RoundToInt(selectedJutsu.Power * damageBoost);
                targetPlayer.TakeDamagePlayer(boostedPower);
            }

            // All Debuffs to Player
            if (selectedJutsu.IsStunner && targetPlayer != null)
            {
                targetPlayer.ApplyStun(selectedJutsu.StunRounds);
            }
            if (selectedJutsu.IsConfusion && targetPlayer != null)
            {
                targetPlayer.ApplyConfusion(selectedJutsu.ConfusionRounds);
            }

            // Execute the jutsu with the current damage boost
            selectedJutsu.Execute(damageBoost);

            defence += selectedJutsu.DefenceBoost;
        }
    }

    public void TakeDamageEnemy(int damage)
    {
        if (currentHealth > 0 && defence > 0)
        {
            currentHealth -= damage / defence;
        }
        else if (currentHealth > 0 && defence <= 0)
        {
            currentHealth -= damage;
        }
    }

    public void HealDamageEnemy(int heal)
    {
        if ((currentHealth > 0) && (currentHealth < maxHealth))
        {
            currentHealth += heal;
        }
    }


    // Everything that has to do with UI
    private IEnumerator TypeText(TMP_Text text, string effect)
    {
        text.text = "";

        foreach (char letter in effect)
        {
            text.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator DisplayMessage(string message)
    {
        MessageManager.Instance.EnqueueRegularMessage(message); // Send to MessageManager
        yield return null; // Ensure coroutine exits after enqueueing message
    }

    private IEnumerator DisplayResistMessage(string message)
    {
        MessageManager.Instance.EnqueueRegularMessage(message); // Send to MessageManager
        yield return null;
    }


    // Debuffs To Self
    public void ApplyStun(int rounds)
    {
        bool isStunned = Random.Range(0, 100) >= stunResistance;

        if (isStunned)
        {
            stunRoundsRemaining = rounds;
        }
        else
        {
            MessageManager.Instance.EnqueueHighPriorityMessage($"{myName} resisted the stun!");
        }
    }

    public void ApplyConfusion(int rounds)
    {
        bool isConfused = Random.Range(0, 100) >= confusionResistance;

        if (isConfused)
        {
            confusionRoundsRemaining = rounds;
        }
        else
        {
            MessageManager.Instance.EnqueueHighPriorityMessage($"{myName} resisted the confusion!");
        }
    }
}
