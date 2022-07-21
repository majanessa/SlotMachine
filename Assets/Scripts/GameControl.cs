using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static event Action HandlePulled = delegate { };

    [SerializeField]
    private Text prizeText;

    [SerializeField]
    private Row[] rows;

    [SerializeField]
    private Transform handle;

    [SerializeField]
    private GameObject[] lamps;

    [SerializeField]
    private AudioClip audioPullHandle, audioRotate, audioJackpot, audioWin, audioFail;

    private int prizeValue;

    private bool resultsChecked = false;
    
    // Update is called once per frame
    private void Update() 
    {
        if (!rows[0].rowStopped || !rows[1].rowStopped || !rows[2].rowStopped)
        {
            prizeValue = 0;
            prizeText.enabled = false;
            resultsChecked = false;
        }

        if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped && !resultsChecked)
        {
            CheckResults();
            prizeText.enabled = true;
            prizeText.text = "Prize: " + prizeValue;
        }            
    }

    private void OnMouseDown() 
    {
        if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped)
            StartCoroutine("PullHandle");
    }

    private IEnumerator PullHandle()
    {
        GetComponent<AudioSource>().clip = audioPullHandle;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < 15; i += 5)
        {
            handle.Rotate(0f, 0f, i);
            yield return new WaitForSeconds(0.1f);
        }

        HandlePulled();

        for (int i = 0; i < 15; i += 5)
        {
            handle.Rotate(0f, 0f, -i);
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<AudioSource>().clip = audioRotate;
        GetComponent<AudioSource>().Play();
    }

    IEnumerator TurnOnLamps()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < lamps.Length; j++)
            {
                lamps[j].SetActive(!lamps[j].activeSelf);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private void Jackpot()
    {
        StartCoroutine("TurnOnLamps");
        Result(audioJackpot);
    }

    private void Win()
    {
        Result(audioWin);
    }

    private void Fail()
    {
        Result(audioFail);
    }

    private void Result(AudioClip audio)
    {
        GetComponent<AudioSource>().clip = audio;
        GetComponent<AudioSource>().Play();
    }

    private void CheckResults()
    {
        if (rows[0].stoppedSlot == rows[1].stoppedSlot && rows[1].stoppedSlot == rows[2].stoppedSlot)
        {
            if (rows[0].stoppedSlot == "Seven")
            {
                prizeValue = 1500;
                Jackpot();
            } 
            else if (rows[0].stoppedSlot == "Cherry")
            {
                prizeValue = 3000;
                Jackpot();
            }
            else if (rows[0].stoppedSlot == "Lemon") 
            {
                prizeValue = 5000;
                Jackpot();
            }
        }

        else if (((rows[0].stoppedSlot == rows[1].stoppedSlot)
            && (rows[0].stoppedSlot == "Seven"))

            || ((rows[0].stoppedSlot == rows[2].stoppedSlot)
            && (rows[0].stoppedSlot == "Seven"))
            
            || ((rows[1].stoppedSlot == rows[2].stoppedSlot)
            && (rows[1].stoppedSlot == "Seven")))
        {
            prizeValue = 1000;
            Win();
        }    
        
        else if (((rows[0].stoppedSlot == rows[1].stoppedSlot)
            && (rows[0].stoppedSlot == "Cherry"))

            || ((rows[0].stoppedSlot == rows[2].stoppedSlot)
            && (rows[0].stoppedSlot == "Cherry"))
            
            || ((rows[1].stoppedSlot == rows[2].stoppedSlot)
            && (rows[1].stoppedSlot == "Cherry")))
        {
            prizeValue = 2000;
            Win();
        }

        else if (((rows[0].stoppedSlot == rows[1].stoppedSlot)
            && (rows[0].stoppedSlot == "Lemon"))

            || ((rows[0].stoppedSlot == rows[2].stoppedSlot)
            && (rows[0].stoppedSlot == "Lemon"))
            
            || ((rows[1].stoppedSlot == rows[2].stoppedSlot)
            && (rows[1].stoppedSlot == "Lemon")))
        {
            prizeValue = 4000;
            Win();
        } else 
            Fail();   
        
        resultsChecked = true;
    }
}
