﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyState
{
    CHASE,
    ATTACK
}

public class EnemyController : MonoBehaviour
{


    private CharacterAnimations enemy_Anim;
    private NavMeshAgent navAgent;
    private Transform playerTarget;
    public float move_Speed = 3.5f;
    public float attack_Distance = 1f;
    public float chase_Player_After_Attack_Distance = 1f;
    private float wait_Before_Attack_Time = 3f;
    private float attack_Timer;
    private EnemyState enemy_State;
    public int startingHealth = 100;


    private int CurrentHealth;
    private Vector3 StartPosition;

    public EnemyManager enemyManager;

    // Start is called before the first frame update
    void Awake()
    {
        enemy_Anim = GetComponent<CharacterAnimations>();
        navAgent = GetComponent<NavMeshAgent>();
        playerTarget = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).transform;

    }


    void Start()
    {   
        StartPosition = transform.position;
        CurrentHealth = startingHealth;
        enemy_State = EnemyState.CHASE;
        attack_Timer = wait_Before_Attack_Time;
        navAgent = GetComponent<NavMeshAgent>();
    }

  //  private void FixedUpdate()
   // {
   //     navAgent.destination = Agent.transform.position;
        //transform.position = Vector3.MoveTowards(transform.position, Agent.transform.position, Time.fixedDeltaTime * speed);
 //   }
    // Update is called once per frame
    void Update()
    {
        if(enemy_State == EnemyState.CHASE)
        {
            ChasePlayer();
        }
        if(enemy_State == EnemyState.ATTACK)
        {
            AttackPlayer();
        }
    }

    public void GetShot(int damage, PlayerAttackInput shooter)
    {
        ApplyDamage(damage, shooter);
    }
    
    private void ApplyDamage(int damage, PlayerAttackInput shooter)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die(shooter);
        }
    }
    
    private void Die(PlayerAttackInput shooter)
    {
        shooter.RegisterKill();
        
        gameObject.SetActive(false);
        enemyManager.RegisterDeath();
    }

    void ChasePlayer()
    {
        navAgent.SetDestination(playerTarget.position);
        navAgent.speed = move_Speed;
        if(navAgent.velocity.sqrMagnitude == 0)
        {
            enemy_Anim.Walk(false);
        }
        else
        {
            enemy_Anim.Walk(true);
        }
        if(Vector3.Distance(transform.position,playerTarget.position) <= attack_Distance)
        {
            enemy_State = EnemyState.ATTACK;
        }
    }

    void AttackPlayer()
    {
        navAgent.velocity = Vector3.zero;
        navAgent.isStopped = true;
        enemy_Anim.Walk(false);
        attack_Timer += Time.deltaTime;
        if(attack_Timer > wait_Before_Attack_Time)
        {
            if(Random.Range(0,2)>0)
            {
                enemy_Anim.Attack_1();
            }
            else
            {
                enemy_Anim.Attack_2();
            }
            attack_Timer = 0f;
        }
        if(Vector3.Distance(transform.position,playerTarget.position) > attack_Distance + chase_Player_After_Attack_Distance)
        {
            navAgent.isStopped = false;
            enemy_State = EnemyState.CHASE;
        }
    } 

}
