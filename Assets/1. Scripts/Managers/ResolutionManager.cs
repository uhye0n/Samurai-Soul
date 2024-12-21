using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private int targetWidth = 720;  // 원하는 가로 해상도
    [SerializeField] private int targetHeight = 1080; // 원하는 세로 해상도
    [SerializeField] private bool fullscreen = false;  // 전체 화면 여부

    void Start()
    {
        SetResolution(targetWidth, targetHeight, fullscreen);
    }

    public void SetResolution(int width, int height, bool isFullscreen)
    {
        Screen.SetResolution(width, height, isFullscreen);
        Debug.Log($"해상도가 {width}x{height}로 설정되었습니다 (전체 화면: {isFullscreen}).");
    }
}
