# Translation API System

Hệ thống dịch thuật đa ngôn ngữ(Phiên bản thu nhỏ của Google Translate) được xây dựng bằng .NET và Blazor WebAssembly, sử dụng Google's Gemini API.

## Chức năng chính

### Hệ thống xác thực và phân quyền
- Đăng nhập/đăng ký tài khoản
- Quản lý thông tin người dùng và phân quyền
- Hỗ trợ JWT (JSON Web Token) để bảo mật API
- Quản lý phiên làm việc và token

### Dịch thuật thông minh
- Hỗ trợ dịch đa ngôn ngữ (hơn 30 ngôn ngữ) thông qua Gemini API
- Tính năng tự động nhận diện ngôn ngữ nguồn
- Xử lý văn bản dài thông qua chia nhỏ tự động
- Bảo toàn định dạng văn bản (xuống dòng, bullet points)
- Giữ nguyên các tên riêng, thương hiệu và thuật ngữ kỹ thuật
- Xử lý lỗi thông minh và thông báo chi tiết

### Tính năng người dùng
- Giao diện dịch thuật thân thiện với người dùng
- Chức năng hoán đổi ngôn ngữ nguồn/đích
- Tính năng sao chép nhanh kết quả dịch

### Quản lý hệ thống
- Quản lý mô hình AI (thêm/sửa/xóa/kích hoạt)
- Theo dõi và quản lý người dùng
- Xử lý lỗi và logging chi tiết
- Tối ưu hóa hiệu suất hệ thống

## Kiến trúc kỹ thuật

### Backend (ASP.NET Core)
- RESTful API với chuẩn OpenAPI/Swagger
- Xử lý đồng thời và tối ưu hiệu suất
- Tích hợp với Gemini API
- Hệ thống cache và retry policy
- Entity Framework Core cho quản lý dữ liệu

### Frontend (Blazor WebAssembly)
- Giao diện người dùng phản hồi nhanh
- State management hiệu quả
- Xử lý lỗi trực quan
- Tương thích đa nền tảng

## Yêu cầu hệ thống

### Môi trường phát triển
- .NET 7.0 trở lên
- SQL Server
- Gemini API key

### Cấu hình triển khai
1. Clone repository
2. Cập nhật connection string trong `appsettings.json`
3. Cấu hình Gemini API key
4. Chạy database migrations:
   ```
   dotnet ef database update
   ```

### Chạy ứng dụng
1. Khởi động API:
   ```
   cd TranslationApi.API
   dotnet run
   ```
2. Khởi động Web UI:
   ```
   cd TranslationWeb
   dotnet run
   ```

## Cấu trúc project

```
TranslationApi/
├── TranslationApi.API/          # API endpoints và controllers
├── TranslationApi.Application/  # Business logic và services
├── TranslationApi.Domain/       # Entities và business rules
├── TranslationApi.Infrastructure/ # Data access và external services
└── TranslationWeb/              # Blazor WebAssembly UI
```

## License

MIT License