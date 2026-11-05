using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreComponent : MonoBehaviour
{
    [SerializeField] protected Core core;

    // Getter an toàn: Chỉ tìm Core khi thực sự cần dùng đến
    public Core Core
    {
        get
        {
            if (core == null)
            {
                // includeInactive: true giúp tìm Core kể cả khi object cha đang bị ẩn
                core = GetComponentInParent<Core>(true);

                if (core == null)
                {
                    Debug.LogError($"[CoreComponent] Khong tim thay Core o Parent cua {gameObject.name}");
                }
                else
                {
                    // Tự động đăng ký component vào Core khi tìm thấy
                    core.AddComponent(this);
                }
            }
            return core;
        }
    }

    protected virtual void Awake()
    {
        // Gọi getter Core để kích hoạt kiểm tra và đăng ký vào Core
        _ = Core;
    }

    public virtual void LogicUpdate() { }
}