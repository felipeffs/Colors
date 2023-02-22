using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class ButtonPlus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Action<ButtonPlus> OnEnterOver;
    public static Action<ButtonPlus> OnExitOver;

    [Header("States")]
    [SerializeField] private GameObject normal;
    [SerializeField] private GameObject highlighted;
    [SerializeField] private GameObject pressed;

    [Header("Action")]
    [SerializeField] private UnityEvent onAction;

    private State _currentState;
    private State _nextState;

    private bool isPointAbove;
    private RectTransform _rT;

    public ButtonPlus upButton;

    private void Awake()
    {
        _rT = GetComponent<RectTransform>();
    }

    private void Start()
    {
        normal.gameObject.SetActive(true);
        highlighted.gameObject.SetActive(false);
        pressed.gameObject.SetActive(false);
        UIPlusModule.Instance.AddButton(this);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (isPointAbove == false)
        {
            isPointAbove = true;
            OnEnterOver?.Invoke(this);
        }
    }

    void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (isPointAbove == true)
        {
            isPointAbove = false;
            OnExitOver?.Invoke(this);
        }
    }

    private void Update()
    {
        RunState();
    }

    private void RunState()
    {
        _nextState = _currentState switch
        {
            State.Normal => Normal(),
            State.Highlighted => Highlighted(),
            State.Pressed => Pressed(),
            _ => State.Normal
        };

        if (_nextState != _currentState)
        {
            _currentState = _nextState;
        }
    }

    private State Normal()
    {
        highlighted.gameObject.SetActive(false);
        normal.gameObject.SetActive(true);
        pressed.gameObject.SetActive(false);

        if (isPointAbove && Cursor.lockState == CursorLockMode.None) return State.Highlighted;
        return State.Normal;
    }

    private State Highlighted()
    {
        highlighted.gameObject.SetActive(true);
        normal.gameObject.SetActive(false);
        pressed.gameObject.SetActive(false);

        if (Cursor.lockState == CursorLockMode.None)
        {
            if (!isPointAbove) return State.Normal;
            if (InputManager.Instance.PointerClickPressed()) return State.Pressed;
        }
        return State.Highlighted;
    }

    private State Pressed()
    {
        highlighted.gameObject.SetActive(false);
        normal.gameObject.SetActive(false);
        pressed.gameObject.SetActive(true);

        if (Cursor.lockState == CursorLockMode.None)
        {
            if (!isPointAbove) return State.Normal;
            if (InputManager.Instance.PointerClickReleased())
            {
                onAction?.Invoke();
                return State.Normal;
            }
        }
        return State.Pressed;
    }

    public Vector2 GetCurrentRectPosition()
    {
        return _rT.position;
    }

    public void SetCurrentState(State newState)
    {
        _currentState = newState;
    }

    public enum State
    {
        Normal,
        Highlighted,
        Pressed
    }

}