# 🎗️ SWP391_RedRibbonLife - Hệ thống Quản lý Điều trị HIV/AIDS

## 📋 Giới thiệu

**SWP391_RedRibbonLife** là một hệ thống quản lý điều trị HIV/AIDS toàn diện, được thiết kế để hỗ trợ bệnh nhân, bác sĩ và nhân viên y tế trong việc quản lý và theo dõi quá trình điều trị. Hệ thống tập trung vào việc cung cấp dịch vụ y tế chất lượng, giáo dục cộng đồng và giảm thiểu kỳ thị xã hội.

## 🎯 Mục tiêu dự án

- **Quản lý bệnh nhân**: Theo dõi thông tin y tế và lịch sử điều trị
- **Hệ thống đặt lịch**: Đặt lịch hẹn giữa bệnh nhân và bác sĩ
- **Quản lý phác đồ ARV**: Theo dõi việc sử dụng thuốc kháng virus
- **Hệ thống thông báo**: Nhắc nhở uống thuốc và lịch hẹn tự động
- **Giáo dục y tế**: Chia sẻ kiến thức và kinh nghiệm về HIV/AIDS
- **Chống kỳ thị**: Tạo môi trường hỗ trợ tích cực cho bệnh nhân

## 🏗️ Kiến trúc hệ thống

Hệ thống được xây dựng theo mô hình **N-layer Architecture**:

```
┌─────────────────────────────────────┐
│           Web API Layer             │  ← Controllers, Authentication
├─────────────────────────────────────┤
│      Business Logic Layer (BLL)     │  ← Services, DTOs, Utilities
├─────────────────────────────────────┤
│      Data Access Layer (DAL)        │  ← Models, Repositories
├─────────────────────────────────────┤
│           Database Layer            │  ← SQL Server Database
└─────────────────────────────────────┘
```

## 👥 Hệ thống vai trò người dùng

- **Patient** (Bệnh nhân HIV/AIDS): Xem thông tin cá nhân, đặt lịch hẹn, xem kết quả xét nghiệm.
- **Doctor** (Bác sĩ điều trị): Quản lý bệnh nhân, kê đơn thuốc, tạo phác đồ điều trị.
- **Staff** (Nhân viên y tế): Hỗ trợ bệnh nhân, quản lý lịch hẹn, tạo bài viết.
- **Manager** (Quản lý): Giám sát hoạt động, quản lý nhân viên.
- **Admin** (Quản trị viên): Toàn quyền quản lý hệ thống.

## 🚀 Tính năng chính

### 1. 👤 Quản lý người dùng
- **Đăng ký/Đăng nhập**: Xác thực an toàn với JWT
- **Phân quyền**: 5 vai trò người dùng khác nhau
- **Xác minh email**: Kích hoạt tài khoản qua email
- **Quản lý thông tin cá nhân**: Cập nhật profile, thay đổi mật khẩu

### 2. 🏥 Hệ thống đặt lịch hẹn
- **Đặt lịch trực tuyến**: Đặt lịch với bác sĩ theo lịch làm việc
- **Lịch hẹn ẩn danh**: Hỗ trợ bệnh nhân muốn giữ bí mật
- **Quản lý trạng thái**: Scheduled → Confirmed → Completed
- **Thông báo tự động**: Nhắc nhở qua email

### 3. 🔬 Hệ thống xét nghiệm
- **Quản lý loại xét nghiệm**: CD4, Viral Load, và các xét nghiệm khác
- **Lưu trữ kết quả**: Theo dõi tiến triển qua thời gian
- **Đơn vị đo linh hoạt**: cells/mm³, copies/mL, mg/dL, etc.
- **Liên kết với lịch hẹn**: Tự động ghi nhận kết quả

### 4. 💊 Hệ thống điều trị ARV
- **Thành phần ARV**: Quản lý các loại thuốc (TDF, 3TC, DTG, AZT, NVP, etc.)
- **Phác đồ chuẩn**: Các phác đồ được WHO khuyến nghị
- **Phác đồ tùy chỉnh**: Điều chỉnh theo tình trạng bệnh nhân
- **Theo dõi tuân thủ**: Lịch sử uống thuốc và hiệu quả

### 5. 🔔 Hệ thống thông báo thông minh
- **Nhắc nhở uống thuốc**: Thông báo hàng ngày
- **Lịch hẹn**: Thông báo trước 24h
- **Background Jobs**: Tự động với Hangfire
- **Email marketing**: SendGrid integration
- **Retry mechanism**: Đảm bảo gửi thành công

### 6. 📚 Hệ thống giáo dục và hỗ trợ
- **Bài viết giáo dục**: Kiến thức về HIV/AIDS
- **Chia sẻ kinh nghiệm**: Câu chuyện từ bệnh nhân
- **Chống kỳ thị**: Nâng cao nhận thức cộng đồng
- **Phân loại nội dung**: About Us, HIV Education, Stigma Reduction, Experience Blog

## 🗄️ Cơ sở dữ liệu

