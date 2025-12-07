using UnityEngine;

public class PropBase : MonoBehaviour
{
    public PropData propData;
    
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

        foreach (var collider in colliders) {
            if (collider.CompareTag("Player"))
            {
                if(Input.GetKeyDown(KeyCode.F) && !DialogueSystem.Instance.isDialogueActive) DialogueSystem.Instance.StartDialogue(propData);
                Debug.Log($"Prop is Colliding with Player: {propData.propID}"); 
            }
        }
    }
}
