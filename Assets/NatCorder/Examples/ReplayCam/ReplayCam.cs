/* 
*   NatCorder
*   Copyright (c) 2020 Yusuf Olokoba
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using System.Collections;
    using Recorders;
    using Recorders.Clocks;
    using Recorders.Inputs;
    using UnityEngine.Video;
    using UnityEngine.UI;
    using System;
    using UnityEngine.Networking;
    using System.Collections.Generic;
    using System.IO;

    public class ReplayCam : MonoBehaviour {

        [Header("Recording")]
        public int videoWidth = 1280;
        public int videoHeight = 720;
        public bool recordMicrophone;

        private IMediaRecorder recorder;
        private CameraInput cameraInput;
        private AudioInput audioInput;
        private AudioSource microphoneSource;
        public AudioListener audioListener;
        public Camera cam;


        public VideoPlayer videoPlayer; //zcp add
        public GameObject RecodeBtnPanel;
        public GameObject RecodeBtn;
        public RenderTexture Rt;

        private IEnumerator Start () {
            // Start microphone
            videoPlayer.gameObject.SetActive(false); //zcp add
            RecodeBtnPanel.SetActive(false); //zcp add
            microphoneSource = gameObject.AddComponent<AudioSource>();
            // microphoneSource.mute = //zcp delete
            microphoneSource.loop = true;
            microphoneSource.bypassEffects =
            microphoneSource.bypassListenerEffects = false;
            if(recordMicrophone)
                microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
            microphoneSource.Play();
        }

        private void OnDestroy () {
            // Stop microphone
            if (recordMicrophone)
            {
                microphoneSource.Stop();
                Microphone.End(null);

            }
               
        }

        public void StartRecording () {
            // Start recording
            var frameRate = 30;
            //var sampleRate = recordMicrophone ? AudioSettings.outputSampleRate : 0;
            //var channelCount = recordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var sampleRate =  AudioSettings.outputSampleRate;
            var channelCount = (int)AudioSettings.speakerMode ;
            var clock = new RealtimeClock();
            recorder = new MP4Recorder(videoWidth, videoHeight, frameRate, sampleRate, channelCount);
            // Create recording inputs
            cameraInput = new CameraInput(recorder, clock, cam);
            //audioInput = recordMicrophone ? new AudioInput(recorder, clock, microphoneSource, true) : null;
            audioInput = new AudioInput(recorder, clock, audioListener);
            // Unmute microphone
            //microphoneSource.mute = audioInput == null; //zcp delete
        }
        private string path;
        public async void StopRecording () {
            // Mute microphone
            // microphoneSource.mute = true;//zcp delete
            // Stop recording
            audioInput?.Dispose();
            cameraInput.Dispose();
            path = await recorder.FinishWriting();
            // Playback recording
            Debug.Log("zzzz "+$"Saved recording to: {path}");
            var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
            path = string.Format("{0}{1}", prefix, path);
            // ÒÆ¶¯¶Ë£¬Â¼ÆµÍêºó£¬²¥·ÅÂ¼ÖÆµÄÊÓÆµ
            //Handheld.PlayFullScreenMovie($"{prefix}{path}");//zcp delete
            videoPlayer.gameObject.SetActive(true); //zcp add
            videoPlayer.url = path; //$"{prefix}{path}";
            videoPlayer.source = VideoSource.Url;
            videoPlayer.Play();
            videoPlayer.isLooping = true;
            RecodeBtnPanel.SetActive(true);
            RecodeBtn.SetActive(false);//
        }


        public void RecodeCancle()
        {
            Rt.Release();
            RecodeBtn.SetActive(true);
            videoPlayer.gameObject.SetActive(false); //zcp add
            RecodeBtnPanel.SetActive(false); //zcp add
            path = null;
        }
        public void RecodeOK()
        {
            RecodeBtn.SetActive(true);
            videoPlayer.gameObject.SetActive(false); //zcp add
            RecodeBtnPanel.SetActive(false); //zcp add

            //文件写入
            string savePath = Application.persistentDataPath;
            Debug.Log("zzzz folderPath==  "+CreateFolder(Application.persistentDataPath,"/TestdataPath"));
   
            string inPath;
            if (Application.platform != RuntimePlatform.Android)
            {
                inPath = @"file://" + path;
            }
            else
                inPath = path;
        }


        public string CreateFolder(string path, string FolderName)
        {
            string FolderPath = path + FolderName;
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            return FolderPath;
        }

       
    }
}