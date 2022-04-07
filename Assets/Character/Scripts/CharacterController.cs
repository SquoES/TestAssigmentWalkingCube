using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private Transform tr_Visual;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed = 5;
    
    private float _width;
    private int _linesCount;
    private int _currentLine = 0;
    private float _curBias = 0f;

    private List<Vector2> _path;
    private int _targetIndex;    
    private float _distanceTraveled = 0;
    
    private Vector2 _prevPosition;
    private Vector2 _targetPosition;
    private bool _enable = false;

    internal EventHandler OnPathEnded;

    internal void SetPath(List<Vector2> path, float roadWidth, int linesCount)
    {
        _path = path;
        _width = roadWidth;

        _linesCount = linesCount < 1 ? 1
            : linesCount % 2 == 1 ? linesCount
            : linesCount - 1;

        _currentLine = 0;
        _targetIndex = 1;
        _curBias = 0;
        _distanceTraveled = 0;
        SetTargetPosition();
        transform.position = new Vector3(_path[0].x, _path[0].y);
    }

    internal void SetEnable(bool enable)
    {
        _enable = enable;
    }

    private void SetTargetPosition()
    {
        if (_targetIndex >= _path.Count)
        {
            SetEnable(false);
            if (OnPathEnded != null)
            {
                OnPathEnded.Invoke(this, null);
            }

            return;
        }

        _targetPosition = _path[_targetIndex];
        _prevPosition = _path[_targetIndex - 1];
    }
    
    private void MoveForward()
    {
        Vector3 dir = _targetPosition - _prevPosition;
        float distance = Vector3.Distance(_targetPosition, transform.position);
        float step = distance > speed * Time.deltaTime ? speed * Time.deltaTime : distance;

        Vector3 targetDir = new Vector3(_targetPosition.x - _prevPosition.x,
            _targetPosition.y - _prevPosition.y, 0f).normalized;
        Quaternion lookRot = Quaternion.LookRotation(targetDir, transform.up);

        transform.position += dir.normalized * step;
        tr_Visual.rotation = Quaternion.RotateTowards(tr_Visual.rotation, lookRot, rotateSpeed); 

        _distanceTraveled += step;
        if (Math.Abs(_distanceTraveled - dir.magnitude) < .001f)
        {
            _targetIndex++;
            _distanceTraveled = 0;
            SetTargetPosition();
        }
    }

    private IEnumerator MoveBetweenLines()
    {
        float startBias = _curBias;
        float targetBias = _width / (float)(_linesCount - 1) * _currentLine;
        const float time = .1f;
        float timer = 0f;

        while (timer < time)
        {
            timer += Time.fixedDeltaTime;
            _curBias = Mathf.Lerp(startBias, targetBias, timer / time);
            tr_Visual.localPosition = new Vector3(_curBias, 0f, 0f);
            yield return null;
        }

        _curBias = targetBias;
        tr_Visual.localPosition = new Vector3(_curBias, 0f, 0f);
    }

    private Coroutine cor_SwitchLine;
 
    private void ChangeLine(int direction)
    {
        if (Mathf.Abs(_currentLine + direction) > (float)_linesCount * .5f) return;
        _currentLine += direction;

        if (cor_SwitchLine != null)
        {
            StopCoroutine(cor_SwitchLine);
        }
        cor_SwitchLine = StartCoroutine(MoveBetweenLines());
        SetTargetPosition();
    }
    
    private void Update()
    {
        if (!_enable) return;
        MoveForward();
        GetInput();
    }

    private void GetInput()
    {
        #if UNITY_EDITOR
        GetPCInput();
        #else
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GetMobileInput();
        }
        else
        {
            GetPCInput();
        }
        #endif
    }

    private void GetMobileInput()
    {
    }

    private void GetPCInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ChangeLine(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ChangeLine(1);
        }
    }
}
