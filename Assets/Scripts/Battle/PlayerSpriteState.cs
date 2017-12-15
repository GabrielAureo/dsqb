using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stance{
    public Sprite armed;
    public Sprite afterThrow;
}
public class PlayerSpriteState : MonoBehaviour {
    SpriteRenderer sr;
    Player player;
    [SerializeField]
    Sprite defaultSprite;

    [SerializeField]
    Stance bowStance;
    [SerializeField]
    Stance knifeStance;
    [SerializeField]
    Stance spearStance;

    Stance currentStance;

    [SerializeField]
    int recoveryFrames;


    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        sr = GetComponent<SpriteRenderer>();

    }

    void Update(){
        if(player.charge > 0.0f){
            StartCoroutine(UpdateStance());
        }
        
        

    }

    IEnumerator UpdateStance(){
        switch(player.current_weapon){
            case Weapon.BOW:
                currentStance = bowStance;
                break;
            case Weapon.KNIFE:
                currentStance = knifeStance;
                break;
            case Weapon.SPEAR:
                currentStance = spearStance;
                break;
            default:
                break;
        }

        while(player.charge > 0.0f){
            sr.sprite = currentStance.armed;
            yield return null;
        }

        sr.sprite = currentStance.afterThrow;
        yield return HushPuppy.WaitForEndOfFrames(recoveryFrames);
        sr.sprite = defaultSprite;
    }
  




}