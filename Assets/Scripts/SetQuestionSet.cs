using UnityEngine;

public class SetQuestionSet : MonoBehaviour
{
    public int questionSet;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
            GameManager.Instance.currentQuestionSet = questionSet;
    }
}