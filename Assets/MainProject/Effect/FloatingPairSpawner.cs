using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPairSpawner : MonoBehaviour
{
    public Sprite[] mainImages;
    public Sprite upSprite;
    public Sprite middleSprite;
    public Sprite downSprite;

    public GameObject floatingPairPrefab;

    public float stackSpacing = 0.5f;

    public float cooltime = 0.5f;

    private Dictionary<Transform, int> stackCounts = new Dictionary<Transform, int>();

    // 스폰 요청을 저장하는 큐
    private struct SpawnRequest
    {
        public int pairIndex;
        public int subType;
        public Transform target;
    }

    private Queue<SpawnRequest> spawnQueue = new Queue<SpawnRequest>();
    private bool isProcessingQueue = false;

    public static FloatingPairSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnFloatingPair(int pairIndex, int subType, Transform targetPos)
    {
        // 요청을 큐에 넣기
        spawnQueue.Enqueue(new SpawnRequest()
        {
            pairIndex = pairIndex,
            subType = subType,
            target = targetPos
        });

        // 큐를 처리하는 코루틴이 없으면 시작
        if (!isProcessingQueue)
            StartCoroutine(ProcessQueue());
    }

    // 🔵 큐를 순서대로 처리하는 코루틴
    private IEnumerator ProcessQueue()
    {
        isProcessingQueue = true;

        while (spawnQueue.Count > 0)
        {
            SpawnRequest req = spawnQueue.Dequeue();
            DoSpawn(req.pairIndex, req.subType, req.target);

            yield return new WaitForSeconds(cooltime);
        }

        isProcessingQueue = false;
    }

    private void DoSpawn(int pairIndex, int subType, Transform targetPos)
    {
        if (targetPos == null) return;

        if (!stackCounts.ContainsKey(targetPos))
            stackCounts[targetPos] = 0;

        int count = stackCounts[targetPos];

        Vector3 spawnPos = targetPos.position + new Vector3(0, count * stackSpacing, 0);
        GameObject obj = Instantiate(floatingPairPrefab, spawnPos, Quaternion.identity);

        // 메인
        var mainR = obj.transform.Find("MainImage").GetComponent<SpriteRenderer>();
        mainR.sprite = mainImages[pairIndex];

        // 서브
        var subR = obj.transform.Find("SubImage").GetComponent<SpriteRenderer>();
        switch (subType)
        {
            case 1: subR.sprite = upSprite; break;
            case 2: subR.sprite = middleSprite; break;
            case 3: subR.sprite = downSprite; break;
        }

        stackCounts[targetPos]++;

        obj.GetComponent<FloatingPairObject>().StartFloating();

        StartCoroutine(ReduceStackRoutine(targetPos));
    }

    private IEnumerator ReduceStackRoutine(Transform t)
    {
        yield return new WaitForSeconds(2f);

        if (!stackCounts.ContainsKey(t)) yield break;

        stackCounts[t]--;
        if (stackCounts[t] < 0) stackCounts[t] = 0;
    }
}
