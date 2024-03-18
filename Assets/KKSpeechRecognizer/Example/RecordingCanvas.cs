using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;
using TMPro;
public class RecordingCanvas : MonoBehaviour
{
    public Button startRecordingButton;
    //public TextMeshProUGUI resultText;
    public TMP_InputField searchInput;

    void Start()
    {
        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            listener.onFinalResults.AddListener(OnFinalResult);
            listener.onPartialResults.AddListener(OnPartialResult);
            listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            //resultText.SetText("Sorry, but this device doesn't support speech recognition");
            searchInput.GetComponentInChildren<TextMeshProUGUI>().SetText("Sorry, but this device doesn't support speech recognition");
            //resultText.text = "Sorry, but this device doesn't support speech recognition";
            startRecordingButton.enabled = false;
        }


    }

    public void OnFinalResult(string result)
    {
        //startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Start Recording");
        //resultText.SetText(result);
        searchInput.text = result;
        startRecordingButton.enabled = true;
    }

    public void OnPartialResult(string result)
    {
        //resultText.text = result;
        //resultText.SetText(result);
        searchInput.text = result;
    }

    public void OnAvailabilityChange(bool available)
    {
        startRecordingButton.enabled = available;
        if (!available)
        {
            //resultText.SetText("Speech Recognition not available");
            searchInput.text = "Speech Recognition not available";
        }
        else
        {
            //resultText.SetText("Say something :-)");
            searchInput.text = "Say something :-)";
        }
    }

    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                startRecordingButton.enabled = true;
                break;
            default:
                startRecordingButton.enabled = false;
                //resultText.SetText("Cannot use Speech Recognition, authorization status is " + status);
                searchInput.text = "Cannot use Speech Recognition, authorization status is " + status;
                break;
        }
    }

    public void OnEndOfSpeech()
    {
        //startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Start Recording");
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        //startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Start Recording");
        startRecordingButton.enabled = true;
    }

    public void OnStartRecordingPressed()
    {
        if (SpeechRecognizer.IsRecording())
        {
#if UNITY_IOS && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			
			startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			
#endif
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
            //startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Stop Recording");
            //resultText.SetText("Say something :-)");
            searchInput.text = "Say something :-)";
        }
    }
}
