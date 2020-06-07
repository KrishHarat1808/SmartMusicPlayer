using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
public class UIManager : MonoBehaviour
{
    public GameObject Play;
    public GameObject Pause;
    public GameObject Mute;
    public GameObject UnMute;
    public GameObject Prev;
    public GameObject Next;
    //public GameObject VolSlider;

    public GameObject SoundManager;

    protected DictationRecognizer dictationRecognizer;

    [System.Serializable]
    public class UnityEventString : UnityEngine.Events.UnityEvent<string> { };
    public UnityEventString OnPhraseRecognized;

    public UnityEngine.Events.UnityEvent OnUserStartedSpeaking;

    private bool isUserSpeaking;

    void Start()
    {
        Play.SetActive(true);
        Pause.SetActive(false);

        Mute.SetActive(true);
        UnMute.SetActive(false);

        Prev.SetActive(true);
        Next.SetActive(true);

        StartDictationEngine();
    }

    public void _Play()
    {
        Play.SetActive(false);
        Pause.SetActive(true);

        SoundManager.SendMessage("__Play");
    }

    public void _Pause()
    {
        Play.SetActive(true);
        Pause.SetActive(false);

        SoundManager.SendMessage("__Pause");
    }

    public void _Mute()
    {
        Mute.SetActive(false);
        UnMute.SetActive(true);

        SoundManager.SendMessage("__Mute");
    }

    public void _UnMute()
    {
        Mute.SetActive(true);
        UnMute.SetActive(false);

        SoundManager.SendMessage("__UnMute");
    }

    public void _Prev()
    {
        SoundManager.SendMessage("__Prev");
    }

    public void _Next()
    {
        SoundManager.SendMessage("__Next");
    }
    
    public void _VolAdd()
    {
        SoundManager.SendMessage("__VolAdd");
    }

    public void _VolMin()
    {
        SoundManager.SendMessage("__VolMin");
    }

    private void DictationRecognizer_OnDictationHypothesis(string text)
    {
        Debug.LogFormat("Dictation hypothesis: {0}", text);

        if (isUserSpeaking == false)
        {
            isUserSpeaking = true;
            OnUserStartedSpeaking.Invoke();
        }
    }

    private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause)
    {
        if (completionCause != DictationCompletionCause.Complete)
        {
            Debug.LogWarningFormat("Dictation completed unsuccessfully: {0}.", completionCause);


            switch (completionCause)
            {
                case DictationCompletionCause.TimeoutExceeded:
                case DictationCompletionCause.PauseLimitExceeded:
                     CloseDictationEngine();
                     StartDictationEngine();
                     break;

                case DictationCompletionCause.UnknownError:
                case DictationCompletionCause.AudioQualityFailure:
                case DictationCompletionCause.MicrophoneUnavailable:
                case DictationCompletionCause.NetworkFailure:
                     CloseDictationEngine();
                    break;

                case DictationCompletionCause.Canceled:
                case DictationCompletionCause.Complete:
                    CloseDictationEngine();
                    StartDictationEngine();
                    break;
            }
        }
    }

    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.LogFormat(text);

        if (text == "play" || text == "continue" || text == "start")
            _Play();
        else if (text == "pause" || text == "stop")
            _Pause();
        else if (text == "next" || text == "skip" || text == "forward")
            _Next();
        else if (text == "previous" || text == "back" || text == "backward")
            _Prev();
        else if (text == "mute")
            _Mute();
        else if (text == "unmute")
            _UnMute();
        else if (text == "volume up" || text == "plus" || text == "increase")
             _VolAdd();
         else if (text == "volume down" || text == "minus" || text == "decrease")
             _VolMin();
        /*else
            _Pause();*/

        if (isUserSpeaking == true)
        {
            isUserSpeaking = false;
            OnPhraseRecognized.Invoke(text);
        }

    }

    private void DictationRecognizer_OnDictationError(string error, int hresult)
    {
        Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
    }


    private void OnApplicationQuit()
    {
        CloseDictationEngine();
    }

    private void StartDictationEngine()
    {
        isUserSpeaking = false;

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;

        dictationRecognizer.Start();
    }

    private void CloseDictationEngine()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();

            dictationRecognizer.Dispose();
        }
    }
}
