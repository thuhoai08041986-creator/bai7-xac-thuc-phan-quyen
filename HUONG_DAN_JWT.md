# JWT authentication & role authorization — BIT240098

## 1. Cấu hình
Cần .NET 8, SQL Server và NuGet restore. Chạy script `Sql/01_CreateDatabase.sql` (nếu chưa có database) rồi `Sql/02_CreateUsers.sql` trong SSMS. Sửa connection string trong `appsettings.json` theo SQL Server của máy.

Tại thư mục chứa `.csproj`, mở Terminal:

```powershell
dotnet restore
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "CHANGE_THIS_TO_A_RANDOM_SECRET_AT_LEAST_32_BYTES_LONG"
dotnet user-secrets set "Jwt:Issuer" "DishManagement_BIT240098"
dotnet user-secrets set "Jwt:Audience" "DishManagement_BIT240098_Client"
dotnet user-secrets set "Seed:Secret" "CHANGE_TO_ANOTHER_LONG_RANDOM_BOOTSTRAP_SECRET"
dotnet run
```

**Quan trọng:** Thay cả hai secret ví dụ bằng chuỗi ngẫu nhiên riêng, không commit vào GitHub. User-secrets chỉ phù hợp môi trường development. Khi triển khai production dùng secret manager và HTTPS.

## 2. Tạo tài khoản mẫu một lần
Dùng PowerShell, thay `https://localhost:PORT` bằng địa chỉ hiển thị khi `dotnet run`, thay mật khẩu và secret của bạn:

```powershell
$body = @{secret="CHANGE_TO_ANOTHER_LONG_RANDOM_BOOTSTRAP_SECRET";adminPassword="AdminPassword_2026!";userPassword="UserPassword_2026!"} | ConvertTo-Json
Invoke-RestMethod -Uri "https://localhost:PORT/api/dev/seed" -Method Post -ContentType "application/json" -Body $body
```

Endpoint seed chỉ hoạt động trong môi trường Development, không ghi đè tài khoản đã tồn tại. Để đổi mật khẩu cần cập nhật hash bằng PasswordHasher.

## 3. Thử nghiệm giao diện
Mở `/AuthPage/Login`. Đăng nhập admin hoặc user với mật khẩu đã seed. Trang giữ token trong RAM (reload trang sẽ mất token) và gửi `Authorization: Bearer <JWT>` cho mỗi lần bấm nút API. Kết quả:

| Trường hợp | API | HTTP |
|---|---|---|
| Chưa đăng nhập | GET /api/auth/me | 401 |
| User hợp lệ | GET /api/auth/user | 200 |
| User hợp lệ | GET /api/auth/admin | 403 |
| Admin hợp lệ | GET /api/auth/admin | 200 |
| Mật khẩu sai | POST /api/auth/login | 401 |

Lưu ý: các endpoint JWT demo có phân quyền; các trang MVC quản lý món ăn cũ vẫn giữ hành vi hiện tại, không tự động bị khóa bằng JWT. Khi cần bảo vệ cả CRUD, phải thêm [Authorize] và thiết kế cách client gửi bearer token trong các request đó.

## 4. GitHub
Tạo repository cá nhân hoặc branch riêng, không commit `bin`, `obj`, `.vs`, file mật khẩu, token. Ví dụ:
```powershell
git init
git add .
git commit -m "Add JWT authentication and role-based authorization"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPOSITORY.git
git push -u origin main
```

## 5. Kịch bản video cá nhân (3–5 phút)
1. Giới thiệu project ASP.NET Core MVC .NET 8, SQL Server và yêu cầu JWT.
2. Mở `Program.cs`: giải thích `AddJwtBearer`, `UseAuthentication`, `UseAuthorization`, kiểm tra chữ ký/issuer/audience/expiry.
3. Mở `AuthApiController`: xác thực mật khẩu hash, tạo JWT 30 phút, claims Role; giải thích `[Authorize]`.
4. Mở giao diện đăng nhập; thử chưa đăng nhập -> 401; đăng nhập user -> 200 ở User và 403 ở Admin; đăng nhập admin -> 200 ở Admin.
5. Mở Developer Tools > Network: chỉ ra header Authorization: Bearer (không hiển thị toàn bộ token/mật khẩu trong video).
6. Mở trang quản lý món ăn cũ chứng minh tính năng giữ nguyên, cuối cùng mở repository GitHub và link commit.

## Lưu ý bảo mật
Đây là bản minh họa JWT API cho bài tập. Không lưu JWT ở localStorage để giảm rủi ro lộ token qua XSS. Trong hệ thống production, nên dùng mô hình BFF/cookie HttpOnly, CSRF protection, giới hạn đăng nhập, quản lý khóa và thu hồi phiên. Seed chỉ dùng cho máy phát triển.
