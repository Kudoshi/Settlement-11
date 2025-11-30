using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class TrainPair
{
    public GameObject trainObject;
    public Transform pointA;
    public Transform pointB;
    public float travelTime = 5f;
    public Ease easeType = Ease.InOutSine;
}

public class SubwayTrainSystem : MonoBehaviour
{
    [Header("Train Configuration")]
    public List<TrainPair> trainPairs = new List<TrainPair>();

    [Header("Timing Settings")]
    public float minDelayBetweenTrains = 2f;
    public float maxDelayBetweenTrains = 5f;
    public bool autoStart = true;

    [Header("Randomization")]
    public bool randomizeSpeed = true;
    public float speedVariation = 0.3f; // +/- 30% variation

    private int currentTrainIndex = 0;
    private bool isSystemRunning = false;

    private void Start()
    {
        if (autoStart)
        {
            StartSystem();
        }
    }

    public void StartSystem()
    {
        if (trainPairs.Count == 0)
        {
            Debug.LogError("SubwayTrainSystem: No train pairs assigned!");
            return;
        }

        isSystemRunning = true;
        MoveNextTrain();
    }

    public void StopSystem()
    {
        isSystemRunning = false;
        DOTween.KillAll();
    }

    private void MoveNextTrain()
    {
        if (!isSystemRunning || trainPairs.Count == 0)
            return;

        TrainPair currentPair = trainPairs[currentTrainIndex];

        if (currentPair.trainObject == null || currentPair.pointA == null || currentPair.pointB == null)
        {
            Debug.LogWarning($"Train pair {currentTrainIndex} has missing references!");
            ScheduleNextTrain();
            return;
        }

        // Position train at point A
        currentPair.trainObject.transform.position = currentPair.pointA.position;
        currentPair.trainObject.transform.rotation = currentPair.pointA.rotation;

        // Calculate travel time with randomization
        float travelTime = currentPair.travelTime;
        if (randomizeSpeed)
        {
            float variation = Random.Range(-speedVariation, speedVariation);
            travelTime *= (1f + variation);
        }

        // Move train from A to B
        currentPair.trainObject.transform
            .DOMove(currentPair.pointB.position, travelTime)
            .SetEase(currentPair.easeType)
            .OnComplete(() => OnTrainArrived());

        // Optional: Rotate to face direction
        currentPair.trainObject.transform
            .DORotateQuaternion(currentPair.pointB.rotation, travelTime * 0.2f)
            .SetEase(Ease.InOutQuad);

        Debug.Log($"Train {currentTrainIndex} departing from A to B");
    }

    private void OnTrainArrived()
    {
        Debug.Log($"Train {currentTrainIndex} arrived at B");

        // Schedule next train
        ScheduleNextTrain();
    }

    private void ScheduleNextTrain()
    {
        if (!isSystemRunning)
            return;

        // Move to next train in the list
        currentTrainIndex = (currentTrainIndex + 1) % trainPairs.Count;

        // Random delay before next train
        float delay = Random.Range(minDelayBetweenTrains, maxDelayBetweenTrains);

        DOVirtual.DelayedCall(delay, () => MoveNextTrain());

        Debug.Log($"Next train ({currentTrainIndex}) scheduled in {delay:F1}s");
    }

    private void OnDestroy()
    {
        // Clean up tweens
        DOTween.Kill(this);
    }

    // Gizmos to visualize paths in editor
    private void OnDrawGizmos()
    {
        if (trainPairs == null || trainPairs.Count == 0)
            return;

        for (int i = 0; i < trainPairs.Count; i++)
        {
            TrainPair pair = trainPairs[i];
            if (pair.pointA != null && pair.pointB != null)
            {
                // Draw line from A to B
                Gizmos.color = new Color(i * 0.2f, 1f - (i * 0.2f), 0.5f, 0.5f);
                Gizmos.DrawLine(pair.pointA.position, pair.pointB.position);

                // Draw spheres at points
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(pair.pointA.position, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(pair.pointB.position, 0.5f);
            }
        }
    }
}
