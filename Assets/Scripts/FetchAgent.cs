using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FetchAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer FloorMeshRenderer;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-6f, 0.5f, -4.5f);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        float moveX = -actions.ContinuousActions[1];

        float moveSpeed = 2.0f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TagTarget>(out TagTarget tagTarget))
        {
            SetReward(1.0f);
            FloorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<TagWall>(out TagWall tagWall))
        {
            SetReward(-1.0f);
            FloorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}
