using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

using Random = UnityEngine.Random;
public class CubeManager : MonoBehaviour
{
    [SerializeField] private GameObject _cubeStartOutline;
    [SerializeField] private float _clampValue;
    [SerializeField] private Transform _cubeSpawnPosition;
    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private LayerMask _cubeLayer;
    private GameObject _currentCube;

    public List<CubeInfo> cubeInfos;

    private Tween _startBubbleAnimation;

    public enum CubeRank
    {
        None,
        Cube_2,
        Cube_4,
        Cube_8,
        Cube_16,
        Cube_32,
        Cube_64,
        Cube_128,
        Cube_256,
        Cube_512,
        Cube_1024,
        Cube_2048,
    }

    [Serializable]
    public class CubeInfo
    {
        public string Name;
        public CubeRank rank;
        public Color cubeColor;
        public int value;
    }

    private void Awake()
    {
        _inputManager = _inputManager == null ? FindObjectOfType<InputManager>() : _inputManager;
    }

    void Start()
    {
        StartCoroutine(SpawnCube(0f));
        _startBubbleAnimation = _cubeStartOutline.transform.DOScale(Vector3.one * 1.3f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _currentCube)
        {
            _cubeStartOutline.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                _startBubbleAnimation.Rewind();
            });
        }

        ThrowCube();
        SwipeCurrentCube();
    }

    private void ThrowCube()
    {
        if (Input.GetMouseButtonUp(0) && _currentCube)
        {
            if (_currentCube.TryGetComponent(out Cube cube))
            {
                cube.PushCube(25f);
                _currentCube = null;
                StartCoroutine(SpawnCube(.3f));
            }
        }
    }

    private void SwipeCurrentCube()
    {
        if (_currentCube)
        {
            _currentCube.transform.position += new Vector3(_inputManager.DeltaMousePosition.x, 0, 0);
            _currentCube.transform.position = new Vector3(Mathf.Clamp(_currentCube.transform.position.x, -_clampValue, _clampValue), _currentCube.transform.position.y, _currentCube.transform.position.z);
        }
    }

    private IEnumerator SpawnCube(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject cube = PoolingSystem.Instance.InstantiateAPS("CubePrefab", _cubeSpawnPosition.position, Quaternion.identity);

        _currentCube = cube;

        if (cube.TryGetComponent(out Cube cb))
        {
            cb.isMatch = false;
            cb._cubeManager = this;
            cb.OnMatchCube += UpgradeCube;

            int rand = Random.Range(0, (cubeInfos.Count / 2));
            cb.Initialize(cubeInfos[rand]);
        }

        cube.transform.localScale = Vector3.zero;
        _startBubbleAnimation.Play();
        cube.transform.DOScale(Vector3.one * 1.4f, .15f).OnComplete(() =>
        {
            cube.transform.DOScale(Vector3.one, .15f).SetEase(Ease.OutSine);
        }).SetEase(Ease.InSine);
    }

    internal void UpgradeCube(CubeRank rank, Vector3 spawnPos)
    {
        for (int i = 0; i < cubeInfos.Count; i++)
        {
            if (cubeInfos[i].rank == rank)
            {
                if (i + 1 <= cubeInfos.Count)
                {
                    GameObject obj = PoolingSystem.Instance.InstantiateAPS("CubePrefab", spawnPos, Quaternion.identity);

                    if (obj.TryGetComponent(out Cube cube))
                    {
                        cube.isMatch = false;
                        cube._cubeManager = this;
                        cube.OnMatchCube += UpgradeCube;

                        cube.Initialize(cubeInfos[i + 1]);
                    }

                    if (obj.TryGetComponent(out Rigidbody rb))
                    {
                        rb.isKinematic = false;
                        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
                        rb.angularVelocity = new Vector3(Random.Range(6, 10f), Random.Range(6, 10f), Random.Range(6, 10f));
                    }
                }
            }
        }
    }
}
