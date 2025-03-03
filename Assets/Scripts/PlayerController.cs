﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public ScoreManager scoreManager;

    private int lifeCount = 0;
    public float speed;
    public bool isFlipped;
    public Sprite hold;
    Animator animator;

    private GameController gc = new GameController();    
    private Spawn sp = new Spawn();
    private int count;
    private bool carryingChild = false;
    private GameObject child;
    private SpriteRenderer spRen;
    private Sprite walk;
    private Rigidbody2D rb2d;
   

    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_GRAB = 2;
    const int STATE_GRABidle = 3;

    int _currentAnimationState = STATE_IDLE;

    public void Start() {
        isFlipped = false;
        spRen = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        walk = spRen.sprite;
    }
    
    // Movement
    public void Update() {

        if(!carryingChild) {
            //BEGIN MOVEMENT WITHOUT CHILD
            if (Input.GetKey(KeyCode.W) && !carryingChild)
            {
                changeState(STATE_WALK);
                transform.position += Vector3.up * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A) && !carryingChild)
            {
                if (!isFlipped)
                {
                    Flip();
                    isFlipped = true;
                }
                changeState(STATE_WALK);
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) && !carryingChild)
            {
                changeState(STATE_WALK);
                transform.position += Vector3.down * speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) && !carryingChild)
            {
                if (isFlipped)
                {
                    Flip();
                    isFlipped = false;
                }
                changeState(STATE_WALK);
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
        } else if(carryingChild) {
            //BEGIN MOVEMENT WITH CHILD
            if (Input.GetKey(KeyCode.W))
            {
                changeState(STATE_GRAB);
                transform.position += Vector3.up * speed * Time.deltaTime;
                child.transform.position = transform.position;
                if (!isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(.25f, -.1f);
                }
                else if (isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(-.19f, -.25f);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (!isFlipped)
                {
                    Flip();
                    isFlipped = true;
                }
                changeState(STATE_GRAB);
                transform.position += Vector3.left * speed * Time.deltaTime;    
                if (!isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(.25f, -.1f);
                }
                else if (isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(-.19f, -.25f);
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                changeState(STATE_GRAB);
                transform.position += Vector3.down * speed * Time.deltaTime;
                child.transform.position = transform.position;
                if (!isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(.25f, -.1f);
                }
                else if (isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(-.19f, -.25f);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (isFlipped)
                {
                    Flip();
                    isFlipped = false;
                }
                changeState(STATE_GRAB);
                transform.position += Vector3.right * speed * Time.deltaTime;
                if (!isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(.25f, -.1f);
                }
                else if (isFlipped)
                {
                    child.transform.position = transform.position;
                    child.transform.position += new Vector3(-.19f, -.25f);
                }
                
            }
        }
    } 

    void LateUpdate() {
        if (rb2d.velocity.magnitude == 0.0 && !carryingChild)
        {
            changeState(STATE_IDLE);
        }
        if(rb2d.velocity.magnitude == 0.0 && carryingChild)
        {
            changeState(STATE_GRABidle);
        }

    }


    
    // Changed the players animation state
    public void changeState(int state)
    {
        if (_currentAnimationState == state)
            return;
        if (!animator)
        {
            Debug.Log("asdf");

        }
        switch (state)
        {
            case STATE_WALK:
                animator.SetInteger("state", STATE_WALK);
                break;
            case STATE_IDLE:
                    animator.SetInteger("state", STATE_IDLE);
                    break;
            case STATE_GRAB:
                animator.SetInteger("state", STATE_GRAB);
                break;
            case STATE_GRABidle:
                animator.SetInteger("state", STATE_GRABidle);
                break;
        }

        _currentAnimationState = state;
    }

    //Used to determine if Harambe is in contact with a child or with his den
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Child"))
        {
            if(Input.GetKey(KeyCode.E) && !carryingChild) {
                child = other.gameObject;
                child.transform.Rotate(Vector3.forward * 180);
                child.transform.position = transform.position;
                child.transform.localScale += new Vector3(-0.05f, -.05f, 0);
                if(!isFlipped)
                {
                    child.transform.position += new Vector3(.25f, -.1f);
                } else if(isFlipped) {
                    child.transform.position += new Vector3(-.19f, -.25f);
                }
                
                carryingChild = true;
                if (count == 0) {
                    gc.startShooting();
                }
                spRen.sprite = hold;
                count++;
            }
        }
        if(other.gameObject.CompareTag("Den_door")) {
            if(carryingChild) {
                int currentKid = scoreManager.GetCurrentKids();
                scoreManager.kidCountAdd(); 
                Destroy(child);
                carryingChild = false;
                spRen.sprite = walk;
                sp.Respawn();
            }
        }
    }

    //Used to flip Harambe if another direction is pushed
    public void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


}