### Các bảng chính:
- **Users**: Thông tin người dùng cơ bản
- **Patients**: Thông tin y tế bệnh nhân
- **Doctors**: Thông tin bác sĩ và chứng chỉ
- **Appointments**: Lịch hẹn khám
- **TestResults**: Kết quả xét nghiệm
- **ARVRegimens**: Phác đồ điều trị ARV
- **ARVComponents**: Các thành phần thuốc ARV
- **Treatment**: Lịch sử điều trị
- **Notifications**: Hệ thống thông báo
- **Articles**: Bài viết giáo dục

### 📊 Tối ưu hóa:
- **27 indexes** được tạo để tối ưu performance
- **Foreign key constraints** đảm bảo tính toàn vẹn dữ liệu
- **Check constraints** validate dữ liệu đầu vào

## 🛠️ Công nghệ sử dụng

### Backend:
- **.NET 8.0**: Framework chính
- **ASP.NET Core Web API**: RESTful API
- **Entity Framework Core**: ORM cho database
- **SQL Server**: Hệ quản trị cơ sở dữ liệu
- **AutoMapper**: Object-to-object mapping
- **JWT Bearer**: Authentication & Authorization

### Background Services:
- **Hangfire**: Background job processing
- **SendGrid**: Email service
- **Cron Jobs**: Scheduled notifications

### Development Tools:
- **Swagger/OpenAPI**: API documentation
- **Docker**: Containerization
- **CORS**: Cross-origin resource sharing

## 📦 Cài đặt và chạy dự án

### Yêu cầu hệ thống:
- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio 2022 hoặc VS Code

### 1. Clone repository:
```bash
git clone https://github.com/your-repo/SWP391_RedRibbonLife.git
cd SWP391_RedRibbonLife
```

### 2. Cấu hình database:
```bash
# Tạo database từ script SQL
sqlcmd -S your-server -i SWP391_RedRibbonLife.sql
```

### 3. Cập nhật connection string:
Chỉnh sửa `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=SWP391_RedRibbonLife;Trusted_Connection=true"
  }
}
```

### 4. Restore packages và build:
```bash
dotnet restore
dotnet build
```

### 5. Chạy ứng dụng:
```bash
cd SWP391_RedRibbonLife
dotnet run
```

### 6. Truy cập API:
- **API**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Hangfire Dashboard**: `https://localhost:5001/hangfire` (development only)

## 🐳 Docker Deployment

### Build Docker image:
```bash
docker build -t swp391-redribbonlife .
```

### Run container:
```bash
docker run -p 8080:8080 swp391-redribbonlife
```

## 📖 API Documentation

### Authentication Endpoints:
- `POST /api/Auth/login` - Đăng nhập
- `POST /api/Auth/register` - Đăng ký
- `POST /api/Auth/forgot-password` - Quên mật khẩu

### Patient Management:
- `GET /api/Patient/GetAll` - Danh sách bệnh nhân
- `GET /api/Patient/GetByID/{id}` - Thông tin bệnh nhân
- `POST /api/Patient/Create` - Tạo bệnh nhân mới
- `PUT /api/Patient/Update` - Cập nhật thông tin

### Appointment System:
- `GET /api/Appointment/GetAll` - Danh sách lịch hẹn
- `POST /api/Appointment/Create` - Đặt lịch hẹn
- `PUT /api/Appointment/Update` - Cập nhật lịch hẹn

### Treatment Management:
- `GET /api/ARVRegimens/GetAll` - Danh sách phác đồ ARV
- `POST /api/Treatment/Create` - Tạo phác đồ điều trị
- `GET /api/TestResult/GetAll` - Kết quả xét nghiệm

## 🔐 Bảo mật

- **JWT Authentication**: Tokens có thời hạn
- **Role-based Authorization**: Phân quyền theo vai trò
- **Password Hashing**: Mã hóa mật khẩu an toàn
- **Email Verification**: Xác minh tài khoản
- **Input Validation**: Kiểm tra dữ liệu đầu vào
- **CORS Configuration**: Kiểm soát cross-origin requests

## 📈 Tính năng nâng cao

### Notification System:
- **Scheduled Jobs**: Chạy vào 6:30 AM và 8:00 PM hàng ngày
- **Smart Retry**: Tự động thử lại khi gửi thất bại
- **Multiple Channels**: Email, trong tương lai có thể mở rộng SMS

### Data Analytics:
- **Performance Indexes**: Tối ưu truy vấn database
- **Audit Trail**: Theo dõi lịch sử thay đổi
- **Health Monitoring**: Theo dõi tình trạng hệ thống

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Tạo Pull Request

## 📞 Liên hệ

- **Project Team**: SWP391 Development Team
- **Email**: support@redribbonlife.com
- **Website**: [https://redribbonlife.com](https://redribbonlife.com)

## 📄 License

Dự án này được phát triển cho mục đích giáo dục trong khóa học SWP391.

---

*Được phát triển với ❤️ bởi SWP391 Team - Vì một cộng đồng không kỳ thị HIV/AIDS*