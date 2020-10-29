﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float m_primaryFieldOfViewDistance = 10f;
    public float m_secondaryFieldOfViewDistance = 30f;

    public float m_totalViewAngleInDegree = 40f;

    public FieldOfView m_primaryFieldOfView;
    public FieldOfView m_secondaryFieldOfView;

    public float m_totalRotationAngle = 40f;

    public float m_tweenDuration = 5f;

    private float m_currentRotationAngle;

    private CoverSystem m_coverSystem;

    private CoverMeshDrawer m_coverMeshDrawer;
    // Start is called before the first frame update
    private void Awake()
    {
        m_coverSystem = GetComponent<CoverSystem>();
        m_coverMeshDrawer = GetComponent<CoverMeshDrawer>();
    }

    void Start()
    {
        m_currentRotationAngle = -m_totalRotationAngle * .5f;
        DOTween.To(() => m_currentRotationAngle, x => m_currentRotationAngle = x, m_totalRotationAngle * .5f,
                m_tweenDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        m_primaryFieldOfView.DrawFieldOfView(m_totalViewAngleInDegree,m_primaryFieldOfViewDistance, m_currentRotationAngle);
        m_secondaryFieldOfView.DrawFieldOfView(m_totalViewAngleInDegree,m_secondaryFieldOfViewDistance,m_currentRotationAngle);

        var fullCoverMeshData = m_coverSystem.generateMaskMeshData(MaskMeshData.CoverType.FULL, m_totalViewAngleInDegree,
            m_secondaryFieldOfViewDistance, m_currentRotationAngle);
        
        var semiCoverMeshData = m_coverSystem.generateMaskMeshData(MaskMeshData.CoverType.SEMI, 
        m_totalViewAngleInDegree,
            m_secondaryFieldOfViewDistance, m_currentRotationAngle);


        var coverData = new List<MaskMeshData>();
        coverData.AddRange(semiCoverMeshData);
        coverData.AddRange(fullCoverMeshData);
        
        
        m_coverMeshDrawer.DrawMesh(coverData);
    }
}
