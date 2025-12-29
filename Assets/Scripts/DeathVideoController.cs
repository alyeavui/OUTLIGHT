using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class DeathVideoController : MonoBehaviour
{
    [SerializeField] private GameObject videoCanvasObject;
    [SerializeField] private VideoPlayer videoPlayer;
    private Canvas videoCanvas;
    private static DeathVideoController instance;
    private bool isPlayingVideo = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (videoCanvasObject == null || videoPlayer == null) return;

        videoCanvas = videoCanvasObject.GetComponent<Canvas>();
        if (videoCanvas != null)
        {
            videoCanvas.sortingOrder = 1000;
        }

        videoCanvasObject.SetActive(false);
    }

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.skipOnDrop = true;
        }
    }

    public void PlayDeathVideo()
    {
        if (isPlayingVideo || videoPlayer == null || videoCanvasObject == null || videoPlayer.clip == null)
        {
            ShowGameOverDirectly();
            return;
        }

        isPlayingVideo = true;
        Time.timeScale = 1f;

        videoCanvasObject.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.Play();

        StartCoroutine(VideoSafetyTimer());
    }

    private IEnumerator VideoSafetyTimer()
    {
        float waitTime = videoPlayer.clip != null ? (float)videoPlayer.clip.length + 1f : 5f;
        yield return new WaitForSecondsRealtime(waitTime);

        if (isPlayingVideo)
        {
            OnVideoFinished(videoPlayer);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!isPlayingVideo) return;

        isPlayingVideo = false;

        if (videoCanvasObject != null)
        {
            videoCanvasObject.SetActive(false);
        }

        ShowGameOverDirectly();
    }

    private void ShowGameOverDirectly()
    {
        if (UI.Instance != null)
        {
            UI.Instance.ShowGameOverUI("You died!");
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static DeathVideoController GetInstance()
    {
        return instance;
    }
}
