using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Messenger<string>.Broadcast(GameEvent.SCORE_UPDATED, tag);
        }
    }
}
