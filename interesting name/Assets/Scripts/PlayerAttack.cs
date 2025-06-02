using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] pewpews;
    private Animator anim;
    private PlayerMove playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMove>();

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && cooldownTimer > attackCooldown)
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        //pool fireballs
        pewpews[FindPewpew()].transform.position = firePoint.position;
        pewpews[FindPewpew()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindPewpew()
    {
        for (int i = 0; i < pewpews.Length; i++)
        {
            if (!pewpews[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
