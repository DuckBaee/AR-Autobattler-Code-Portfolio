using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Action_Attack", story: "Process Action In Character : [Agent] attacks [Target]", category: "Custom/Character/Action", id: "7bd1fbda1e32db4d5a64053315904023")]
public partial class ActionAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        // Agent와 Target이 null인지 확인
        if (Agent == null || Agent.Value == null)
        {
            Debug.LogError("Agent is null or uninitialized.");
            return Status.Failure;
        }

        if (Target == null || Target.Value == null)
        {
            Debug.LogWarning("Target is null. Waiting for a valid target...");
            return Status.Running;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent == null || Agent.Value == null)
        {
            Debug.LogError("Agent is null or uninitialized during update.");
            return Status.Failure;
        }

        Character agentCharacter = Agent.Value.GetComponent<Character>();
        if (agentCharacter == null)
        {
            Debug.LogError("Agent does not have a Character component.");
            return Status.Failure;
        }

        if (Target == null || Target.Value == null)
        {
            Debug.LogWarning("Target is null. Skipping update...");
            return Status.Failure;
        }

        // 공격 논리 처리 (공격 범위 확인)
        if (agentCharacter.AttackRange >= Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position) && Agent.Value.name != "Healer")
        {
            Character targetCharacter = Target.Value.GetComponent<Character>();
            if (targetCharacter == null)
            {
                Debug.LogError("Target does not have a Character component.");
                return Status.Failure;
            }

            agentCharacter.Attack(targetCharacter);
            return Status.Success;
        }
        else if (Agent.Value.name == "Healer")
        {
            Vector3 agentPosition = Agent.Value.transform.position;

            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ally");
            if (gameObjects.Length == 0)
            {
                Debug.LogWarning("No allies found for healer to heal.");
                return Status.Failure;
            }

            float closestDistanceSq = Mathf.Infinity;
            GameObject closestGameObject = null;

            foreach (GameObject gameObject in gameObjects)
            {
                float distanceSq = Vector3.SqrMagnitude(agentPosition - gameObject.transform.position);
                if (closestGameObject == null || distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closestGameObject = gameObject;
                }
            }

            if (closestGameObject != null)
            {
                Target.Value = closestGameObject;

                Character allyCharacter = Target.Value.GetComponent<Character>();
                if (allyCharacter != null)
                {
                    allyCharacter.Heal(agentCharacter.AttackPower);
                    Debug.Log($"Healer healed {allyCharacter.CharacterName} for {agentCharacter.AttackPower} HP.");
                    return Status.Success;
                }
            }

            return Status.Failure;
        }

        return Status.Failure;
    }

    protected override void OnEnd()
    {
        // 행동 종료 시 필요한 정리 작업
    }
}


