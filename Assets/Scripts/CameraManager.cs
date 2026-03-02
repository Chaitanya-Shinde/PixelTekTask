using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCameraBase[] cameras;

    [SerializeField]
    private int activePriority = 20;

    [SerializeField]
    private int inactivePriority = 5;

    private int currentIndex = 0;

    void Start()
    {
        ActivateCamera(currentIndex);
    }

    public void CycleCamera()
    {
        currentIndex++;

        if (currentIndex >= cameras.Length)
            currentIndex = 0;

        ActivateCamera(currentIndex);
    }

    void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == index) ? activePriority : inactivePriority;
        }
    }
}