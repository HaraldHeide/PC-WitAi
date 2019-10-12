/*
 * https://wit.ai
 * Remember to give your app permission to access microphone
 * Remember to give your app permission to access microphone
 * Remember to give your app permission to access microphone
 * Remember to give your app permission to access microphone
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public partial class WitAiSender : MonoBehaviour
{
    // Class Variables
    // Audio variables
    public AudioClip commandClip;
    //
    [SerializeField]
    private GameObject[] myPrefab;
    [SerializeField]
    //private GameObject PointCload;
    //[SerializeField]
    //private GameObject PlaneGenerator;

    int samplerate = 16000;
    // API access parameters


    // https://wit.ai

    #region TechnoWizzard
    //string url = "https://api.wit.ai/speech?v=20191006";
    //string token = "LIWIBPEWDCTA5DR6UCXAD5JZ5NAOF3MQ";  // TechnoWizzard
    #endregion TechnoWizzard

    #region ChessMoves
    string url = "https://api.wit.ai/speech?v=20191012";
    string token = "I3TRWK4IOC2APPCQEYVILCOWX6LXPL64";  // TechnoWizzard
    #endregion ChessMoves

    // GameObject to use as a default spawn point
    private bool isRecording = false;
	private bool pressedButton = false;
	public Text myResultBox;
    //public VideoPlayer vidScreen;
    //public GameObject vidCanvas;
    // Use this for 


    void Start ()
    {
        // If you are a Windows user and receiving a Tlserror
        // See: https://github.com/afauch/wit3d/issues/2
        // Uncomment the line below to bypass SSL
        // System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };

        // set samplerate to 16000 for wit.ai
        samplerate = 16000;
		//vidScreen.GetComponent<VideoPlayer> ();
	}

    public void startStopRecord()
    {
        if (isRecording == true)
        {
            pressedButton = true;
            isRecording = false;
        }
        else if (isRecording == false)
        {
            isRecording = true;
            pressedButton = true;
        }
    }


	// Update is called once per frame
	void Update ()
    {
		if (pressedButton == true)
        {
			pressedButton = false;
			if (isRecording)
            {
				myResultBox.text = "Listening for command";
				commandClip = Microphone.Start (null, false, 5, samplerate);  //Start recording (rewriting older recordings)
            }
			if (!isRecording)
            {
				myResultBox.text = null;
				myResultBox.text = "Saving Voice Request";
				// Save the audio file
				Microphone.End (null);
				if (SaveWav.Save ("sample", commandClip))  //Savewav is a static class
                {
					myResultBox.text = "Sending audio to AI...";
				}
                else
                {
					myResultBox.text = "FAILED";
				}
				// At this point, we can delete the existing audio clip
				commandClip = null;
 				//Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
				StartCoroutine(SendRequestToWitAi());
			}
		}
	}

    public IEnumerator SendRequestToWitAi()
    {
        string file = Application.persistentDataPath + "/sample.wav";
        string API_KEY = token;

        FileStream filestream = new FileStream(file, FileMode.Open, FileAccess.Read);
        BinaryReader filereader = new BinaryReader(filestream);
        byte[] postData = filereader.ReadBytes((Int32)filestream.Length);
        filestream.Close();
        filereader.Close();

        // https://stackoverflow.com/questions/46003824/sending-http-requests-in-c-sharp-with-unity

        float timeSent = Time.time;

        UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");
        unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "audio/wav");
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            myResultBox.text = unityWebRequest.error;
        }
        else
        {
            myResultBox.text = "Upload complete!";
        }

        while (!unityWebRequest.isDone)
        {
            myResultBox.text = "Thinking and deciding ...";
            yield return null;
        }

        float duration = Time.time - timeSent;

        if (unityWebRequest.error != null && unityWebRequest.error.Length > 0)
        {
            myResultBox.text = "Error: " + unityWebRequest.error + "(" + duration + " secs)";
            Debug.Log("Error: " + unityWebRequest.error + " (" + duration + " secs)");
            yield break;
        }
        Handle(unityWebRequest.downloadHandler.text);  // Sjekk other part of this partial class
    }
}