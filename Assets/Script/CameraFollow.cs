using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("地图边界限制")]
    public float minX, maxX; // 左右边界
    public float minY, maxY; // 上下边界

    [Header("Y轴跟随设置")]
    [SerializeField] private float yFollowThreshold = 2f;  // 玩家跳多高才开始跟Y
    private float lockedY;                                  // 锁定的Y值
    private bool isFollowingY = false;                      // 当前是否跟Y

    [Header("区域设置")]
    public float platformSectionX = 10f; // X超过这个值就进入跳跃区域，开始跟Y

    void Start()
    { 
        if (target != null)
        {
            // ✅ 初始位置直接对准玩家
            Vector3 startPos = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );
            transform.position = startPos;
            lockedY = target.position.y; // ✅ 锁定玩家Y，不是相机Y
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x;
        float targetY = transform.position.y; // 默认不动Y

        // ========== Y轴逻辑 ==========
        if (target.position.x >= platformSectionX)
        {
            // 进入跳跃区域，紧跟玩家Y
            isFollowingY = true;
        }
        else
        {
            isFollowingY = false;
        }

        if (isFollowingY)
        {
            // 跳跃区域：紧跟玩家Y
            targetY = target.position.y;
        }
        else
        {
            // 普通区域：Y轴死区，玩家跳跃不跟
            float yDiff = target.position.y - transform.position.y;
            if (Mathf.Abs(yDiff) > yFollowThreshold)
            {
                // 超过阈值才跟（比如玩家落到很低的地方）
                lockedY = target.position.y;
            }
            targetY = lockedY;
        }

        // ========== 计算目标位置 ==========
        Vector3 desiredPos = new Vector3(targetX, targetY, transform.position.z);

        // ========== 平滑移动 ==========
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // ========== 地图边界限制 ==========
        smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
        smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);

        transform.position = smoothed;
    }

    // ✅ 在 Scene 视图显示边界
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);

        // 显示跳跃区域分界线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(platformSectionX, minY, 0f),
            new Vector3(platformSectionX, maxY, 0f)
        );
    }
}