using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MiniGames.SAS;

public class Slot : MonoBehaviour
{
    public GameObject emptyBlockPrefab;
    public float insertTime = 0.4f;
    public float matchTime = 0.4f;
    public float shuffleTime = 1f;

    public Vector2 offset;
    public Vector2 blockSize;
    public Transform mapContent;


    public List<int> patternSlot;

    public int matchLock = 0;
    public bool insertLock = false;
    public bool skillLock = false;

    private void Awake()
    {
        mapContent = transform.parent.Find("Map");
        var hlg = transform.GetComponent<HorizontalLayoutGroup>();
        offset = new Vector2(hlg.padding.left + blockSize.x / 2.0f, -(hlg.padding.top + blockSize.y / 2.0f));
    }

    private void Update()
    {
        if (matchLock > 0 || skillLock) return;
        MatchBlock();
    }

    public void InsertBlock(Transform block, int pattern)
    {
        // ���ó�ȥ
        block.GetComponent<Button>().enabled = false;
        block.SetParent(transform.parent);

        // �����patternӦ�ò����λ��
        matchLock++;
        int insertIndex = GetInsertIndex(pattern);
        patternSlot.Insert(insertIndex, pattern);

        // ����һ����λ��
        Transform emptyBlock = Instantiate(emptyBlockPrefab, transform).transform;
        emptyBlock.SetSiblingIndex(insertIndex);

        // ����λ��
        Vector2 targetPos = (transform as RectTransform).anchoredPosition + offset + 
            new Vector2(insertIndex * 100, 0);

        // ʹ�� DOTween �� Block ��λ���ƶ���ת����ı�������
        (block as RectTransform).DOAnchorPos(targetPos, insertTime).SetEase(Ease.InOutQuad).
            OnComplete(()=>
            {
                // ����������ʱ��
                int index = emptyBlock.GetSiblingIndex();
                Destroy(emptyBlock.gameObject);
                block.SetParent(transform);
                block.SetSiblingIndex(index);
                matchLock--;
            });
    }

    private int GetInsertIndex(int pattern)
    {
        int n = patternSlot.Count;
        int insertIndex = -1;
        for(int i = 0; i < n; ++i)
        {
            if (patternSlot[i] == pattern)
            {
                if (i + 1 >= n || patternSlot[i + 1] != pattern)
                {
                    if (i - 1 < 0 || i - 2 < 0 ||
                        (!(patternSlot[i] == patternSlot[i - 1] && patternSlot[i] == patternSlot[i - 2])))
                        return i + 1;
                }
            }
        }
        if (insertIndex == -1) insertIndex = n;
        return insertIndex;
    }

    public void MatchBlock()
    {
        int n = patternSlot.Count;

        // ��� patternSlot ��������������һ������ֵ
        List<int> matchIndices = new List<int>();
        for (int i = 0; i < n - 2; ++i)
        {
            if (patternSlot[i] == patternSlot[i + 1] && patternSlot[i] == patternSlot[i + 2])
            {
                // ���ƥ�����������
                matchIndices.Add(i);
                matchIndices.Add(i + 1);
                matchIndices.Add(i + 2);
                i += 2;
            }
        }

        // ���û���ҵ�ƥ�䣬�򷵻�
        if (matchIndices.Count == 0)
            return;

        insertLock = true;

        SASAudioManager.Instance.PlayMatchEffect();

        List<GameObject> destoryList = new List<GameObject>();

        // ����ƥ���������ɾ����Ӧ��Block
        foreach (int index in matchIndices)
        {
            Transform block = transform.GetChild(index);
            destoryList.Add(block.gameObject);
        }

        // ɾ�� patternSlot ��ƥ�����ֵ
        for (int i = matchIndices.Count - 1; i >= 0; --i)
        {
            patternSlot.RemoveAt(matchIndices[i]);
        }
        foreach (var go in destoryList)
        {
            Destroy(go);
        }

        insertLock = false;
    }

