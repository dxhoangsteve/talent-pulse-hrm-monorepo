# TalentPulse HRM

Hệ thống Quản lý Nhân sự & Chấm công cho doanh nghiệp vừa và nhỏ.

---

## Giới thiệu

TalentPulse là giải pháp quản lý nhân sự (HRM) toàn diện, xây dựng trên nền tảng **monorepo** hiện đại. Dự án được phát triển với mục tiêu giải quyết các bài toán thực tế: chấm công GPS, quản lý nghỉ phép/làm thêm giờ, tính lương tự động, và phê duyệt đa cấp.

**Tech Stack**:

- **Backend**: .NET 10 Web API (Clean Architecture)
- **Frontend**: React Native với Expo
- **Database**: SQL Server

---

## Tính năng chính

### 👤 Dành cho Nhân viên

- **Chấm công GPS** - Check-in/out dựa trên vị trí thực, chống fake location.
  > **Lưu ý**: Khi check-in, nhân viên cần ở gần vị trí máy chủ (hoặc công ty) được cấu hình. Nếu ở quá xa, hệ thống sẽ báo lỗi.

- **Xem lịch sử chấm công** - Theo dõi số ngày làm, đi muộn, nghỉ phép
- **Gửi đơn nghỉ phép** - Sử dụng **Date Picker** để chọn ngày (không cần nhập tay)
- **Đăng ký làm thêm giờ** - Date & Time Picker, hệ số lương tự động (1.5x, 2x, 3x)
- **Xem phiếu lương** - Thông báo popup khi lương được chi trả
- **Khiếu nại lương** - Gửi khiếu nại nếu chưa nhận lương hoặc sai số tiền

### 👔 Dành cho Quản lý (Manager/Deputy Manager)

- **Dashboard kết hợp** - Vừa có chức năng Admin, vừa có chức năng Employee
- **Phê duyệt đơn** - Duyệt/từ chối đơn nghỉ phép và OT của nhân viên phòng ban
- **Xem chấm công phòng ban** - Theo dõi tình hình làm việc của team
- **Tự chấm công** - Đăng ký OT, xin nghỉ phép như nhân viên thường

### 🛠 Dành cho Admin/HR

- **Quản lý nhân viên** - Thêm/sửa/xóa thông tin nhân viên, phân quyền
- **Quản lý phòng ban**:
  - Thiết lập trưởng/phó phòng
  - **Thêm/xóa nhân viên** vào phòng ban với **Search Bar**
  - Mỗi nhân viên chỉ thuộc 1 phòng ban (trừ admin)
- **Tính lương** - Tự động tính lương theo công, OT, thưởng, khấu trừ
- **Chỉnh sửa lương** - Cập nhật bonus, deductions trước khi duyệt
- **Chi trả lương** - Duyệt và phát lương, nhân viên nhận thông báo
- **Xử lý khiếu nại** - Xem và phản hồi khiếu nại lương từ nhân viên

---

## Cấu trúc dự án

```
talent-pulse-hrm-monorepo/
├── BackEnd/
│   ├── BaseSource.API/        # Web API Controllers
│   ├── BaseSource.Services/   # Business Logic Layer
│   ├── BaseSource.Data/       # Entity Framework, Entities
│   ├── BaseSource.ViewModels/ # DTOs, Request/Response Models
│   └── BaseSource.Shared/     # Constants, Enums, Helpers
├── FrontEnd/
│   ├── src/
│   │   ├── screens/           # Các màn hình chính
│   │   ├── services/          # API Services (Axios)
│   │   ├── context/           # Auth Context
│   │   ├── navigation/        # React Navigation
│   │   └── constants/         # Theme, Config
│   └── package.json
└── README.md
```

---

## Cài đặt và Chạy

### Yêu cầu

- .NET 10 SDK
- Node.js 18+
- SQL Server (LocalDB hoặc remote)
- Expo CLI (`npm install -g expo-cli`)

### Backend

```bash
cd BackEnd/BaseSource.API

# Cấu hình connection string trong appsettings.json
# Chạy migration (nếu chưa có DB)
dotnet ef database update

# Chạy server
dotnet run --urls "http://0.0.0.0:5294"
```

Server sẽ chạy tại `http://localhost:5294`. Swagger UI: `http://localhost:5294/swagger`

### Frontend

```bash
cd FrontEnd

# Cài dependencies
npm install

# Cấu hình API URL trong src/constants/config.ts
# Sửa LOCAL_IP thành IP máy chạy backend

# Chạy Expo
npx expo start
```

Scan QR code bằng Expo Go app trên điện thoại hoặc nhấn `a` để mở Android emulator.

---

## API Endpoints

### Authentication

| Method | Endpoint                    | Mô tả                      |
| ------ | --------------------------- | -------------------------- |
| POST   | `/api/account/authenticate` | Đăng nhập                  |
| GET    | `/api/account/users`        | Lấy danh sách user (Admin) |

### Attendance

| Method | Endpoint                    | Mô tả             |
| ------ | --------------------------- | ----------------- |
| POST   | `/api/attendance/check-in`  | Check-in với GPS  |
| POST   | `/api/attendance/check-out` | Check-out với GPS |
| GET    | `/api/attendance/today`     | Status hôm nay    |
| GET    | `/api/attendance/history`   | Lịch sử chấm công |

### Leave Request

| Method | Endpoint                          | Mô tả                     |
| ------ | --------------------------------- | ------------------------- |
| POST   | `/api/leave-request`              | Tạo đơn nghỉ phép         |
| GET    | `/api/leave-request/my`           | Đơn của tôi               |
| POST   | `/api/leave-request/{id}/approve` | Duyệt đơn (Admin/Manager) |
| POST   | `/api/leave-request/{id}/reject`  | Từ chối đơn               |


