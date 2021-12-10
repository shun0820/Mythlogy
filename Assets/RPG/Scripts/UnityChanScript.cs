
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanScript : MonoBehaviour
{
    public enum State
    {
        Normal,
        Wait,
        Talk,
        Command
    }

    private State state;

    private UnityChanTalkScript unityChanTalkScript;

    private CharacterController characterController;
    private Animator animator;
    //　キャラクターの速度
    private Vector3 velocity;
    //　キャラクターの歩くスピード
    [SerializeField]
    private float walkSpeed = 2f;
    //　キャラクターの走るスピード
    [SerializeField]
    private float runSpeed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        state = State.Wait;
        unityChanTalkScript = GetComponent<UnityChanTalkScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (characterController.isGrounded)
                {
                    velocity = Vector3.zero;

                    var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

                    if (input.magnitude > 0.1f)
                    {
                        transform.LookAt(transform.position + input.normalized);
                        animator.SetFloat("Speed", input.magnitude);
                        if (input.magnitude > 0.5f)
                        {
                            velocity += transform.forward * runSpeed;
                        }
                        else
                        {
                            velocity += transform.forward * walkSpeed;
                        }
                    }
                    else
                    {
                        animator.SetFloat("Speed", 0f);
                    }

                    if (unityChanTalkScript.GetConversationPartner() != null && Input.GetButtonDown("Action"))
                    {
                        SetState(State.Talk);
                    }
                }
                break;
            
            case State.Wait:
                if (unityChanTalkScript.GetConversationPartner() != null && Input.GetButtonDown("Action"))
                {
                    SetState(State.Talk);
                }
                break;
            
            case State.Talk:
                velocity = Vector3.zero;
                animator.SetFloat("Speed", 0f);
                unityChanTalkScript.StartTalking();
                break;

            case State.Command:
                velocity = Vector3.zero;
                animator.SetFloat("Speed", 0f);
                break;
        }

        if (state != State.Wait)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }else {
            if (!Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f)) {
                SetState(State.Normal);
            }
        }
        
    }

    public void SetState(State state)
    {
        this.state = state;

        if (state == State.Talk)
        {
            velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f);
            unityChanTalkScript.StartTalking();
        }else if (state == State.Command)
        {
            velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f);
        }else if (state == State.Wait)
        {
            velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f);
        }
    }

    public State GetState()
    {
        return state;
    }
}
