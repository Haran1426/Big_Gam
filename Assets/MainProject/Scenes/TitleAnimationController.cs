using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트 제어용
using DG.Tweening;    // DOTween 네임스페이스 필수

public class TitleAnimationController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image lightImage;          // 빛(십자가) 이미지 (Image 컴포넌트 필요)


    void Awake()
    {
        Color c = lightImage.color;
        c.a = 0f;
        lightImage.color = c;
    }

    void Start()
    {
        StartLightLoop();
    }

    void StartLightLoop()
    {
        lightImage.DOFade(1.0f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}