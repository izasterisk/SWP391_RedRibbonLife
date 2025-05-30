-- Tạo cơ sở dữ liệu SWP391_RedRibbonLife
CREATE DATABASE SWP391_RedRibbonLife;
GO

-- Sử dụng cơ sở dữ liệu vừa tạo
USE SWP391_RedRibbonLife;
GO

-- 1. Bảng Users (Lưu thông tin cơ bản của người dùng, thêm isActive)
CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    email NVARCHAR(100) UNIQUE,
    phone_number VARCHAR(20),
    full_name NVARCHAR(100),
    date_of_birth DATE,
    gender NVARCHAR(10),
    address NVARCHAR(MAX),
    user_role NVARCHAR(50) NOT NULL,
    isActive BIT DEFAULT 1,
    CONSTRAINT chk_user_role CHECK (user_role IN ('Customer', 'Staff', 'Doctor', 'Manager', 'Admin'))
);

-- 2. Bảng Patients (Lưu thông tin bệnh nhân)
CREATE TABLE Patients (
    patient_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    blood_type VARCHAR(5),
    is_pregnant BIT DEFAULT 0,
    special_notes NVARCHAR(MAX)
);

-- 3. Bảng Doctors (Lưu thông tin bác sĩ)
CREATE TABLE Doctors (
    doctor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    doctor_image NVARCHAR(MAX),
    bio NVARCHAR(MAX)
);

-- 4. Bảng DoctorCertificates (Bằng cấp chuyên môn của bác sĩ)
CREATE TABLE DoctorCertificates (
    certificate_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT,
    certificate_name NVARCHAR(100),
    issued_by NVARCHAR(100),
    issue_date DATE,
    expiry_date DATE,
    certificate_image NVARCHAR(MAX)
);

-- 5. Bảng Category (Phân loại bài viết)
CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(100) NOT NULL,
    isActive BIT DEFAULT 1
);

-- 6. Bảng Articles (Lưu tất cả các bài viết trên trang)
CREATE TABLE Articles (
    article_id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(200) NOT NULL,
    content NVARCHAR(MAX),
    thumbnail_image NVARCHAR(MAX),
    category_id INT,
    isActive BIT DEFAULT 1
);

-- 7. Bảng Appointments (Lưu thông tin lịch hẹn)
CREATE TABLE Appointments (
    appointment_id INT PRIMARY KEY IDENTITY(1,1),
    patient_id INT,
    doctor_id INT NOT NULL,
    appointment_date DATE NOT NULL,
    appointment_time TIME NOT NULL,
    appointment_type NVARCHAR(50) DEFAULT 'Appointment',
    status NVARCHAR(50) DEFAULT 'Scheduled',
    isAnonymous BIT DEFAULT 0,
    CONSTRAINT chk_appointment_type CHECK (appointment_type IN ('Appointment', 'Medication')),
    CONSTRAINT chk_appointment_status CHECK (status IN ('Scheduled', 'Confirmed', 'Completed', 'Cancelled'))
);

-- 8. Bảng Reminders (Lưu thông tin nhắc nhở lịch hẹn)
CREATE TABLE Reminders (
    reminder_id INT PRIMARY KEY IDENTITY(1,1),
    appointment_id INT,
    reminder_time DATETIME NOT NULL,
    reminder_type NVARCHAR(50) DEFAULT 'Appointment',
    status NVARCHAR(50) DEFAULT 'Pending',
    sent_at DATETIME NULL,
    CONSTRAINT chk_reminder_type CHECK (reminder_type IN ('Appointment', 'Medication')),
    CONSTRAINT chk_reminder_status CHECK (status IN ('Pending', 'Sent', 'Failed'))
);

-- 9. Bảng ARVRegimens (Các phác đồ điều trị ARV có sẵn)
CREATE TABLE ARVRegimens (
    regimen_id INT PRIMARY KEY IDENTITY(1,1),
    regimen_name NVARCHAR(100) NOT NULL,
    regimen_code NVARCHAR(20) UNIQUE,
    components NVARCHAR(MAX) NOT NULL,
    description NVARCHAR(MAX),
    suitable_for NVARCHAR(MAX),
    side_effects NVARCHAR(MAX),
    usage_instructions NVARCHAR(MAX),
    isActive BIT DEFAULT 1
);

-- 10. Bảng TestResults (Lưu trữ kết quả xét nghiệm của bệnh nhân)
CREATE TABLE TestResults (
    test_result_id INT PRIMARY KEY IDENTITY(1,1),
    appointment_id INT,
    patient_id INT NOT NULL,
    doctor_id INT,
    test_type NVARCHAR(100) NOT NULL,
    result_value NVARCHAR(255),
    unit NVARCHAR(50) DEFAULT 'N/A',
    normal_range NVARCHAR(50),
    notes NVARCHAR(MAX),
    CONSTRAINT chk_unit CHECK (unit IN ('cells/mm³', 'copies/mL', 'mg/dL', 'g/L', 'IU/L', '%', 'mmHg', 'N/A'))
);

-- 11. Bảng DoctorSchedules (Lưu lịch làm việc của bác sĩ, work_date đổi thành work_day ENUM)
CREATE TABLE DoctorSchedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT,
    work_day NVARCHAR(50) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    CONSTRAINT chk_work_day CHECK (work_day IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'))
);

