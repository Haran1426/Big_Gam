using UnityEngine;

public class PartBase : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite highlightSprite;

    public float detectRange = 1f;

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float dist = Vector2.Distance(transform.position, player.transform.position);

        bool isNear = dist <= detectRange;

        spriteRenderer.sprite = isNear ? highlightSprite : normalSprite;

        if (isNear && Input.GetKeyDown(KeyCode.F))
        {
            PartManager.Instance.AddPart();
            gameObject.SetActive(false);
        }
    }
}