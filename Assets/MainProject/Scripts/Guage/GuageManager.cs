using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum GuageType
{
    Evironment,
    Economy,
    Happiness,
    Security
}

public class GuageManager : MonoBehaviour
{
    public static GuageManager Instance;

    public float evironmentValue;
    public float economyValue;
    public float happinessValue;
    public float securityValue;

    public Slider evironmentSlider;
    public Slider economySlider;
    public Slider happinessSlider;
    public Slider securitySlider;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateGuageUI();
    }

    public void UpdateGuageUI()
    {
        DOTween.To(() => evironmentSlider.value, x => evironmentSlider.value = x, evironmentValue / 100f, 0.5f);
        DOTween.To(() => economySlider.value, x => economySlider.value = x, economyValue / 100f, 0.5f);
        DOTween.To(() => happinessSlider.value, x => happinessSlider.value = x, happinessValue / 100f, 0.5f);
        DOTween.To(() => securitySlider.value, x => securitySlider.value = x, securityValue / 100f, 0.5f);

        Debug.Log("Gauge UI Updated");
    }

    public void AddGuageValue(GuageType type, float value)
    {
        switch (type)
        {
            case GuageType.Evironment:
                evironmentValue += value;
                break;
            case GuageType.Economy:
                economyValue += value;
                break;
            case GuageType.Happiness:
                happinessValue += value;
                break;
            case GuageType.Security:
                securityValue += value;
                break;
        }

        Debug.Log("" + type.ToString() + " Gauge Updated: " + value);
    }

    public void SetGuageValue(GuageType type, float value)
    {
        switch (type)
        {
            case GuageType.Evironment:
                evironmentValue = value;
                break;
            case GuageType.Economy:
                economyValue = value;
                break;
            case GuageType.Happiness:
                happinessValue = value;
                break;
            case GuageType.Security:
                securityValue = value;
                break;
        }
        Debug.Log("" + type.ToString() + " Gauge Set: " + value);
    }   
}
