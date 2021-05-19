using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Saea.Networking;
using Utils;

public class MoviePlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TextMeshPro debug_log;
    public TextMeshPro text_3d;
    Server server;

    DebugState debugState;

    float videolength;
    float currentVolume;
    float currentTime;
    string lastCommand;

    int FRAME_STEP;
    const int SECOND_STEP = 2;

    void Start()
    {
        videoPlayer.frame = 0;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videolength = videoPlayer.frameCount / videoPlayer.frameRate; // ×ÜÖ¡Êý/Ö¡ÂÊ
        videoPlayer.Play();
        currentTime = 0;
        currentVolume = 1.0F;
        lastCommand = "";
        debug_log.SetText("DEBUG LOG: \n not used yet.");

        FRAME_STEP = (int) Mathf.Round(videoPlayer.frameRate * SECOND_STEP);

        int maxConnection = 1;
        int bufferSize = (1024 * 1024) * 1; // 1mb
        int port = 9999;
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
        server = new Server(maxConnection, bufferSize);
        server.Init();
        server.Start(ip);
    }

    // Update is called once per frame
    void Update()
    {
        updateText();
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (videoPlayer.isPlaying)
                pause();
            else
                play();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            fastForward();
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            fastBackward();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            volumeUp();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            volumeDown();
        }
    }

    public void play()
    {
        videoPlayer.Play();
    }

    public void pause()
    {
        videoPlayer.Pause();
    }

    public void fastForward()
    {
        videoPlayer.frame += FRAME_STEP;
    }

    public void fastBackward()
    {
        videoPlayer.frame -= FRAME_STEP;
    }

    const float VOLUME_STEP = 0.05F;

    public void volumeUp()
    {
        if (videoPlayer.audioTrackCount == 1 && videoPlayer.canSetDirectAudioVolume)
        {
            currentVolume = (float) videoPlayer.GetDirectAudioVolume(0);
            currentVolume = Mathf.Min(currentVolume + VOLUME_STEP, (float)1.0);
            videoPlayer.SetDirectAudioVolume(0, currentVolume);
        }
    }

    public void volumeDown()
    {
        if (videoPlayer.audioTrackCount == 1 && videoPlayer.canSetDirectAudioVolume)
        {
            currentVolume = (float) videoPlayer.GetDirectAudioVolume(0);
            currentVolume = Mathf.Max(currentVolume - VOLUME_STEP, (float)0);
            videoPlayer.SetDirectAudioVolume(0, currentVolume);
        }
    }

    string buildText()
    {
        return string.Format("P: {0:f2}/{3:f2} \nV: {1}% \nC: {2}", currentTime, 
                                (int)Mathf.Round(currentVolume * 100), lastCommand, videolength);
    }

    int lastNum = -1;
    void updateText()
    {
        if (videoPlayer.time <= videolength)
        {
            currentTime = (float)videoPlayer.time;
            if (lastNum != server.msg_num)
            {
                Debug.Log("get new message: " + server.msg_received);
                lastCommand = server.msg_received;
                lastNum = server.msg_num;
            }
            text_3d.SetText(buildText());
        }
    }
}
