# Unity Lab 3 – Physics & Colliders

Bài thực hành cuối slide **1.5 – Introduction to Physics and Colliders** (Rigidbody 2D và Collider 2D).

- Unity **6000.3.23f1** (Unity 6.3 LTS), 2D, URP
- Scene: `Assets/Scenes/SampleScene.unity`
- Prefab dùng: `Assets/Prefabs/1.5_Physics-and-Colliders/` (`Tilemap Grid`, `Ginger (Player)`)

## Các bước đã làm

### Practice 1 – Composite Collider cho nền
1. Kéo prefab **Tilemap Grid** vào scene tại (0, 0, 0).
2. Trên `Tilemap Grid/Platforms Tilemap` (tilemap nền, không phải tilemap trang trí) đã có đủ collider từ prefab, đã kiểm tra lại giá trị trên instance:
   - **Tilemap Collider 2D**: `Used By Composite = true`
   - **Rigidbody 2D**: `Body Type = Static`, `Simulated = true`
   - **Composite Collider 2D**: `Geometry Type = Outlines`, `Generation Type = Synchronous`
3. Composite Collider tạo ra 2 path: viền ngoài (4 điểm) và viền khoang chơi (10 điểm), sàn khoang ở y ≈ 4.

### Practice 2 – Physics cho Ginger (Player)
Chỉ sửa **instance trong scene**, không sửa file prefab gốc. Ginger đặt tại (-15.81, 7.33, 0).

| Component | Giá trị |
|---|---|
| **Capsule Collider 2D** | Direction = Vertical, Size = (0.5, 0.96), Offset = (0, -0.02) (chỉnh lại cho vừa sprite tự tạo; slide tham khảo (0.63, 0.92) / (-0.05, -0.04)) |
| **Rigidbody 2D** | Body Type = Dynamic, Simulated = on, Mass = 1, Gravity Scale = 1, Collision Detection = Continuous, Constraints = Freeze Rotation Z |
| Linear Damping / Angular Damping | 0 / 0.05 (mặc định; Unity 6 đổi tên từ Linear Drag / Angular Drag) |

### Phần mở rộng
- **Physics Material 2D**: `Assets/Physics/Platform.physicsMaterial2D`, gán vào Composite Collider 2D của Platforms Tilemap.
  - Thử với Bounciness = 0.8: Ginger nảy ít nhất 4 lần và vẫn còn nảy sau 4 giây.
  - Giá trị cuối cho platformer: **Friction = 0.4, Bounciness = 0**.
- **PlayerController** (`Assets/Code/PlayerController.cs`), gắn lên Ginger:
  - A/D hoặc phím mũi tên: di chuyển bằng `Rigidbody2D.linearVelocity` trong `FixedUpdate`.
  - Space: nhảy bằng `AddForce(..., ForceMode2D.Impulse)`, chỉ khi đang đứng trên mặt đất (contact có pháp tuyến hướng lên).
  - `OnCollisionEnter2D`, `OnCollisionExit2D`, `OnTriggerEnter2D` ghi `Debug.Log` để minh họa va chạm và trigger.
  - Dùng **Input System** mới (project đặt `Active Input Handling = Input System Package`), không dùng `UnityEngine.Input`.

## Asset thay thế (tự tạo)
Project không có asset gốc của bài (thư mục `Assets/Art` trống, các GUID tile/sprite mà 2 prefab tham chiếu không còn), nên **toàn bộ sprite và tile ở đây là asset tự tạo để thay thế**. Sprite là pixel art sinh bằng script cục bộ, không tải gì từ internet.

- `Assets/Art/` (Sprite Single, Point filter, Uncompressed, pivot Center):
  `ground_stone.png`, `ground_grass.png`, `background.png`, `spike.png`, `water.png`, `exit.png` (16×16, PPU 16) và `ginger.png`, `ginger_idle.png` (16×24, **PPU 24** để nhân vật cao 1 unit, khớp collider).
- `Assets/Tilemap/` (Tile asset): `ground_stone.asset`, `ground_grass.asset` (collider Grid), `spike.asset` (collider Sprite), `water.asset`, `background.asset` (không collider).
- `Assets/Physics/Platform.physicsMaterial2D` và `Assets/Code/PlayerController.cs` (xem phần mở rộng ở trên).

Cách dựng scene (chỉ sửa **instance trong scene**, prefab gốc không đổi):
- **Platforms**: giữ đúng 279 ô cũ lấy từ dữ liệu prefab nên Composite Collider không đổi (sàn y = 4, hố x -12..-9, trần y = 10). Đá cho thân, cỏ cho ô đá có khoang rỗng kín ngay phía trên.
- **Hazards**: 3 ô gai ở đáy hố (-12..-10, 2), lấy từ dữ liệu prefab.
- **Water**: prefab không có ô nào, nên hồ nước 5×2 ô ở x -9..-5, y 4..5 là vẽ mới (trang trí, không collider).
- **Background**: phủ x -24..3, y -2..14.
- **Exit**: GameObject `Exit` tại (-1.5, 4.5), có BoxCollider2D trigger (Ginger chạm vào sẽ ghi log `[Trigger] Enter: Exit`).
- **Ginger**: gán `ginger_idle`, gỡ Animator trên instance vì Animator Controller gốc bị mất.
- **Render**: mọi renderer dùng `Sprite-Lit-Default` (URP 2D). Prefab dùng sorting layer ID không còn trong project (chỉ có `Default`), nên thứ tự vẽ được cố định bằng Sorting Order: Background -10, Platforms 0, Hazards 1, Exit 2, Ginger 5, Water 10.
- **Camera**: vị trí (-8, 7, -10), orthographic size 9, nền xanh đen. Global Light 2D giữ Intensity 1.

## Kết quả kiểm thử (Play Mode)
- Ginger rơi và dừng ở y ≈ 4.505 (đáy collider y ≈ 4.005, sàn y = 4), vận tốc ≈ 0, rotation Z = 0, nhân vật không xuyên tilemap. Ảnh Game view chụp bằng camera cho thấy hang tối với đá, cỏ, gai, nước, cửa thoát và Ginger, không còn màu hồng (missing).
- Console không có error hay warning liên quan đến physics.
- Test `PlayerController` bằng phím ảo: giữ D trong 1 giây đi được ≈ 4.93 đơn vị (tốc độ 5), Space nhảy cao ≈ 2.43 đơn vị.

## Cách chạy thử
1. Mở project bằng Unity 6000.3 (LTS) và mở `Assets/Scenes/SampleScene.unity`.
2. Bấm **Play**: Ginger rơi xuống nền và đứng yên.
3. Điều khiển: **A/D** (hoặc mũi tên trái/phải) để đi, **Space** để nhảy. Xem log va chạm trong Console.
4. Muốn xem viền collider: chọn `Platforms Tilemap` trong Hierarchy, bật Gizmos ở Scene view (viền xanh lá); có thể tạm tắt Tilemap Renderer để nhìn rõ hơn.

## Ghi chú
- `Hazards Tilemap` hiện chưa có collider nên `OnTriggerEnter2D` chưa được kích hoạt. Cần thêm collider với `Is Trigger` nếu muốn thử.
- `Background Tilemap` có PolygonCollider2D nhưng đang tắt sẵn trong prefab.
- `.mcp.json` (token của Unity-MCP) đã nằm trong `.gitignore`, không đưa lên GitHub.
