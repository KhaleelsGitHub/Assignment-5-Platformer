using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;

    private Animator anim;
    private NewBehaviourScript playerMovement;
    private float cooldownTimer = Mathf.Infinity; //mathf.infinity is used so the player can attack right away even though the cooldown timer is defaulted to 0

    //Creating variables for the player movement script and the animator 
    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<NewBehaviourScript>();
    }

    //Update function checks for attack input on every frame
    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        fireBalls[FindFireball()].transform.position = firePoint.position;
        fireBalls[FindFireball()].GetComponent<Projectile>().setDirection(Mathf.Sign(transform.localScale.x));
        //pool fireballs
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireBalls.Length; i++)
        {
            if (!fireBalls[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
