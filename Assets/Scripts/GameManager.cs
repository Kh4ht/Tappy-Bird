using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Ins { get; private set; }
    private Coins_Manager cm;

    [System.Serializable]
    public class Birds
    {
        public GameObject Bird_Prefab;
        public int Value_Coins;
    }
    public Birds[] birds;

    private const string Highest_Score_key = "Highest_Score",
                         Last_Score_key = "Last_Score",
                         Selected_Bird_key = "Selected_Bird",
                         Unlocked_Birds_num_key = "Unlocked_Birds_num";

    private UI_Manager UIm;
    private AudioSource sfx;
    private Bird bird;

    private Tween changebirdTween,
                  pauseTween;

    [HideInInspector] public bool IsLevelStarted,
                                  IsPaused;

    [Header("_-_-_-_-_ Level -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-")]
    public int Score;
    public int Max_TopPos_Score,
               Max_ObstacleSpeed_Score,
               Max_SpawnSpeed_Score,
               Max_Rotation_Score;

    [Header("_-_-_-_-_ Events -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-")]
    public UnityEvent On_ScoreIncrease;
    public UnityEvent On_Death,
                      On_StartLvL;

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject); // Prevent duplicates

        sfx = GetComponent<AudioSource>();

        cm = Coins_Manager.Instance;
        UIm = UI_Manager.Instance;
    }

    private void Start()
    {
        UIm = UI_Manager.Instance;

        Time.timeScale = 0;

        Change_Bird(PlayerPrefs.GetInt(Selected_Bird_key, 0));

        Lock_System();

        Score = -1;
    }

    private void Update()
    {
        if (bird != null)
        {
            if (bird.IsDead
                && IsLevelStarted)
            {
                GameOver();
            }
        }

        if (IsLevelStarted)
        {
            if (Score > PlayerPrefs.GetInt(Highest_Score_key, 0))
            {
                PlayerPrefs.SetInt(Highest_Score_key, Score);
                PlayerPrefs.Save();
            }
        }
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    public void Play()
    {
        bird = GameObject.FindGameObjectWithTag("bird").GetComponent<Bird>();

        IsLevelStarted = true;

        On_StartLvL?.Invoke();

        UIm.When_LVL_isStarted();

        UIm.TouchToStart_txt_localscale();

        Time.timeScale = 1;
    }

    public void GameOver()
    {
        PlayerPrefs.SetInt(Last_Score_key, Score);
        PlayerPrefs.Save();

        UIm.FadeInImage();
    }

    public void Pause_UnPause()
    {
        IsPaused = !IsPaused;

        if (pauseTween != null && pauseTween.IsActive())
        {
            pauseTween.Kill();
            pauseTween = null; // Prevent memory leaks
        }

        if (IsPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            pauseTween = DOTween.To(() => Time.timeScale,
                x => Time.timeScale = x, 1, 3)
                    .SetEase(Ease.OutSine).OnComplete(() => Time.timeScale = 1f).SetUpdate(true);
        }
    }

    public void playSound(AudioClip clip)
    {
        if (!sfx.isPlaying)
        {
            sfx.PlayOneShot(clip);
        }
    }

    private void Change_Bird(int ID)
    {
        GameObject SelectedBird = GameObject.FindGameObjectWithTag("bird");

        if (SelectedBird == null
            || PlayerPrefs.GetInt(Selected_Bird_key, 0) != ID)
        {
            if (changebirdTween != null && changebirdTween.IsActive())
            {
                changebirdTween.Kill();
                changebirdTween = null;
            }

            Destroy(SelectedBird);

            PlayerPrefs.SetInt(Selected_Bird_key, ID);
            PlayerPrefs.Save();

            GameObject BB = Instantiate(birds[ID].Bird_Prefab,
                        new Vector3(-Camera.main.orthographicSize * Camera.main.aspect + 2.88888f, 0),
                        transform.rotation);

            SpriteRenderer spriteRenderer = BB.GetComponent<SpriteRenderer>();

            spriteRenderer.material.SetFloat("_Step", -0.2f);

            changebirdTween = DOTween.To(() => spriteRenderer.material.GetFloat("_Step"),
                x => spriteRenderer.material.SetFloat("_Step", x), 1.2f, 1)
                .SetEase(Ease.Linear).SetUpdate(true);
        }
    }

    public void Lock_System()
    {
        for (int i = 0; i < birds.Length; i++)
        {
            UIm.Contents[i].GetComponent<Image>().sprite
                = birds[i].Bird_Prefab.GetComponent<SpriteRenderer>().sprite;


            if (i < PlayerPrefs.GetInt(Unlocked_Birds_num_key, 1))
            {
                UIm.Contents[i].GetComponent<Image>().color = Color.white;

                UIm.Contents[i].GetComponent<Button>().onClick.RemoveAllListeners();
                int ii = i;
                UIm.Contents[i].GetComponent<Button>().onClick.AddListener(() => Change_Bird(ii));
            }else
                UIm.Contents[i].GetComponent<Image>().color = Color.black;


            if (i == PlayerPrefs.GetInt(Unlocked_Birds_num_key, 1))
            {
                UIm.Contents[i].Find("txt").gameObject.SetActive(true);
                UIm.Contents[i].Find("txt").GetComponent<TextMeshProUGUI>().text = birds[i].Value_Coins.ToString();

                if (Coins_Manager.Instance.Coins >= birds[i].Value_Coins)
                    UIm.Contents[i].Find("txt").GetComponent<TextMeshProUGUI>().color = Color.white;
                else
                    UIm.Contents[i].Find("txt").GetComponent<TextMeshProUGUI>().color = Color.red;

                UIm.Contents[i].GetComponent<Button>().onClick.RemoveAllListeners();
                int ii = i;
                UIm.Contents[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (Coins_Manager.Instance.Coins >= birds[ii].Value_Coins)
                    {
                        PlayerPrefs.SetInt(Unlocked_Birds_num_key,
                                           PlayerPrefs.GetInt(Unlocked_Birds_num_key, 1) + 1);
                        PlayerPrefs.Save();

                        Coins_Manager.Instance.Subtract_Coins(birds[ii].Value_Coins);

                        Lock_System();
                    }
                    else
                    {
                        Animate_lock_img_OR_txt(ii,
                                            UIm.Contents[ii].Find("txt"));
                        Debug.Log("No Enough Money");
                    }
                });
            }
            else
                UIm.Contents[i].Find("txt").gameObject.SetActive(false);


            if (i <= PlayerPrefs.GetInt(Unlocked_Birds_num_key, 1))
                UIm.Contents[i].Find("lock_img").gameObject.SetActive(false);
            else
            {
                UIm.Contents[i].Find("lock_img").gameObject.SetActive(true);

                UIm.Contents[i].GetComponent<Button>().onClick.RemoveAllListeners();
                int ii = i;
                UIm.Contents[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    Animate_lock_img_OR_txt(ii,
                                            UIm.Contents[ii].Find("lock_img"));
                });
            }
        }


        void Animate_lock_img_OR_txt(int ID, Transform Rtransform)
        {
            float Duration = 0.8f;
            
            Rtransform.DOScale(1.2f,
                               Duration / 2)
                .SetEase(Ease.OutQuint)
                .OnComplete(() =>
                    Rtransform.DOScale(1,
                                       Duration / 2)
                    .SetEase(Ease.InQuint)
                    .SetUpdate(true))
                .SetUpdate(true);


            int loops = 4;

            Sequence rotationSequence = DOTween.Sequence();

            rotationSequence.Append(Rtransform.DORotate(new Vector3(0, 0, -Random.Range(5f, 10f)),
                                                        Duration / (3 * loops))
                .SetEase(Ease.InOutQuad))

                .Append(Rtransform.DORotate(new Vector3(0, 0, Random.Range(5f, 10f)),
                                            Duration / (3 * loops))
                .SetEase(Ease.InOutQuad))

                .SetLoops(loops,
                          LoopType.Restart) // Loops the entire sequence (both rotations)

                .Append(Rtransform.DORotate(new Vector3(0, 0, 0),
                                            Duration / (3 * loops)))

                .SetUpdate(true);
        }
    }
}