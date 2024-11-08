using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    [SerializeField] private TMP_Text displayText;
    [SerializeField] private float typingSpeed = 0.01f;

    private Queue<string> highPriorityQueue = new Queue<string>(); // For confusion/stun messages
    private Queue<string> regularQueue = new Queue<string>(); // For normal jutsu messages
    private bool isDisplayingMessage = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Enqueue high-priority message
    public void EnqueueHighPriorityMessage(string message)
    {
        highPriorityQueue.Enqueue(message);

        if (!isDisplayingMessage)
        {
            StartCoroutine(ProcessMessageQueue());
        }
    }

    // Enqueue regular message
    public void EnqueueRegularMessage(string message)
    {
        regularQueue.Enqueue(message);

        if (!isDisplayingMessage)
        {
            StartCoroutine(ProcessMessageQueue());
        }
    }

    private IEnumerator ProcessMessageQueue()
    {
        isDisplayingMessage = true;

        while (highPriorityQueue.Count > 0 || regularQueue.Count > 0)
        {
            string currentMessage = highPriorityQueue.Count > 0 ? highPriorityQueue.Dequeue() : regularQueue.Dequeue();
            displayText.text = "";

            foreach (char letter in currentMessage)
            {
                displayText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            yield return new WaitForSeconds(2.5f); // Delay between messages
        }

        isDisplayingMessage = false;
    }
}