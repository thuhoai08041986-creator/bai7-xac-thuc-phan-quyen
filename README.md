# Dish Management BIT240098 – ASP.NET Core MVC

## Yêu cầu
- Visual Studio 2022, .NET 8 SDK, SQL Server Express và SSMS.
- ORM: Entity Framework Core 8, SQL Server, Database First theo schema SQL đã cung cấp (mapping Fluent API).

## Cách chạy
1. Mở SSMS, kết nối `.\SQLEXPRESS03` (Windows Authentication).
2. Mở `Sql/01_CreateDatabase.sql`, nhấn Execute để tạo `BIT240098_MID` và dữ liệu mẫu. Script chỉ thêm dữ liệu nếu bảng trống.
3. Mở `DishManagement_BIT240098.csproj` trong Visual Studio.
4. Kiểm tra `appsettings.json`: `Server=.\SQLEXPRESS03` (trong JSON phải dùng `\\`).
5. Restore NuGet, Build Solution, Ctrl+F5. Trang mặc định `/Home/Index`.

## Demo
- Truy cập trang chủ bằng English mặc định, chuyển VI/EN qua thanh menu.
- Danh sách món: kết hợp tên, loại, trạng thái, min/max, sắp xếp; giữ giá trị trong form và URL khi chuyển ngôn ngữ.
- Thêm/sửa món, kiểm tra Name, Price, PreparationTime, loại tồn tại, tên không trùng trong cùng loại.
- Chi tiết món: thêm nhiều ảnh URL, chọn ảnh đại diện, xóa ảnh.
- Xóa loại đang được sử dụng -> thông báo lỗi; thử `/Dishes/Details/999999` -> 404.
- Kiểm tra tên DB, bảng và toàn bộ cột SQL có BIT240098_; tên 3 Model có MSSV.

**Lưu ý:** Đây là dự án mẫu, chưa được kiểm thử trực tiếp trên SQL Server máy người dùng. Các URL ảnh cần Internet để hiển thị. Dữ liệu tên/miêu tả món ăn không cần dịch.


## Thêm ảnh món ăn không cần SQL
- Vào **Dishes > Add dish**, điền tên, giá, thời gian, loại món.
- Trong **ImageUrl**, dán mỗi URL ảnh `https://...` trên một dòng; có thể thêm nhiều URL.
- Chọn **Thumbnail image number** (1 = ảnh thứ nhất), nhấn **Save**. Món ăn và các ảnh được lưu qua Entity Framework vào SQL Server tự động.
- Khi **Edit**, có thể thêm nhiều URL mới; ảnh mới được chọn làm ảnh đại diện.
- Trong **Details**, có thể xem tất cả ảnh, chọn **Set as thumbnail**, xóa ảnh.
- Không phải chạy INSERT SQL cho ảnh. URL phải là liên kết trực tiếp tới ảnh và có thể truy cập từ trình duyệt.
