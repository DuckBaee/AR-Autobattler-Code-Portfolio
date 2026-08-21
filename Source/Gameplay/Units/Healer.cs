using System;
using UnityEngine;

public class Healer : Character
{
    public float HealRadius { get; private set; } = 0.4f;

    private float _lastHealTime = 0f; // 마지막 힐 타이밍
    private float _healInterval = 0.3f; // 힐 주기 (1초)

    private void Awake()
    {
        CharacterName = "Healer";
        Health = 50f;
        MaxHealth = Health;
        AttackPower = 5f;
        MoveSpeed = 0.1f;
        AttackRange = 0.2f;
        AttackSpeed = 0.5f;
    }

    private void Start()
    {
        if (gameObject.TryGetComponent(out _agent))
        {
            _agent.SetVariableValue<float>("NavMeshAgent_Speed", MoveSpeed);
            _agent.SetVariableValue<float>("NavMeshAgent_AttackRange", AttackRange - 0.01f);
        }
        else
        {
            Debug.Log("device_debug : failure");
        }
    }

    private void FixedUpdate()
    {
        if (Time.time - _lastHealTime >= _healInterval)
        {
            _lastHealTime = Time.time;
            HealNearbyAllies();    
        }
    }

    public override void Attack(Character ally)
    {
        if (Vector3.Distance(transform.position, ally.transform.position) <= AttackRange)
        {
            PerformAttack(ally);
        }
    }

    private void HealNearbyAllies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, HealRadius);

        foreach (var hit in hitColliders)
        {
            Character ally = hit.GetComponent<Character>();
            if (ally != null && ally != this && !ally.gameObject.CompareTag("Enemy")) // 자신을 제외한 아군만 힐
            {
                ally.Heal(AttackPower / 3);
            }
        }
    }
}