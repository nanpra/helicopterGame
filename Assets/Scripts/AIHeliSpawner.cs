using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class AIHeliSpawner : MonoBehaviour
{
    public GameObject helicopterPrefab;
    public Transform spawnPoint;
    public CinemachineCamera heliCam;
    public CinemachineCamera mainCam;
    public float cameraFocusDuration = 5f;

    private GameObject spawnedHelicopter;

    public void SpawnAndFocus()
    {
        // Spawn the helicopter
        spawnedHelicopter = Instantiate(helicopterPrefab, spawnPoint.position, spawnPoint.rotation);

        // Switch to helicopter camera
        StartCoroutine(SwitchToHelicopterCamera());
    }

    private IEnumerator SwitchToHelicopterCamera()
    {
        heliCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);

        yield return new WaitForSeconds(cameraFocusDuration);

        heliCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
    }
}
