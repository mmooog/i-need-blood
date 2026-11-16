using UnityEngine;
using Fungus;

public class NPCInteract : MonoBehaviour
{
    public GameObject promptBubble;      // Drag your animated prompt child object here
    public Flowchart targetFlowchart;    // Drag your Flowchart here
    public string blockName = "TalkToNPC"; // Must match your Block's exact name
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        if (promptBubble != null)
            promptBubble.SetActive(false);
    }

    void Update()
    {
        // Only allow starting dialogue if player is nearby, presses E, and no dialogue is currently open
        bool isTalking = SayDialog.ActiveSayDialog != null && SayDialog.ActiveSayDialog.isActiveAndEnabled;

        if (playerInRange && !isTalking && Input.GetKeyDown(interactKey))
        {
            if (promptBubble != null)
                promptBubble.SetActive(false);

            targetFlowchart.ExecuteBlock(blockName);
        }
        else if (playerInRange && !isTalking && promptBubble != null && !promptBubble.activeSelf)
        {
            // Bring the prompt back once the conversation ends if the player is still standing there
            promptBubble.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptBubble != null)
                promptBubble.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptBubble != null)
                promptBubble.SetActive(false);
        }
    }
}
