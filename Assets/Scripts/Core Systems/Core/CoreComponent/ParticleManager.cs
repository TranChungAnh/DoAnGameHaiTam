using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : CoreComponent
{
    private Transform particlesContainer;

    protected override void Awake()
    {
        base.Awake();

        // Kiểm tra an toàn: Tìm theo Tag trước, nếu lỗi hoặc không thấy thì tìm theo tên / tự tạo mới
        GameObject container = null;

        try
        {
            container = GameObject.FindGameObjectWithTag("ParticleContainer");
        }
        catch
        {
            // Tránh crash nếu chưa tạo Tag trong Project Settings
        }

        if (container == null)
        {
            container = GameObject.Find("ParticleContainer");
        }

        if (container == null)
        {
            container = new GameObject("ParticleContainer");
        }

        particlesContainer = container.transform;
    }

    // Tạo hiệu ứng ở vị trí và góc xoay cụ thể
    public GameObject StartParticles(GameObject particlePrefab, Vector2 position, Quaternion rotation)
    {
        return Instantiate(particlePrefab, position, rotation, particlesContainer);
    }

    // Tạo hiệu ứng ngay tại vị trí object và không xoay
    public GameObject StartParticles(GameObject particlePrefab)
    {
        return StartParticles(particlePrefab, transform.position, Quaternion.identity);
    }

    // Tạo hiệu ứng ngay tại vị trí object và xoay ngẫu nhiên
    public GameObject StartPaticlesWithRandomRotation(GameObject particlePrefab)
    {
        var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        return StartParticles(particlePrefab, transform.position, randomRotation);
    }
}