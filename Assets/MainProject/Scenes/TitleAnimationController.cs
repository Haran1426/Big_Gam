using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트 제어용
using DG.Tweening;    // DOTween 네임스페이스 필수

public class TitleAnimationController : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform titleImage1; // 'EFFECT' 이미지
    public RectTransform titleImage2; // 'LOOP' 이미지

    [Header("Animation Settings")]
    public float entranceDuration = 1.5f; // 등장 시간
    public float startXOffset = 500f;     // 오른쪽에서 시작할 거리

    private Vector2 title1OriginalPos;
    private Vector2 title2OriginalPos;

    void Awake()
    {
        // 1. 시작 전 원래 위치(도착점) 저장
        title1OriginalPos = titleImage1.anchoredPosition;
        title2OriginalPos = titleImage2.anchoredPosition;

        // 2. 초기화: 타이틀을 오른쪽으로 이동시켜두고, 빛은 투명하게 설정
        titleImage1.anchoredPosition += new Vector2(startXOffset, 0);
        titleImage2.anchoredPosition += new Vector2(startXOffset, 0);

    }

    void Start()
    {
        PlayTitleAnimation();
    }

    void PlayTitleAnimation()
    {
        // DOTween 시퀀스 생성 (순차적 실행을 위해)
        Sequence seq = DOTween.Sequence();

        // === 1. 타이틀 등장 애니메이션 ===
        // Ease.OutBack: 목표 지점을 약간 지나쳤다가 다시 돌아오는 효과 (고무줄 느낌)

        // 두 이미지가 동시에 혹은 약간의 시차를 두고 등장
        seq.Append(titleImage1.DOAnchorPos(title1OriginalPos, entranceDuration).SetEase(Ease.OutBack));
        seq.Join(titleImage2.DOAnchorPos(title2OriginalPos, entranceDuration).SetEase(Ease.OutBack).SetDelay(0.1f));
    }
}