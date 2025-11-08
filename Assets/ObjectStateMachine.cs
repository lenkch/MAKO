using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class ObjectState<E> : MonoBehaviour
where E : Enum
{
    public abstract void OnStart();
    public abstract bool OnEnter(E previousState);
    public abstract void OnExit();
    public abstract E OnUpdate();
    public abstract E OnLateUpdate();
    public abstract E OnFixedUpdate();
    
    void Start() { }
    void Update() { }
    void LateUpdate() { }
    void FixedUpdate() { }
}

public abstract class ObjectStateMachine<T, E> : MonoBehaviour
where E : Enum
where T : ObjectState<E>
{
    [SerializeField, SerializeAs("Current State")] private E m_currentState;
    [SerializeField, SerializeAs("Next State")] private E m_nextState;
    [SerializeField, SerializeAs("State Pile")] protected T[] m_statePile;

    private T m_currentStateObject;

    public abstract void InitializeStates();
    void Start()
    {
        m_statePile = new T[Enum.GetValues(typeof(E)).Length];
        InitializeStates();
        foreach (var state in m_statePile)
            state.OnStart();

        ChangeState(m_nextState);
    }

    void Update()
    {
        var newState = m_currentStateObject.OnUpdate();
        if (!newState.Equals(m_currentState))
            m_nextState = newState;
    }

    void FixedUpdate()
    {
        var newState = m_currentStateObject.OnFixedUpdate();
        if (!newState.Equals(m_currentState))
            m_nextState = newState;
    }

    void LateUpdate()
    {
        var newState = m_currentStateObject.OnLateUpdate();
        if (!newState.Equals(m_currentState))
            m_nextState = newState;

        if (!m_nextState.Equals(m_currentState))
            ChangeState(m_nextState);
    }

    public void ChangeState(E nextState)
    {
        var lastState = m_currentState;
        var nextObject = m_statePile[(int) (object) nextState];
        if (nextObject.OnEnter(lastState) || m_currentState.Equals(nextState))
        {
            var lastObject = m_currentStateObject;
            if (lastObject != null)
                lastObject.OnExit();

            m_currentState = nextState;
            m_currentStateObject = nextObject;
        } else
        {
            Debug.LogWarning("Failed to enter state");
        }
    }
}
