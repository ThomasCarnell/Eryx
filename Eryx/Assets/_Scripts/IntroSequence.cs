using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IntroSequence : MonoBehaviour {

    public float hoverHeightFadeTime = 1;

    float initHoverHeight;

    float tweenHeight;


    public void IntroSequenceInit(){
        initHoverHeight = GameManager.instance.selectedCharacter.hoverHeight;
        GameManager.instance.selectedCharacter.hoverHeight = 0;
        tweenHeight = 0;

    }

    public void SetPlayerHoverOn(){
        DOTween.To(() => tweenHeight, x => tweenHeight = x, initHoverHeight, hoverHeightFadeTime)
               .OnUpdate(UpdateHover)
               .OnComplete(UpdateHover);
    }

    void UpdateHover(){
        GameManager.instance.selectedCharacter.hoverHeight = tweenHeight;
    }

    private void OnDisable()
    {
        //Da det er et scriptable object skal den sættes tilbage hvis personen går ud af spillet
        GameManager.instance.selectedCharacter.hoverHeight = initHoverHeight;
    }
}
