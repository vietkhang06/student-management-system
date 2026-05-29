Student Management System
Hệ thống quản lý học sinh theo mô hình Client-Server:
Backend: Spring Boot REST API
Frontend: WPF Desktop (.NET 8)
Database: MySQL
1. Yêu cầu môi trường
Backend
Java 21
Maven Wrapper (`mvnw`)
IntelliJ IDEA hoặc IDE tương đương
MySQL 8+
Frontend
.NET 8 SDK
Visual Studio 2022
Workload: Desktop development with .NET
Database
MySQL Workbench hoặc công cụ quản trị MySQL bất kỳ
---
2. Cấu trúc thư mục
```text
student-management-system/
├── backend/
├── frontend/
├── database/
├── docs/
└── README.md
```
---
3. Clone dự án
```bash
git clone <YOUR_REPOSITORY_URL>
cd student-management-system
```
---
4. Khởi tạo cơ sở dữ liệu
4.1 Tạo database
Tạo một database mới trong MySQL, ví dụ:
```sql
CREATE DATABASE student_management CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```
4.2 Import dữ liệu
Chạy file SQL mẫu trong thư mục `database/`:
`schema.sql` nếu có
`student_management_full_seed.sql` để nạp dữ liệu mẫu đầy đủ
Ví dụ:
```bash
mysql -u root -p student_management < database/student_management_full_seed.sql
```
4.3 Kiểm tra dữ liệu
Đảm bảo các bảng chính đã có dữ liệu:
`TAIKHOAN`
`BANQUANLY`
`GIAOVIEN`
`LOP`
`HOCSINH`
`MONHOC`
`HOCKY`
`CHITIET_DIEM`
`THAMSO`
---
5. Cấu hình Backend
Mở file:
```text
backend/src/main/resources/application.properties
```
Kiểm tra các thông số sau:
```properties
spring.datasource.url=jdbc:mysql://localhost:3306/student_management?useUnicode=true&characterEncoding=UTF-8&serverTimezone=Asia/Ho_Chi_Minh&useSSL=false&allowPublicKeyRetrieval=true
spring.datasource.username=root
spring.datasource.password=YOUR_PASSWORD
spring.jpa.hibernate.ddl-auto=validate
server.port=8080
```
Lưu ý
`ddl-auto` nên để `validate` hoặc `none` để tránh Hibernate tự sửa schema.
Nếu port `8080` bị chiếm, hãy tắt tiến trình đang dùng port đó hoặc đổi sang port khác.
---
6. Chạy Backend
Mở terminal tại thư mục `backend/` và chạy:
```bash
./mvnw spring-boot:run
```
Trên Windows có thể dùng:
```powershell
mvnw.cmd spring-boot:run
```
Backend mặc định chạy tại:
```text
http://localhost:8080
```
---
7. Cấu hình Frontend WPF
Mở solution trong thư mục `frontend/`:
```text
frontend/StudentManagement.sln
```
Kiểm tra API base URL
Nếu cần đổi địa chỉ backend, đặt biến môi trường:
```powershell
setx STUDENT_MANAGEMENT_API_BASE_URL "http://localhost:8080/"
```
Hoặc chỉnh trực tiếp trong code nếu đang chạy nội bộ.
---
8. Chạy Frontend WPF
Mở `StudentManagement.sln` bằng Visual Studio 2022, sau đó:
Chọn project `StudentManagement.Desktop` làm Startup Project
Nhấn F5 để chạy
Hoặc chạy bằng CLI:
```bash
dotnet build
dotnet run --project .\StudentManagement.Desktop
```
---
9. Tài khoản đăng nhập mẫu
Sau khi import seed data, có thể dùng các tài khoản test như:
Ban quản lý
Tên đăng nhập: `admin01`
Mật khẩu: `Admin@123`
Giáo viên
Tên đăng nhập: `gv01`
Mật khẩu: `Gv@123456`
> Nếu dữ liệu seed đã được cập nhật khác đi, hãy kiểm tra lại nội dung trong file SQL mẫu.
---
10. Luồng chạy chuẩn từ con số 0
Clone project.
Tạo database MySQL.
Import file SQL mẫu.
Cấu hình `application.properties`.
Chạy backend.
Mở `StudentManagement.sln`.
Chạy WPF frontend.
Đăng nhập bằng tài khoản test.
Kiểm tra các màn hình:
Tra cứu học sinh
Danh sách lớp
Nhập điểm
Báo cáo
Quản trị hệ thống
---
11. Các lỗi thường gặp
11.1 Backend không khởi động do port 8080
Kiểm tra tiến trình đang chiếm cổng:
```bash
netstat -ano | findstr :8080
```
Sau đó tắt PID tương ứng:
```bash
taskkill /PID <PID> /F
```
---
11.2 Không đăng nhập được
Kiểm tra:
backend đã chạy chưa
URL API trong frontend có đúng không
dữ liệu tài khoản trong MySQL có đúng không
mật khẩu có đúng seed data không
---
11.3 Lỗi schema / foreign key
Nếu Hibernate báo lỗi tự sửa bảng, hãy đảm bảo:
```properties
spring.jpa.hibernate.ddl-auto=validate
```
---
11.4 Frontend không kết nối được backend
Kiểm tra:
backend có đang chạy không
API URL có đúng không
CORS / security có chặn không
log backend có lỗi gì không
---
12. Ghi chú kiến trúc
Backend xử lý nghiệp vụ và phân quyền
WPF chỉ xử lý giao diện và gọi API
Không nên hardcode dữ liệu nghiệp vụ ở frontend
Quy định QĐ1–QĐ6 nên lấy từ backend / database
Giáo viên chỉ được truy cập lớp/môn được phân công
---
13. Phát triển tiếp
Nếu tiếp tục phát triển, nên theo thứ tự:
Hoàn thiện quản trị hệ thống
Hoàn thiện quy định hệ thống
Hoàn thiện báo cáo tổng kết
Hoàn thiện quản lý điểm chi tiết
Hoàn thiện cảnh báo xác nhận thao tác
Tối ưu phân quyền giáo viên theo lớp/môn/chức danh
---
14. Liên hệ / hỗ trợ
Nếu gặp lỗi trong quá trình chạy:
kiểm tra log backend trước
kiểm tra API response bằng Postman
kiểm tra binding / DataContext trong WPF
kiểm tra seed data và schema MySQL
