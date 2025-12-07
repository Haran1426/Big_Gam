using UnityEngine;

public class PropBase : MonoBehaviour
{
    public PropData propData;
    public PropData subPropData;
    public GameObject outline;

    public bool isAlreadyInteracted = false;
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position + (Vector3)propData.interactiveAreaOffset;
        Vector3 size = new Vector3(propData.interactiveAreaSize.x, propData.interactiveAreaSize.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + propData.interactiveAreaOffset,
            propData.interactiveAreaSize,
            0f
        );

        bool isPlayerNearby = false;

        foreach (var collider in colliders) {
            if (collider.CompareTag("Player"))
            {
                if(Input.GetKeyDown(KeyCode.F) && !DialogueSystem.Instance.isDialogueActive && !isAlreadyInteracted) 
                {
                    DialogueSystem.Instance.StartDialogue(propData);
                    isAlreadyInteracted = true;
                }
                else if(Input.GetKeyDown(KeyCode.F) && !DialogueSystem.Instance.isDialogueActive && isAlreadyInteracted && subPropData != null) 
                {
                    DialogueSystem.Instance.StartDialogue(subPropData);
                }
                isPlayerNearby = true;
            }
        }

        outline.SetActive(isPlayerNearby);
    }
}
