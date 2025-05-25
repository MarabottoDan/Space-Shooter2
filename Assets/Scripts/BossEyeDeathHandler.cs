using System.Collections;
using UnityEngine;

public class BossEyeDeathHandler : MonoBehaviour
{
    private bool _leftEyeDead = false;
    private bool _rightEyeDead = false;

    [SerializeField] private AudioClip _finalGrowlClip;
    [SerializeField] private GameObject _bossRightHand;

    [SerializeField] private float _delayBeforeShrink = 10f;
    [SerializeField] private BossLeftHand _bossLeftHand;


    private AudioSource _audioSource;
    private bool _hasStartedExit = false;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    public void OnEyeDeath(string whichEye)
    {
        if (whichEye == "Left") _leftEyeDead = true;
        if (whichEye == "Right") _rightEyeDead = true;

        if (_leftEyeDead && _rightEyeDead && !_hasStartedExit)
        {
            _hasStartedExit = true;
            StartCoroutine(BeginBossExitSequence());
        }
    }

    private IEnumerator BeginBossExitSequence()
    {
        Debug.Log("Both eyes destroyed.Beginning boss exit sequence");

        if (_finalGrowlClip != null)
        {
            _audioSource.PlayOneShot(_finalGrowlClip);
            yield return new WaitForSeconds(_finalGrowlClip.length);
        }

        if (_bossRightHand != null)
        {
            Animator anim = _bossRightHand.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Exit");
            }

            if (_bossLeftHand != null)
            {
                _bossLeftHand.StopFiring();
            }


        }   yield return new WaitForSeconds(_delayBeforeShrink);

        GetComponent<BossShrinkExit>().StartShrinkExit();
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
