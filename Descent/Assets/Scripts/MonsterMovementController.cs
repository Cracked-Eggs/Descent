using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterMovementController : MonoBehaviour
{
    public RunnerController runner;
    public Transform spider;
    public Animator animator;
    public MonsterIKController controller;

    public float stopChaseDistance;
    public float stopRunningDistance;

    public float walkSpeed;
    public float runnerWalkSpeed;
    public float walkStepThreshold;
    public float walkStepSpeed;

    public float runSpeed;
    public float runnerRunSpeed;
    public float runStepThreshold;
    public float runStepSpeed;

    private float m_currentMoveSpeed;

    public void Move(Vector3 target) => runner.SetTarget(target);
    public void Run() => SetRunning(true);
    public void Walk() => SetRunning(false);
    public bool ReachedTarget() => runner.ReachedTarget();

    private void SetRunning(bool isRunning)
    {
        m_currentMoveSpeed = isRunning ? runSpeed : walkSpeed;
        controller.stepSpeed = isRunning ? runStepSpeed : walkStepSpeed;
        controller.stepTreshold = isRunning ? runStepThreshold : walkStepThreshold;
        runner.SetSpeed(isRunning ? runnerRunSpeed : runnerWalkSpeed);

        animator.SetBool("isRunning", isRunning);
    }

    private void Start()
    {
        Walk();
    }

    private void Update()
    {
        Vector3 toRunner = runner.transform.position - spider.position;

        if (toRunner.magnitude > stopChaseDistance)
            spider.transform.position += toRunner.normalized * m_currentMoveSpeed * Time.deltaTime;
    }
}
