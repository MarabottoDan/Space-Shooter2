using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition; // Save camera's original position
        float elapsed = 0.0f; // Track how long we've been shaking

        while (elapsed < duration)
        {
            // Create a random offset for X and Y based on the magnitude
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Apply that offset to the camera’s local position
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime; // Add the time that just passed
            yield return null; // Wait for the next frame
        }

        // Return the camera to its exact original position after the shake
        transform.localPosition = originalPos;
    }
}
