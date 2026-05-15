using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    public Camera openingCamera;
    public Canvas mainCanvas;
    public Canvas openingCanvas;


    [Header("Detection")]
    public static bool startScreen = true;

    private void Awake()
    {
        mainCam.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(false);
    }

    public void OnPlay()
    {
        //hide opening canvas and show main one
        openingCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
        //coroutine to move camera
        StartCoroutine(CameraMovement());
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    IEnumerator CameraMovement()
    {
        Vector3 startPos = openingCamera.transform.position;
        Vector3 startRotation = openingCamera.transform.eulerAngles;
        for (float i = 0f; i <= 1f; i += Time.deltaTime)
        {
            openingCamera.transform.position = Vector3.Lerp(startPos, mainCam.transform.position, i);
            openingCamera.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, mainCam.transform.eulerAngles, i));
            yield return null;
        }
        Debug.Log("Finished");
        ChangeCamera();
        yield break;
    }

    public void ChangeCamera()
    {
        openingCamera.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
        StartScreenUpdate(false);
    }
    public static void StartScreenUpdate(bool isActive)
    {
        startScreen = isActive;
        if (startScreen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } 
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
