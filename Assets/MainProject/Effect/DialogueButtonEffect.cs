using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween 필요

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Outline))]
public class DialogueButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    private RectTransform rectTransform;
    private Image buttonImage;
    private Outline outline;
    private Button button;

    [Header("Settings")]
    public Color hoverOutlineColor = Color.red;
    public Color normalOutlineColor = new Color(0, 0, 0, 0.5f);
    public float hoverMoveDistance = 4f;

    private Vector2 originalPos;
    private Material instanceMaterial; // 쉐이더 제어용

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();
        outline = GetComponent<Outline>();
        button = GetComponent<Button>();

        // Outline 초기화
        outline.effectColor = normalOutlineColor;
        outline.effectDistance = new Vector2(1, -1);
        outline.enabled = false;

        // 쉐이더 제어를 위해 머티리얼 인스턴스 생성
        if (buttonImage.material != null)
        {
            instanceMaterial = new Material(buttonImage.material);
            buttonImage.material = instanceMaterial;
        }
    }

    // 1. 등장 연출: 납작한 선에서 늘어나기
    public void PlayOpenAnimation(float delay)
    {
        gameObject.SetActive(true);
        button.interactable = true;

        // 초기 상태: Y축 스케일 0 (납작함), Dissolve 0
        rectTransform.localScale = new Vector3(1, 0, 1);
        if (instanceMaterial != null) instanceMaterial.SetFloat("_DissolveAmount", 0);

        // 연출: 세로로 늘어남 (Elastic 느낌 살짝 추가)
        rectTransform.DOScaleY(1f, 0.5f)
            .SetEase(Ease.OutBack)
            .SetDelay(delay);

        // 위치 초기화 저장
        originalPos = rectTransform.anchoredPosition;
    }

    // 2. 호버 연출: 테두리 강조 + 위로 떠오름
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        outline.enabled = true;
        outline.DOColor(hoverOutlineColor, 0.2f);
        outline.DOFade(0.5f, 0.2f); // 두께감 대신 알파나 거리로 조절 가능

        // 위로 살짝 올라감
        rectTransform.DOAnchorPosY(originalPos.y + hoverMoveDistance, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        outline.DOColor(normalOutlineColor, 0.2f);
        outline.enabled = false;

        // 원래 위치로 복귀
        rectTransform.DOAnchorPosY(originalPos.y, 0.2f);
    }

    // 3. 클릭 연출: 화면 진동 + 푹 꺼짐
    public void PlayClickAnimation()
    {
        // 버튼 푹 꺼짐 (원래 위치보다 아래로)
        rectTransform.DOAnchorPosY(originalPos.y - 2f, 0.1f)
            .SetLoops(2, LoopType.Yoyo) // 2번 반복(왕복), 방식은 Yoyo
            .SetEase(Ease.InOutQuad);

        // 화면 진동 (카메라) - MainCamera에 부착된 DOTween 사용
        if (Camera.main != null)
        {
            Camera.main.transform.DOShakePosition(0.3f, 0.5f, 20, 90, false, true);
        }
    }

    // 4. 선택된 버튼 사라짐: 커졌다가 가로로 찌그러짐 (TV Off)
    public void PlaySelectedCloseAnimation()
    {
        button.interactable = false;

        Sequence seq = DOTween.Sequence();

        // 1. 살짝 커짐
        seq.Append(rectTransform.DOScale(1.1f, 0.1f));
        // 2. 가로로 납작해지며 사라짐 (TV 꺼지듯)
        seq.Append(rectTransform.DOScale(new Vector3(1.5f, 0.01f, 1f), 0.2f).SetEase(Ease.InBack));
        seq.Join(buttonImage.DOFade(0, 0.1f).SetDelay(0.15f));

        seq.OnComplete(() => gameObject.SetActive(false));
    }

    // 5. 선택 안 된 버튼 사라짐: 모래처럼 부서짐
    public void PlayDiscardCloseAnimation()
    {
        button.interactable = false;

        if (instanceMaterial != null)
        {
            // 쉐이더의 _DissolveAmount 값을 0 -> 1로 변경
            DOVirtual.Float(0, 1, 0.6f, (value) =>
            {
                instanceMaterial.SetFloat("_DissolveAmount", value);
            }).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            // 쉐이더 없으면 그냥 Fade Out
            buttonImage.DOFade(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}