    public void WithdrawBlock(string blockName, int pattern, int row, int col)
    {
        skillLock = true;

        int withdrawIndex = -1;

        for (int i = patternSlot.Count - 1; i >= 0; --i)
        {
            if (patternSlot[i] == pattern) withdrawIndex = i;
        }

        if (withdrawIndex == -1)
        {
            Debug.LogError("Slot withdraw ����δ�ҵ�pattern:" + pattern);
            return;
        }

        patternSlot.RemoveAt(withdrawIndex);

        // �ó�ȥ
        var block = transform.Find(blockName);
        int ei = block.GetSiblingIndex();
        block.SetParent(mapContent);
        // ռλ
        Transform emptyBlock = Instantiate(emptyBlockPrefab, transform).transform;
        emptyBlock.SetSiblingIndex(ei);

        Vector2 targetPos = row == -1 ? 
            new Vector2((SASGameManager.Instance.blockMapdata.colNum - 1) * 50 / 2.0f + (col - 1) * 100,
                        -(SASGameManager.Instance.blockMapdata.rowNum + 2) * 50) : 
            new Vector2(col * 50, -row * 50);

        SASAudioManager.Instance.PlayWithdrawEffect();
        // ʹ�� DOTween �� Block ��λ���ƶ���ת����ı�������
        (block as RectTransform).DOAnchorPos(targetPos, insertTime).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                skillLock = false;
                Destroy(emptyBlock.gameObject);
                EventCenter.Broadcast(EventDefine.SAS_RefreshOffsetPosition, SASGameManager.Instance.blockMapdata);
            });



    }

    public void ShuffleBlock()
    {
        skillLock = true;
        List<Vector2> originalPositions = new List<Vector2>();
        // ��ȡ�����������ԭʼλ��
        foreach (Transform child in mapContent)
        {
            originalPositions.Add((child as RectTransform).anchoredPosition);
        }

        // ��ȡMap�����ĵ�
        Vector2 mapCenter = new Vector2(25 * (SASGameManager.Instance.mapdata.colNum-1),
            -25 * (SASGameManager.Instance.mapdata.rowNum-1));

        Debug.Log("mapcenter:" + mapCenter);

        SASAudioManager.Instance.PlayShuffleEffect();
        // ����һ������������ϴ�ƶ���
        Sequence shuffleSequence = DOTween.Sequence();

        // �����е����������ȼ��е����ĵ�
        foreach (RectTransform child in mapContent)
        {
            shuffleSequence.Join(child.DOAnchorPos(mapCenter, shuffleTime / 2.0f).SetEase(Ease.OutQuad));
        }

        // �ڼ��е����ĵ�����һ���ӳ�Ȼ������������ص�ԭʼλ��
        shuffleSequence.AppendInterval(shuffleTime / 2.0f);

        // ��������ص�ԭʼλ��
        foreach (RectTransform child in mapContent)
        {
            shuffleSequence.Join(child.DOAnchorPos(originalPositions[child.GetSiblingIndex()], shuffleTime / 2.0f).SetEase(Ease.OutQuad));
        }

        // ����������������ܲ��㲥�¼�
        shuffleSequence.OnComplete(() =>
        {
            skillLock = false;
            EventCenter.Broadcast(EventDefine.SAS_RefreshBlockMap, SASGameManager.Instance.blockMapdata);
        });


    }

    public void Pop3Block()
    {
        skillLock = true;

        // ��ֹ�ô���matchһ��
        MatchBlock();

        SASAudioManager.Instance.PlayPopEffect();
        // ��ͷ�����ƶ����ض�λ��
        Sequence seq = DOTween.Sequence();
        GameObject[] emptyBlocks = new GameObject[3];

        for (int i = 0; i < 3; ++i)
        {
            if (i >= patternSlot.Count) break;

            var block = transform.GetChild(i);
            block.name = string.Format(Constants.LRC_TPL, 
                SASGameManager.Instance.blockMapdata.zonePattern[i].Count-1, -1, i);
            block.SetParent(mapContent);

            // ռλ
            emptyBlocks[i] = Instantiate(emptyBlockPrefab, transform);
            emptyBlocks[i].transform.SetSiblingIndex(i);

            Vector2 targetPos =
                new Vector2((SASGameManager.Instance.blockMapdata.colNum - 1) * 50 / 2.0f + (i - 1) * 100,
                -(SASGameManager.Instance.blockMapdata.rowNum + 2) * 50);
            // ʹ�� DOTween �� Block ��λ���ƶ���ת����ı�������
            seq.Join((block as RectTransform).DOAnchorPos(targetPos, insertTime).SetEase(Ease.InOutQuad));
        }

        seq.OnComplete(() =>
        {
            skillLock = false;
            for (int i = 2; i >= 0; --i)
            {
                if (emptyBlocks[i] != null)
                {
                    Destroy(emptyBlocks[i]);
                    patternSlot.RemoveAt(i);
                }
            }
            EventCenter.Broadcast(EventDefine.SAS_RefreshBlockMap, SASGameManager.Instance.blockMapdata);
        });

    }

    public void Clear()
    {
        patternSlot.Clear();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
