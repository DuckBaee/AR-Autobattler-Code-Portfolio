using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Ally With Lowest Health Ratio", story: "Find [Object] closest [Self] With [Tag] Exclude This Agent and select lowest health ratio", category: "Custom/Character/Action", id: "f8d532fe764471e3482daca85a227d5c")]
public partial class FindAllyWithLowestHealthRatioAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<string> Tag;
    // The closest ally to store in the blackboard
    // Reference to the "Self" object
    // Tag to search for

    protected override Status OnStart()
    {
        // Ensure all blackboard variables are valid
        if (Self == null || Tag == null)
        {
            Debug.LogError("One or more Blackboard variables are not set.");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Validate inputs
        if (Self.Value == null || string.IsNullOrEmpty(Tag.Value))
        {
            Debug.LogError("Self or Tag value is null/empty.");
            return Status.Failure;
        }

        // Find all objects with the specified tag
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(Tag.Value);

        if (objectsWithTag.Length == 0)
        {
            Debug.LogWarning("No objects found with tag: " + Tag.Value);
            return Status.Failure;
        }

        GameObject closestObject = null;
        float shortestDistance = Mathf.Infinity;
        float lowestHealthRatio = Mathf.Infinity;
        Vector3 selfPosition = Self.Value.transform.position;

        foreach (var obj in objectsWithTag)
        {
            if (obj == Self.Value) continue; // Exclude self

            // Access the Character component and its Health properties
            var characterComponent = obj.GetComponent<Character>();
            if (characterComponent == null)
            {
                Debug.LogWarning($"Object {obj.name} does not have a Character component.");
                continue;
            }

            // Calculate health ratio
            float maxHealth = characterComponent.MaxHealth;
            if (maxHealth <= 0)
            {
                Debug.LogWarning($"Object {obj.name} has invalid MaxHealth value.");
                continue;
            }

            float healthRatio = characterComponent.Health / maxHealth;
            float distance = Vector3.Distance(selfPosition, obj.transform.position);

            // Select the object with the lowest health ratio, prioritizing proximity in case of a tie
            if (healthRatio < lowestHealthRatio || (Mathf.Approximately(healthRatio, lowestHealthRatio) && distance < shortestDistance))
            {
                lowestHealthRatio = healthRatio;
                shortestDistance = distance;
                closestObject = obj;
            }
        }

        if (closestObject != null)
        {
            Object.Value = closestObject;
            return Status.Success;
        }

        Debug.LogWarning("No valid ally with lowest health ratio found.");
        return Status.Failure;
    }

    protected override void OnEnd()
    {
        // Cleanup or reset logic if necessary
    }
}
