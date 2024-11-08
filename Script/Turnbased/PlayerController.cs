using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Personal Info")]
    [SerializeField] private string myName;
    [SerializeField] private TMP_Text myNameText;
    [SerializeField] private string myPronoun;

    [Header("Level")]
    public int currentLevel;
    private int maxLevel = 50;
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
    [SerializeField] private EnemyController targetEnemy;

    [Header("Buttons")]
    [SerializeField] private Button jutsu1Button;
    [SerializeField] private Button jutsu2Button;
    [SerializeField] private Button jutsu3Button;
    [SerializeField] private Button jutsu4Button;
    [SerializeField] private Button jutsu5Button;
    [SerializeField] private Button jutsu6Button;

    [Header("Typewriter")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private float typingSpeed = 0.01f;

    [Header("Jutus")]
    [SerializeField] public List<JutsuHandler> jutsuList;
    public bool usedJutsu;

    // Start is called before the first frame update
    void Start()
    {
        if (jutsuList == null || jutsuList.Count == 0)
        {
            Debug.LogWarning("No jutsus assigned to this player in the editor.");
        }

        myNameText.text = myName;

        currentChakra = maxChakra;
        chakraBar.maxValue = maxChakra;
        chakraBar.value = maxChakra;

        maxHealth += (currentLevel * 1.5f);
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;

        SetButton(jutsu1Button, 0);
        SetButton(jutsu2Button, 1);
        SetButton(jutsu3Button, 2);
        SetButton(jutsu4Button, 3);
        SetButton(jutsu5Button, 4);
        SetButton(jutsu6Button, 5);

        usedJutsu = false;

        levelText.text = "Lvl: " + currentLevel;

    }

    // Update is called once per frame
    void Update()
    {
        chakraBar.value = currentChakra;
        healthBar.value = currentHealth;

        if (currentLevel < 5)
        {
            jutsu4Button.image.color = Color.gray;
        }
        else
        {
            jutsu4Button.image.color = Color.white;
        }

        if (currentLevel < 25)
        {
            jutsu5Button.image.color = Color.gray;
        }
        else
        {
            jutsu5Button.image.color = Color.white;
        }

        if (currentLevel < 50)
        {
            jutsu6Button.image.color = Color.gray;
        }
        else
        {
            jutsu6Button.image.color = Color.white;
        }

        // Test level
        if (Input.GetKeyDown(KeyCode.E) && currentLevel < maxLevel)
        {
            currentLevel += 5;
            levelText.text = "Lvl: " + currentLevel;
        }
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
            yield break;
        }
        usedJutsu = false;

        yield return new WaitUntil(() => usedJutsu);
    }

    public void UseJutsu(int index)
    {
        if (usedJutsu)
        {
            return;
        }

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
            else if (targetEnemy != null) // If not a boost jutsu, treat as damage jutsu
            {
                int boostedPower = Mathf.RoundToInt(selectedJutsu.Power * damageBoost);
                targetEnemy.TakeDamageEnemy(boostedPower);
            }

            StartCoroutine(DisplayMessage($"{myName} uses {selectedJutsu.Name}! {myPronoun} {selectedJutsu.Effect}!"));

            // Checks if you is recharging chakra
            if (selectedJutsu.IsChakraRecharge)
            {
                currentChakra = maxChakra;
            }

            selectedJutsu.Execute(damageBoost);

            defence += selectedJutsu.DefenceBoost;

            usedJutsu = true;


            // All Debuffs To Enemy
            if (selectedJutsu.IsStunner && targetEnemy != null)
            {
                targetEnemy.ApplyStun(selectedJutsu.StunRounds);
            }
            if (selectedJutsu.IsConfusion && targetEnemy != null)
            {
                targetEnemy.ApplyConfusion(selectedJutsu.ConfusionRounds);
            }
        }
    }

    public void TakeDamagePlayer(int damage)
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

    public void HealDamagePlayer(int heal)
    {
        if ((currentHealth > 0) && (currentHealth < maxHealth))
        {
            currentHealth += heal;
        }
    }


    // Everything that has to do with UI
    private void SetButton(Button button, int jutsuIndex)
    {
        if (button != null && jutsuIndex < jutsuList.Count)
        {
            // Set button text to match jutsu name
            var buttonText = button.GetComponentInChildren<Text>();
            var buttonTMPText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = jutsuList[jutsuIndex].Name;
            }
            else if (buttonTMPText != null)
            {
                buttonTMPText.text = jutsuList[jutsuIndex].Name;
            }
        }
    }

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
        yield return null;
    }

    private IEnumerator DisplayResistMessage(string message)
    {
        MessageManager.Instance.EnqueueRegularMessage(message); // Send to MessageManager
        yield return null;
    }


    // All Debuffs To Self
    public void ApplyStun(int rounds)
    {
        bool isStunned = Random.Range(0, 100) >= stunResistance;

        if (isStunned)
        {
            stunRoundsRemaining = rounds;
        }
        else
        {
            StartCoroutine(DisplayMessage($"{myName} resisted the stun!"));
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
            StartCoroutine(DisplayMessage($"{myName} resisted the confusion!"));
        }
    }
}