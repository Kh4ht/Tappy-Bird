using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    private GameManager gm;
    private const string Highest_Score_key = "Highest_Score",
                         Last_Score_key = "Last_Score";

    [Header("_-_-_-_-_ UI -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-")]
    [SerializeField] private Image Fade_Image;
    [SerializeField] private Button changebird_btn;

    [SerializeField] private TextMeshProUGUI Score_txt,
                                             TouchToStart_txt,
                                             LastScore_txt;

    [SerializeField] private GameObject birdsScrollView,
                                        ScrollBar;

    [SerializeField] private RectTransform Content_img;
    [HideInInspector] public List<RectTransform> Contents = new();

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Prevent duplicates

        gm = GameManager.Ins;

        foreach (RectTransform ch in Content_img)
        {
            Contents.Add(ch);
        }
    }

    private void Start()
    {
        AnimateTouchToStart_txt();

        FadeOutImage();

        if (PlayerPrefs.GetInt(Last_Score_key, 0) == 0)
        {
            LastScore_txt.gameObject.SetActive(false);
        }
        else
        {
            LastScore_txt.text = $"Your Last Score was : {PlayerPrefs.GetInt(Last_Score_key, 0)} !";
        }

        Score_txt.text = "Highest Score : " + PlayerPrefs.GetInt(Highest_Score_key, 0);
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void AnimateTouchToStart_txt()
    {
        TouchToStart_txt.GetComponent<RectTransform>().DOShakeScale(duration: 1,
                                                                    strength: 0.1f,
                                                                    vibrato: 1
                                                                    ).OnComplete(AnimateTouchToStart_txt)
                                                                    .SetUpdate(true);
    }

    public void When_LVL_isStarted()
    {
        changebird_btn.GetComponent<RectTransform>().DOAnchorPosX(-150, 0.2f);
    }

    public async void Open_Close_BirdsScrollView_btn()
    {
        if (!birdsScrollView.activeInHierarchy
            && !gm.IsLevelStarted)
        {
            birdsScrollView.SetActive(true);
            birdsScrollView.GetComponent<RectTransform>().DOAnchorPosY(100, 0.2f).SetUpdate(true);
        }
        else
        {
            await birdsScrollView.GetComponent<RectTransform>().DOAnchorPosY(-110, 0.2f).SetUpdate(true).AsyncWaitForCompletion();
            birdsScrollView.SetActive(false);
        }
    }

    public async void AnimateChangebtn()
    {
        var txtTransform = changebird_btn.GetComponent<RectTransform>().Find("txt");

        // Check if there's already a tween playing on the scale before creating a new one
        if (DOTween.IsTweening(txtTransform)) return;

        var tween = txtTransform.DOPunchScale(
            punch: new Vector3(0.1f, 0.1f, 0.1f),
            duration: 0.3f,
            vibrato: 1,
            elasticity: 0
        ).SetUpdate(true);

        await tween.AsyncWaitForCompletion(); // Wait until the tween completes before allowing another one    }
    }

    public void TouchToStart_txt_localscale()
    {
        TouchToStart_txt.GetComponent<RectTransform>().DOKill(); // Stop any running tweens on this object

        TouchToStart_txt.GetComponent<RectTransform>().DOScale(0, 1)
            .SetUpdate(true)
            .OnComplete(() => TouchToStart_txt.gameObject.SetActive(false));
    }

    public void Score_Increase()
    {
        gm.Score++;

        if (gm.Score > 0)
        {
            Score_txt.transform.localScale = Vector3.one; // Reset scale

            Score_txt.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutQuad)
                .OnComplete(() => Score_txt.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad));
        }

        Score_txt.text = gm.Score.ToString();
    }

    public void FadeInImage()
    {
        if (Fade_Image != null)
        {
            Fade_Image.gameObject.SetActive(true);

            Fade_Image.DOFade(1, 2).SetUpdate(true)
                .OnComplete(() =>
                {
                    DOTween.KillAll(); // Stop all active tweens
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    DOTween.KillAll();
                });
        }
    }

    private void FadeOutImage()
    {
        Fade_Image.gameObject.SetActive(true);

        Fade_Image.DOFade(0, 2).OnComplete(() => Fade_Image.gameObject.SetActive(false)).SetUpdate(true);
    }
}