using UnityEngine;
using System.Collections;

public class FloatingPairObject : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float fadeDuration = 1.2f;

    private SpriteRenderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void StartFloating()
    {
        StartCoroutine(FloatingRoutine());
    }

    private IEnumerator FloatingRoutine()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            transform.position = startPos + Vector3.up * (timer * moveSpeed);

            float alpha = 1f - (timer / fadeDuration);
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = alpha;
                r.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
