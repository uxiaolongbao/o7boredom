using UnityEngine;

public class LadderDetector : MonoBehaviour
{
    [SerializeField]
    private PlayerMove player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Ladder>())
        {
            player.ClimbingAllowed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Ladder>())
        {
            player.ClimbingAllowed = false;
        }
    }
}
