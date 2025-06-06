using UnityEngine;
using TMPro;
using System.Collections;
using System.Globalization;
public class DialogueLogic : MonoBehaviour
{
    [SerializeField][Tooltip("Minimum distance to trigger dialogue camera focus")]
    private float _minDistance = 10f;

    private Transform _mainCamera;
    private Transform _player;
    private GameObject _text;

    private void Start()
    {
        _mainCamera = Camera.main.transform;
        _player = GameObject.FindAnyObjectByType<Player>().transform;
        _text = GetComponentInChildren<TMP_Text>(true).gameObject;
    }

    private void Update()
    {
        if (_mainCamera == null || _player == null)
        {
            return;
        }
        if (Vector3.Distance(this.transform.position, _player.transform.position) < _minDistance)
        {
            if (!_text.activeSelf)
            {
                _text.SetActive(true);
                Destroy(gameObject, 30f);
            }
        }
        else return;
        this.transform.LookAt(_mainCamera);
    }
}
