using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("平滑设置")]
    public float xSmoothTime = 0.1f;   // X轴跟随速度
    public float ySmoothTime = 0.15f;  // Y轴跟随速度（稍慢更舒服）

    [Header("地图边界限制")]
    public float minX, maxX;
    public float minY, maxY;

    [Header("Y轴跟随设置")]
    [SerializeField] private float yFollowThreshold = 2f;
    [SerializeField] private float yOffset = 1f;          // 摄像机稍微高一点

    [Header("区域设置")]
    public float platformSectionX = 10f;

    private Vector3 velocity = Vector3.zero;
    private float lockedY;
    private float currentTargetY;

    void Start()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            target.position.y + yOffset,
            transform.position.z
        );

        lockedY = target.position.y + yOffset;
        currentTargetY = lockedY;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ========== Y轴目标计算 ==========
        float targetY;

        if (target.position.x >= platformSectionX)
        {
            // 跳跃区域：跟随玩家Y
            targetY = target.position.y + yOffset;
        }
        else
        {
            float yDiff = target.position.y - (lockedY - yOffset);

            if (Mathf.Abs(yDiff) > yFollowThreshold)
                lockedY = target.position.y + yOffset;

            targetY = lockedY;
        }

        // 用 SmoothDamp 平滑过渡 targetY，防止区域切换时突然跳动
        currentTargetY = Mathf.SmoothDamp(
            currentTargetY,
            targetY,
            ref velocity.y,
            ySmoothTime,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );

        // ========== X轴目标 ==========
        float currentTargetX = Mathf.SmoothDamp(
            transform.position.x,
            target.position.x,
            ref velocity.x,
            xSmoothTime,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );

        // ========== 边界限制 ==========
        float clampedX = Mathf.Clamp(currentTargetX, minX, maxX);
        float clampedY = Mathf.Clamp(currentTargetY, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        // 地图边界
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);

        // 跳跃区域分界线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(platformSectionX, minY, 0f),
            new Vector3(platformSectionX, maxY, 0f)
        );
    }
}
