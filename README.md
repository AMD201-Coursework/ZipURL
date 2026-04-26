# Hướng dẫn

Sau khi `git clone` dự án về máy (clone chỉ với lần đầu lấy dự án về máy local): 

**Bước 1:** Khởi động database 

```bash
docker-compose up -d
```
**Note:** Bật docker trước khi chạy. Trong code hiện tại chỉ có phần database của ShorterURL Service. Còn phần database của Indentity đợi tk Khỏe up lên sau.

**Bước 2:** Cập nhật database (Migrations)
Mở dự án bằng Visual Studio, sau đó mở Package Manager 
- Chọn Default project là ShorterURL 
- Chạy lệnh `Update-Database`

---
- **Branch:** Không commit vào `main`. Tạo nhánh theo feature/tên-tính-năng hoặc fix/tên-lỗi, sau đó merge vào `develop` và test, sau khi ổn rồi mới push lên `main`