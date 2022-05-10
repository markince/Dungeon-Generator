using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   EnemyAttack.cs                                                                                                                   //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Enemy attack class                                                                                                               //
//                                                                                                                                           //
//  Notes:  plays enemy attack sounds and deals damage to player                                                                             //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class EnemyAttack : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    [SerializeField] public Transform target;
    [SerializeField] float damage = 10.0f;
    bool attackSoundPlayed;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public AudioSource audioSource;
    public AudioClip attack1;
    public AudioClip attack2;
    public AudioClip attack3;
    public AudioClip attack4;
    public AudioClip attack5;
    public AudioClip attack6;

    //***************************************************************************************************************************************//
    //	Start Function                                                                                                                       //
    //***************************************************************************************************************************************//


    void Start()
    {
        attackSoundPlayed = false;
    }

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // AttackHitEvent                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that is called when the enemy attacks the player. Attack sound is played and the players health is reduced.                  //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void AttackHitEvent()
    {
        attackSoundPlayed = false; ;


        if (target == null)
        {
            return;

        }

        target.GetComponent<PlayerHealth>().TakeDamage(damage);

    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // PlayAttackSound                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that chooses 1 of 6 attack sounds to play when an enemy attacks the player                                                   //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void PlayAttackSound()
    {
        if (!attackSoundPlayed)
        {
            int ranNum = UnityEngine.Random.Range(1, 6);

            if      (ranNum == 1) audioSource.PlayOneShot(attack1);
            else if (ranNum == 2) audioSource.PlayOneShot(attack2);
            else if (ranNum == 3) audioSource.PlayOneShot(attack3);
            else if (ranNum == 4) audioSource.PlayOneShot(attack4);
            else if (ranNum == 5) audioSource.PlayOneShot(attack5);
            else if (ranNum == 6) audioSource.PlayOneShot(attack6);

            attackSoundPlayed = true;
        }

    }

} // End of enemyAttack class
