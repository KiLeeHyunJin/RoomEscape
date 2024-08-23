using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MoveMiniGame : MonoBehaviour
{
    [SerializeField] Button[] lopes;
    [SerializeField] int[] currentState;
    [SerializeField] int[] answer;
    [SerializeField] Animator animator;
    [SerializeField] GameObject clearPart;
    public int changeState;
    public int questionNum;
    public void ObjectMove(int i)
    {
        StartCoroutine(Animation(i));
    }
    private void CheckAnswer()
    {
        for (int i = 0; i < answer.Length; i++)
        {
            if(currentState[i] != answer[i])
            {
                return;
            }
        }
        clearPart.SetActive(true);
        GameEffect();
    }
    IEnumerator Animation(int i)
    {
        animator.SetTrigger($"Lope{i + 1}");
        currentState[i]++;
        yield return new WaitForSeconds(1f);
        CheckAnswer();
    }
    public virtual void GameEffect() { }
}