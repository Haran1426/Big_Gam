using UnityEngine;

public class TimeMachine : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite completeSprite;

    public float detectRange = 4f;
    public GameObject hintText; 

    bool isActivated = false;

    void Start()
    {
        hintText.SetActive(false);
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float dist = Vector2.Distance(transform.position, player.transform.position);
        bool isNear = dist <= detectRange;

        if (!isActivated)
            spriteRenderer.sprite = normalSprite;
        else
            spriteRenderer.sprite = completeSprite;

        if (PartManager.Instance.partsCount == 3)
        {
            hintText.SetActive(isNear);

            if (isNear && Input.GetKeyDown(KeyCode.F))
            {
                isActivated = true;
                hintText.SetActive(false);
            }
        }
        else
        {
            hintText.SetActive(false);
        }
    }
}
