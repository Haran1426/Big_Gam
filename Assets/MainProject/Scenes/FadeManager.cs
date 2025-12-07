using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public TMP_FontAsset FontAsset;

    [Header("UI Reference")]
    public CanvasGroup fadeCanvasGroup;
    public Image fadeImage;

    public int DayIndex;

    [Header("Settings")]
    public float fadeDuration = 1.0f; // 페이드 걸리는 시간

    public TextMeshProUGUI text;

    public GameObject canvasObj;
    public GameObject imageObj;
    public GameObject textObj;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음

            // UI가 연결되지 않았다면 자동으로 생성 (편의성)
            if (fadeCanvasGroup == null) SetupFadeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 자동으로 페이드용 캔버스/이미지 생성
    private void SetupFadeUI()
    {
        // 1. 캔버스 생성
        canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // 모든 UI보다 위에 보이도록 설정

        // 2. CanvasGroup 추가 (투명도 조절용)
        fadeCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f; // 처음엔 투명하게
        fadeCanvasGroup.blocksRaycasts = false; // 평소엔 클릭 통과

        // 3. 검은 이미지 생성
        imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        imageObj.transform.localPosition = Vector3.zero;

        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = Color.black;

        if (DayIndex != 4)
        {
            textObj = new GameObject("DayText");
            textObj.transform.SetParent(canvasObj.transform);
            textObj.transform.localPosition = Vector3.zero;

            text = textObj.AddComponent<TextMeshProUGUI>();
            text.font = FontAsset;
            text.fontSize *= 2;
            text.rectTransform.sizeDelta *= 2;
            text.text = $"Day {DayIndex}";
        }
        else if (DayIndex == 4)
        {
            DayIndex = 0;
        }

        // 화면 꽉 채우기
        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// 외부에서 호출하는 씬 이동 함수. (페이드 아웃 -> 씬 로드 -> 페이드 인)
    /// </summary>
    public void LoadSceneWithFade(string sceneName)
    {
        if (fadeCanvasGroup == null) SetupFadeUI();
        StartCoroutine(Co_SceneTransition(sceneName));
    }

    private IEnumerator Co_SceneTransition(string sceneName)
    {
        // 1. 페이드 아웃 (화면이 검어짐)
        yield return StartCoroutine(Fade(1f));

        // 2. 씬 로드
        SceneManager.LoadScene(sceneName);

        // 씬 로딩 완료를 위해 1프레임 대기 (안정성)
        yield return null;

        // 3. 페이드 인 (화면이 다시 밝아짐)
        yield return StartCoroutine(Fade(0f));
        Destroy(textObj);
        Destroy(imageObj);
        Destroy(canvasObj);
    }

    // 페이드 동작 (targetAlpha: 1이면 검어짐, 0이면 투명해짐)
    private IEnumerator Fade(float targetAlpha)
    {
        // 클릭 방지 설정: 화면이 보일 때(0)는 클릭 가능, 검어질 때(1)는 클릭 차단
        fadeCanvasGroup.blocksRaycasts = (targetAlpha > 0.1f);

        // DOTween으로 알파값 조절
        Tween fadeTween = fadeCanvasGroup.DOFade(targetAlpha, fadeDuration);

        // 트윈이 끝날 때까지 대기
        yield return fadeTween.WaitForCompletion();
    }
}