-- 12. Bảng TreatmentHistories (Lưu lịch sử phác đồ điều trị của bệnh nhân cùng đơn thuốc đi kèm nếu có)
CREATE TABLE TreatmentHistories (
    treatment_id INT PRIMARY KEY IDENTITY(1,1),
    prescription_id INT,
    patient_id INT,
    doctor_id INT,
    start_date DATE NOT NULL,
    end_date DATE,
    status NVARCHAR(50) DEFAULT 'Active',
    notes NVARCHAR(MAX),
    CONSTRAINT chk_treatment_status CHECK (status IN ('Active', 'Stopped', 'Paused'))
);

-- 13. Bảng Prescriptions (Lưu chi tiết đơn thuốc trong phác đồ)
CREATE TABLE Prescriptions (
    prescription_id INT PRIMARY KEY IDENTITY(1,1),
    treatment_id INT,
    regimen_id INT NOT NULL
);

-- 14. Bảng này sử dụng để báo người dùng uống thuốc
CREATE TABLE MedicationSchedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    prescription_id INT,
    patient_id INT,
    medication_time INT NOT NULL,
    sent_at DATETIME NULL
);

-- FOREIGN KEY
-- Adding foreign keys to Patients table
ALTER TABLE Patients
ADD CONSTRAINT fk_patients_users
FOREIGN KEY (user_id) REFERENCES Users(user_id);

-- Adding foreign keys to Doctors table
ALTER TABLE Doctors
ADD CONSTRAINT fk_doctors_users
FOREIGN KEY (user_id) REFERENCES Users(user_id);

-- Adding foreign keys to DoctorCertificates table
ALTER TABLE DoctorCertificates
ADD CONSTRAINT fk_certificates_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

-- Adding foreign keys to Articles table
ALTER TABLE Articles
ADD CONSTRAINT fk_articles_category
FOREIGN KEY (category_id) REFERENCES Category(category_id);

-- Adding foreign keys to Appointments table
ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

-- Adding foreign keys to Reminders table
ALTER TABLE Reminders
ADD CONSTRAINT fk_reminders_appointments
FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id);

-- Adding foreign keys to TestResults table
ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_appointments
FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

-- Adding foreign keys to DoctorSchedules table
ALTER TABLE DoctorSchedules
ADD CONSTRAINT fk_schedules_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

-- Adding foreign keys to TreatmentHistories table
ALTER TABLE TreatmentHistories
ADD CONSTRAINT fk_treatment_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE TreatmentHistories
ADD CONSTRAINT fk_treatment_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

-- Adding foreign keys to Prescriptions table
ALTER TABLE Prescriptions
ADD CONSTRAINT fk_prescriptions_treatment
FOREIGN KEY (treatment_id) REFERENCES TreatmentHistories(treatment_id);

ALTER TABLE Prescriptions
ADD CONSTRAINT fk_prescriptions_regimen
FOREIGN KEY (regimen_id) REFERENCES ARVRegimens(regimen_id);

-- Adding foreign keys to MedicationSchedules table
ALTER TABLE MedicationSchedules
ADD CONSTRAINT fk_medication_prescriptions
FOREIGN KEY (prescription_id) REFERENCES Prescriptions(prescription_id);

ALTER TABLE MedicationSchedules
ADD CONSTRAINT fk_medication_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

-- Chèn dữ liệu cho vai trò Customer
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive)
VALUES
('customer1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'customer1@example.com', '0123456789', N'Nguyễn Văn A', '1990-01-01', 'Male', N'Hà Nội', 'Customer', 1),
('customer2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'customer2@example.com', '0987654321', N'Trần Thị B', '1995-05-05', 'Female', N'Hồ Chí Minh', 'Customer', 1);

-- Chèn dữ liệu cho vai trò Doctor
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive)
VALUES
('doctor1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'doctor1@example.com', '0123456780', N'Lê Văn C', '1980-02-02', 'Male', N'Đà Nẵng', 'Doctor', 1),
('doctor2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'doctor2@example.com', '0987654320', N'Phạm Thị D', '1985-06-06', 'Female', N'Hải Phòng', 'Doctor', 1);

-- Chèn dữ liệu cho vai trò Staff
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive)
VALUES
('staff1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'staff1@example.com', '0123456781', N'Nguyễn Văn E', '1992-03-03', 'Male', N'Cần Thơ', 'Staff', 1),
('staff2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'staff2@example.com', '0987654322', N'Trần Thị F', '1997-07-07', 'Female', N'Vũng Tàu', 'Staff', 1);

-- Chèn dữ liệu cho vai trò Manager
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive)
VALUES
('manager1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'manager1@example.com', '0123456782', N'Lê Văn G', '1988-04-04', 'Male', N'Nha Trang', 'Manager', 1);

-- Chèn dữ liệu cho vai trò Admin
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive)
VALUES
('admin1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'admin1@example.com', '0987654323', N'Phạm Thị H', '1990-08-08', 'Female', N'Huế', 'Admin', 1);

-- Chèn dữ liệu vào bảng Patients (cho Customer)
INSERT INTO Patients (user_id, blood_type, is_pregnant, special_notes)
VALUES
(1, 'A+', 0, N'Không có ghi chú đặc biệt'),
(2, 'B-', 1, N'Đang mang thai');

-- Chèn dữ liệu vào bảng Doctors (cho Doctor)
INSERT INTO Doctors (user_id, doctor_image, bio)
VALUES
(3, 'doctor1.jpg', N'Bác sĩ chuyên khoa HIV'),
(4, 'doctor2.jpg', N'Bác sĩ có kinh nghiệm trong điều trị HIV');