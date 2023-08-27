using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Cube : MonoBehaviour
{
    internal CubeManager _cubeManager;
    private CubeManager.CubeRank _rank;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rb;

    internal Action<CubeManager.CubeRank, Vector3> OnMatchCube;
    internal bool isMatch = false;
    [SerializeField] TextMeshPro[] _countText;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (_cubeManager != null)
            OnMatchCube -= _cubeManager.UpgradeCube;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMatch)
            return;
        if (collision.collider.TryGetComponent(out Cube cube))
        {
            if (_rank == cube._rank)
            {
                cube.isMatch = true;
                isMatch = true;
                Vector3 spawnPos = (transform.position + cube.transform.position) / 2;
                OnMatchCube?.Invoke(_rank, spawnPos);
                PoolingSystem.Instance.DestroyAPS(gameObject);
                PoolingSystem.Instance.DestroyAPS(cube.gameObject);
            }
        }
    }

    internal void PushCube(float forceMultiply)
    {
        _rb.isKinematic = false;
        _rb.AddForce(Vector3.forward * forceMultiply, ForceMode.Impulse);
    }

    internal void Initialize(CubeManager.CubeInfo cubeInfo)
    {
        _rb.isKinematic = true;
        _rank = cubeInfo.rank;
        _meshRenderer.material.color = cubeInfo.cubeColor;

        for (int i = 0; i < _countText.Length; i++)
        {
            _countText[i].SetText(cubeInfo.value.ToString());
        }
    }
}
