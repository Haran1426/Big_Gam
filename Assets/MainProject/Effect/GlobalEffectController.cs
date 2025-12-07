using UnityEngine;

public class GlobalEffectController : MonoBehaviour
{
    [Header("Settings")]
    public Material screenEffectMaterial;

    // 게임 도중 이 변수를 조절하면 화면이 바뀝니다.
    [Range(-100f, 100f)]
    public float currentTwist = 0f;

    [Range(0, 1)]
    public int currentActive = 0;

    private int twistPropID;
    private int ActivePropID;

    void Start()
    {
        twistPropID = Shader.PropertyToID("_TwistStrength");
        ActivePropID = Shader.PropertyToID("_IsActive");
    }

    void Update()
    {
        if (screenEffectMaterial != null)
        {
            currentTwist = Mathf.Sin(Time.time * 2.0f) * 3.0f;


            // 변경된 값을 실제 마테리얼에 적용
            screenEffectMaterial.SetFloat(twistPropID, currentTwist);
            screenEffectMaterial.SetFloat(ActivePropID, currentActive);
        }
    }

    void OnDisable()
    {
        if (screenEffectMaterial != null)
        {
            screenEffectMaterial.SetFloat(twistPropID, 0f);
            screenEffectMaterial.SetFloat(ActivePropID, 0f);
        }
    }
}