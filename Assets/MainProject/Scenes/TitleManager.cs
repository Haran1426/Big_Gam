using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필요
using DG.Tweening; // DOTween 사용

public class TitleManager : MonoBehaviour
{
    [Header("UI Components")]
    public Button startButton;
    public Button exitButton;

    private bool isBusy = false; // 중복 클릭 방지용

    private void Start()
    {
        // 1. 시작 시 버튼 리스너 연결
        SetupButton(startButton, OnClickStart);
        SetupButton(exitButton, OnClickExit);

        PlayOpenAnimations();
    }

    private void SetupButton(Button btn, UnityEngine.Events.UnityAction action)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);

        // DialogueButtonEffect가 없다면 자동으로 추가 (안전장치)
        if (btn.GetComponent<DialogueButtonEffect2>() == null)
        {
            btn.gameObject.AddComponent<DialogueButtonEffect2>();
        }
    }

    private void PlayOpenAnimations()
    {
        // 시작 버튼 등장
        StartCoroutine(Co_PlayOpen(startButton, 0f));
        // 나가기 버튼 등장 (약간의 딜레이로 순차 등장)
        StartCoroutine(Co_PlayOpen(exitButton, 0.1f));
    }

    private IEnumerator Co_PlayOpen(Button btn, float delay)
    {
        btn.gameObject.SetActive(true);
        var effect = btn.GetComponent<DialogueButtonEffect2>();

        // DialogueSystem의 SetupBranchButtons 로직 참조
        if (effect != null)
        {
            effect.PlayOpenAnimation(delay);
        }
        yield break;
    }

    // 시작 버튼 클릭 시 호출
    public void OnClickStart()
    {
        if (isBusy) return;
        StartCoroutine(Co_SelectAction(startButton, exitButton, () =>
        {
            FadeManager.Instance.LoadSceneWithFade("Day0");
        }));
    }

    // 나가기 버튼 클릭 시 호출
    public void OnClickExit()
    {
        if (isBusy) return;
        StartCoroutine(Co_SelectAction(exitButton, startButton, () =>
        {
            // 1초 후 실행될 동작: 게임 종료
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }));
    }

    // 버튼 선택 연출 및 딜레이 처리 코루틴 (DialogueSystem의 Co_SelectBranch 로직 변형)
    private IEnumerator Co_SelectAction(Button selectedBtn, Button otherBtn, System.Action onComplete)
    {
        isBusy = true;

        // 1. 모든 버튼 인터랙션 비활성화
        startButton.interactable = false;
        exitButton.interactable = false;

        // 2. 선택된 버튼 연출 (클릭 -> TV 꺼짐 효과)
        var selectedEffect = selectedBtn.GetComponent<DialogueButtonEffect2>();
        if (selectedEffect != null)
        {
            selectedEffect.PlayClickAnimation();
        }

        // 클릭감(진동 등)을 위한 짧은 대기 (DialogueSystem 참조: 0.1f)
        yield return new WaitForSeconds(0.1f);

        if (selectedEffect != null)
        {
            selectedEffect.PlaySelectedCloseAnimation();
        }

        // 3. 선택되지 않은 버튼 연출 (모래처럼 부서짐)
        var otherEffect = otherBtn.GetComponent<DialogueButtonEffect2>();
        if (otherEffect != null)
        {
            otherEffect.PlayDiscardCloseAnimation();
        }

        // 4. 요청하신 1초 대기 (연출이 끝나는 시간 + 여운)
        yield return new WaitForSeconds(1.0f);

        // 5. 최종 동작 실행 (씬 이동 또는 종료)
        onComplete?.Invoke();
    }
}