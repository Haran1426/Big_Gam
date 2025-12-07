using UnityEngine;

public class GlobalEffectController : MonoBehaviour
{
    [Header("Settings")]
    public Material screenEffectMaterial;

    [Header("Animation Settings")]
    [Tooltip("활성화되었을 때 도달할 최대 비틀기 강도")]
    public float targetTwist = 100f;
    [Tooltip("변화하는 속도 (높을수록 빠름)")]
    public float transitionSpeed = 5.0f;

    [Range(-100f, 100f)]
    public float currentTwist = 0f;

    [Range(0, 1)]
    public int currentActive = 0;

    private int twistPropID;
    private int ActivePropID;

    public static GlobalEffectController Instance;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        twistPropID = Shader.PropertyToID("_TwistStrength");
        ActivePropID = Shader.PropertyToID("_IsActive");
    }

    void Update()
    {
        if (currentActive == 1)
        {
            //currentTwist = Mathf.Lerp(currentTwist, targetTwist, Time.deltaTime * transitionSpeed);
            currentTwist = currentTwist + Time.deltaTime * transitionSpeed;
        }

        if (screenEffectMaterial != null)
        {
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