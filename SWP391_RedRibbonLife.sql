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
    email NVARCHAR(100) NOT NULL UNIQUE,
    phone_number VARCHAR(20),
    full_name NVARCHAR(100),
    date_of_birth DATE,
    gender NVARCHAR(10),
    address NVARCHAR(MAX),
    user_role NVARCHAR(50) NOT NULL,
    isActive BIT DEFAULT 1 NOT NULL,
    isVerified BIT DEFAULT 0 NOT NULL,
    CONSTRAINT chk_user_role CHECK (user_role IN ('Patient', 'Staff', 'Doctor', 'Manager', 'Admin'))
);

-- 2. Bảng Patients (Lưu thông tin bệnh nhân)
CREATE TABLE Patients (
    patient_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    blood_type VARCHAR(5),
    is_pregnant BIT DEFAULT 0,
    special_notes NVARCHAR(MAX),
    createdAt DATETIME DEFAULT GETDATE() NOT NULL
);

-- 3. Bảng Doctors (Lưu thông tin bác sĩ)
CREATE TABLE Doctors (
    doctor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    bio NVARCHAR(MAX)
);

-- 4. Bảng DoctorCertificates (Bằng cấp chuyên môn của bác sĩ)
CREATE TABLE DoctorCertificates (
    certificate_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT,
    certificate_name NVARCHAR(100),
    issued_by NVARCHAR(100),
    issue_date DATE,
    expiry_date DATE
);

-- 5. Bảng Category (Phân loại bài viết)
CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(100) NOT NULL,
	isActive BIT DEFAULT 1
);

-- 6. Bảng Articles
CREATE TABLE Articles (
    article_id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(200) NOT NULL,
    content NVARCHAR(MAX) NOT NULL,
    category_id INT,
    isActive BIT DEFAULT 1,
    createdDate DATE NOT NULL DEFAULT GETDATE(),
    user_id INT
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
    test_type_id INT NULL,
    isAnonymous BIT DEFAULT 0,
    CONSTRAINT chk_appointment_type CHECK (appointment_type IN ('Appointment', 'Medication')),
    CONSTRAINT chk_appointment_status CHECK (status IN ('Scheduled', 'Confirmed', 'Completed', 'Cancelled'))
);

-- Bảng mới để lưu trữ các thành phần ARV
CREATE TABLE ARVComponents (
    component_id INT PRIMARY KEY IDENTITY(1,1),
    component_name VARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(MAX),
    isActive BIT DEFAULT 1 NOT NULL
);

-- 8. Bảng ARVRegimens (Các phác đồ điều trị ARV có sẵn và tùy chỉnh)
CREATE TABLE ARVRegimens (
    regimen_id INT PRIMARY KEY IDENTITY(1,1),
    regimen_name NVARCHAR(100),
    component1_id INT NOT NULL,
    component2_id INT NULL,
    component3_id INT NULL,
    component4_id INT NULL,
    description NVARCHAR(MAX),
    suitable_for NVARCHAR(MAX),
    side_effects NVARCHAR(MAX),
    usage_instructions NVARCHAR(MAX),
    frequency INT NOT NULL DEFAULT 1,
    isActive BIT DEFAULT 0 NOT NULL,
    isCustomized BIT DEFAULT 1 NOT NULL,
    CONSTRAINT fk_component1 FOREIGN KEY (component1_id) REFERENCES ARVComponents(component_id),
    CONSTRAINT fk_component2 FOREIGN KEY (component2_id) REFERENCES ARVComponents(component_id),
    CONSTRAINT fk_component3 FOREIGN KEY (component3_id) REFERENCES ARVComponents(component_id),
    CONSTRAINT fk_component4 FOREIGN KEY (component4_id) REFERENCES ARVComponents(component_id),
    CONSTRAINT chk_frequency CHECK (frequency IN (1, 2))
);

-- 9. Bảng TestType (Lưu trữ các loại xét nghiệm)
CREATE TABLE TestType (
    test_type_id INT PRIMARY KEY IDENTITY(1,1),
    test_type_name NVARCHAR(200) NOT NULL,
    unit NVARCHAR(50) DEFAULT 'N/A' NOT NULL,
    normal_range NVARCHAR(MAX),
    isActive BIT DEFAULT 1 NOT NULL,
    CONSTRAINT chk_unit CHECK (unit IN ('cells/mm³', 'copies/mL', 'mg/dL', 'g/L', 'IU/L', 'IU/mL', '%', 'mmHg', 'S/C', 'N/A'))
);

-- 10. Bảng TestResults (Lưu trữ kết quả xét nghiệm của bệnh nhân - quan hệ 1-1 với Appointments)
CREATE TABLE TestResults (
    test_result_id INT PRIMARY KEY IDENTITY(1,1),
    appointment_id INT NOT NULL,
    patient_id INT NOT NULL,
    doctor_id INT NOT NULL,
    test_type_id INT NOT NULL,
    result_value NVARCHAR(255),
    notes NVARCHAR(MAX)
);

-- 11. Bảng DoctorSchedules (Lưu lịch làm việc của bác sĩ)
CREATE TABLE DoctorSchedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT NOT NULL,
    work_day NVARCHAR(50) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    CONSTRAINT chk_work_day CHECK (work_day IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'))
);

-- 12. Bảng Treatment (Lưu lịch sử phác đồ điều trị và đơn thuốc của bệnh nhân)
CREATE TABLE Treatment (
    treatment_id INT PRIMARY KEY IDENTITY(1,1),
    test_result_id INT NOT NULL,
    regimen_id INT,
    start_date DATE,
    end_date DATE,
    status NVARCHAR(50) DEFAULT 'Active',
    notes NVARCHAR(MAX),
    CONSTRAINT chk_treatment_status CHECK (status IN ('Active', 'Stopped', 'Paused'))
);

-- 13. Bảng Notifications
CREATE TABLE Notifications (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    appointment_id INT NULL,
    treatment_id INT NULL,
    notification_type NVARCHAR(50) NOT NULL,
    scheduled_time DATETIME NOT NULL,
    status NVARCHAR(50) DEFAULT 'Pending',
    sent_at DATETIME NULL,
    retry_count INT DEFAULT 0,
    error_message NVARCHAR(MAX) NULL,
    CONSTRAINT chk_notification_type CHECK (notification_type IN ('Appointment', 'Medication', 'General')),
    CONSTRAINT chk_notification_status CHECK (status IN ('Pending', 'Sent', 'Failed', 'Cancelled')),
    CONSTRAINT chk_reference_id CHECK (
        (appointment_id IS NOT NULL AND treatment_id IS NULL) OR
        (appointment_id IS NULL AND treatment_id IS NOT NULL) OR
        (appointment_id IS NULL AND treatment_id IS NULL AND notification_type = 'General')
    )
);

-- FOREIGN KEY CONSTRAINTS
ALTER TABLE Patients
ADD CONSTRAINT fk_patients_users
FOREIGN KEY (user_id) REFERENCES Users(user_id);

ALTER TABLE Doctors
ADD CONSTRAINT fk_doctors_users
FOREIGN KEY (user_id) REFERENCES Users(user_id);

ALTER TABLE DoctorCertificates
ADD CONSTRAINT fk_certificates_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE Articles
ADD CONSTRAINT fk_articles_category
FOREIGN KEY (category_id) REFERENCES Category(category_id);

ALTER TABLE Articles
ADD CONSTRAINT fk_articles_user
FOREIGN KEY (user_id) REFERENCES Users(user_id);

ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_test_type
FOREIGN KEY (test_type_id) REFERENCES TestType(test_type_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_appointments
FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_test_type
FOREIGN KEY (test_type_id) REFERENCES TestType(test_type_id);

ALTER TABLE DoctorSchedules
ADD CONSTRAINT fk_schedules_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE Treatment
ADD CONSTRAINT fk_treatment_test_results
FOREIGN KEY (test_result_id) REFERENCES TestResults(test_result_id);

ALTER TABLE Treatment
ADD CONSTRAINT fk_treatment_regimen
FOREIGN KEY (regimen_id) REFERENCES ARVRegimens(regimen_id);

ALTER TABLE Notifications
ADD CONSTRAINT fk_notifications_users
FOREIGN KEY (user_id) REFERENCES Users(user_id);

ALTER TABLE Notifications
ADD CONSTRAINT fk_notifications_appointments
FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id);

ALTER TABLE Notifications
ADD CONSTRAINT fk_notifications_treatment
FOREIGN KEY (treatment_id) REFERENCES Treatment(treatment_id);

-- Users table indexes
-- Optimizes login authentication queries (LoginService.ValidateUserAsync)
CREATE NONCLUSTERED INDEX IX_Users_Username_IsActive_IsVerified 
ON Users (username, isActive, isVerified);

-- Optimizes email verification and duplicate email checks (AdminService, DoctorService, PatientService)
CREATE NONCLUSTERED INDEX IX_Users_Email_IsActive 
ON Users (email, isActive);

-- Optimizes role-based user retrieval (AdminService.GetAllAdminsAsync, DoctorService.GetAllDoctorsAsync, PatientService.GetAllActivePatientsAsync)
CREATE NONCLUSTERED INDEX IX_Users_UserRole_IsActive 
ON Users (user_role, isActive);

-- Optimizes full name searches (UserService.GetUserByFullnameAsync)
CREATE NONCLUSTERED INDEX IX_Users_FullName_IsActive 
ON Users (full_name, isActive);

-- Appointments table indexes
-- Optimizes patient appointment retrieval (AppointmentService.GetAllAppointmentsByPatientIdAsync)
CREATE NONCLUSTERED INDEX IX_Appointments_PatientId 
ON Appointments (patient_id);

-- Optimizes doctor appointment retrieval (AppointmentService.GetAllAppointmentsByDoctorIdAsync)
CREATE NONCLUSTERED INDEX IX_Appointments_DoctorId 
ON Appointments (doctor_id);

-- Optimizes appointment scheduling queries with date/time filtering
CREATE NONCLUSTERED INDEX IX_Appointments_Date_Time_Doctor 
ON Appointments (appointment_date, appointment_time, doctor_id);

-- Articles table indexes
-- Optimizes article retrieval by category (ArticleService filters)
CREATE NONCLUSTERED INDEX IX_Articles_CategoryId_IsActive 
ON Articles (category_id, isActive);

-- Optimizes article retrieval by author (ArticleService filters)
CREATE NONCLUSTERED INDEX IX_Articles_UserId_IsActive 
ON Articles (user_id, isActive);

-- Optimizes title uniqueness checks (ArticleService.CreateArticleAsync, UpdateArticleAsync)
CREATE NONCLUSTERED INDEX IX_Articles_Title 
ON Articles (title);

-- Doctors table indexes
-- Optimizes doctor-user joins (DoctorService.GetAllDoctorsAsync, GetDoctorByDoctorIDAsync)
CREATE NONCLUSTERED INDEX IX_Doctors_UserId 
ON Doctors (user_id);

-- Patients table indexes
-- Optimizes patient-user joins (PatientService.GetAllActivePatientsAsync, GetPatientByPatientIDAsync)
CREATE NONCLUSTERED INDEX IX_Patients_UserId 
ON Patients (user_id);

-- DoctorSchedules table indexes
-- Optimizes doctor schedule retrieval (DoctorScheduleService.GetDoctorScheduleByDoctorIdAsync)
CREATE NONCLUSTERED INDEX IX_DoctorSchedules_DoctorId 
ON DoctorSchedules (doctor_id);

-- Optimizes schedule conflict checking (DoctorScheduleUtils.CheckDoctorScheduleExist)
CREATE NONCLUSTERED INDEX IX_DoctorSchedules_DoctorId_WorkDay 
ON DoctorSchedules (doctor_id, work_day);

-- TestResults table indexes
-- Optimizes test result retrieval by patient (TestResultService queries)
CREATE NONCLUSTERED INDEX IX_TestResults_PatientId 
ON TestResults (patient_id);

-- Optimizes test result retrieval by doctor (TestResultService queries)
CREATE NONCLUSTERED INDEX IX_TestResults_DoctorId 
ON TestResults (doctor_id);

-- Optimizes test result retrieval by type (TestResultService queries)
CREATE NONCLUSTERED INDEX IX_TestResults_TestTypeId 
ON TestResults (test_type_id);

-- Optimizes appointment-test result relationship (1-1 relationship queries)
CREATE NONCLUSTERED INDEX IX_TestResults_AppointmentId 
ON TestResults (appointment_id);

-- Treatment table indexes
-- Optimizes treatment retrieval by regimen (TreatmentService queries)
CREATE NONCLUSTERED INDEX IX_Treatment_RegimenId 
ON Treatment (regimen_id);

-- Optimizes treatment retrieval by test result (TreatmentService queries)
CREATE NONCLUSTERED INDEX IX_Treatment_TestResultId 
ON Treatment (test_result_id);

-- Optimizes active treatment filtering (TreatmentService status-based queries)
CREATE NONCLUSTERED INDEX IX_Treatment_Status 
ON Treatment (status);

-- ARVRegimens table indexes
-- Optimizes active regimen filtering (ARVRegimensService.GetAllARVRegimensAsync, GetARVRegimensByIdAsync)
CREATE NONCLUSTERED INDEX IX_ARVRegimens_IsActive 
ON ARVRegimens (isActive);

-- Optimizes customized regimen filtering (ARVRegimensService queries)
CREATE NONCLUSTERED INDEX IX_ARVRegimens_IsCustomized_IsActive 
ON ARVRegimens (isCustomized, isActive);

-- Notifications table indexes
-- Optimizes user notification retrieval (notification services)
CREATE NONCLUSTERED INDEX IX_Notifications_UserId 
ON Notifications (user_id);

-- Optimizes notification status and scheduling queries
CREATE NONCLUSTERED INDEX IX_Notifications_Status_ScheduledTime 
ON Notifications (status, scheduled_time);

-- Optimizes notification type filtering
CREATE NONCLUSTERED INDEX IX_Notifications_NotificationType 
ON Notifications (notification_type);

-- INSERT DATA

-- Chèn dữ liệu cho vai trò Patient
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('patient1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs1@gmail.com', '0123456789', N'Nguyễn Văn A', '1990-01-01', 'Male', N'Hà Nội', 'Patient', 1, 1),
('patient2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs2@gmail.com', '0987654321', N'Trần Thị B', '1995-05-05', 'Female', N'Hồ Chí Minh', 'Patient', 1, 1),
('patient3', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs9@gmail.com', '0123456790', N'Lê Văn C', '1985-03-10', 'Male', N'Đà Nẵng', 'Patient', 1, 1),
('patient4', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs10@gmail.com', '0987654322', N'Phạm Thị D', '1992-07-15', 'Female', N'Hải Phòng', 'Patient', 1, 1),
('patient5', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs11@gmail.com', '0123456791', N'Hoàng Văn E', '1988-09-20', 'Male', N'Cần Thơ', 'Patient', 1, 1),
('patient6', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs12@gmail.com', '0987654323', N'Nguyễn Thị F', '1993-11-25', 'Female', N'Nha Trang', 'Patient', 1, 1),
('patient7', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs13@gmail.com', '0123456792', N'Trần Văn G', '1987-12-30', 'Male', N'Vũng Tàu', 'Patient', 1, 1);

-- Chèn dữ liệu cho vai trò Doctor
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('doctor1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs3@gmail.com', '0123456780', N'Lê Văn H', '1980-02-02', 'Male', N'Đà Nẵng', 'Doctor', 1, 1),
('doctor2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs4@gmail.com', '0987654320', N'Phạm Thị I', '1985-06-06', 'Female', N'Hải Phòng', 'Doctor', 1, 1);

-- Chèn dữ liệu cho vai trò Staff
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('staff1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs5@gmail.com', '0123456781', N'Nguyễn Văn J', '1992-03-03', 'Male', N'Cần Thơ', 'Staff', 1, 1),
('staff2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs6@gmail.com', '0987654322', N'Trần Thị K', '1997-07-07', 'Female', N'Vũng Tàu', 'Staff', 1, 1);

-- Chèn dữ liệu cho vai trò Manager
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('manager1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs7@gmail.com', '0123456782', N'Lê Văn L', '1988-04-04', 'Male', N'Nha Trang', 'Manager', 1, 1);

-- Chèn dữ liệu cho vai trò Admin
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('admin1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs8@gmail.com', '0987654323', N'Phạm Thị M', '1990-08-08', 'Female', N'Huế', 'Admin', 1, 1);

-- Chèn dữ liệu vào bảng Patients (cho Patient)
INSERT INTO Patients (user_id, blood_type, is_pregnant, special_notes, createdAt)
VALUES
(1, 'A+', 0, N'Không có ghi chú đặc biệt', GETDATE()),
(2, 'B-', 1, N'Đang mang thai', GETDATE()),
(3, 'O+', 0, N'Bệnh nhân mới', GETDATE()),
(4, 'AB+', 0, N'Có tiền sử dị ứng thuốc', GETDATE()),
(5, 'A-', 0, N'Không có ghi chú đặc biệt', GETDATE()),
(6, 'B+', 0, N'Bệnh nhân theo dõi định kỳ', GETDATE()),
(7, 'O-', 0, N'Cần theo dõi đặc biệt', GETDATE());

-- Chèn dữ liệu vào bảng Doctors (cho Doctor) -- ĐÃ SỬA
INSERT INTO Doctors (user_id, bio)
VALUES
(8, N'Bác sĩ chuyên khoa HIV'),
(9, N'Bác sĩ có kinh nghiệm trong điều trị HIV');

-- Chèn dữ liệu vào bảng DoctorCertificates (cho Doctor) -- ĐÃ SỬA
INSERT INTO DoctorCertificates (doctor_id, certificate_name, issued_by, issue_date, expiry_date)
VALUES
(1, N'Bằng cấp Y khoa', N'Đại học Y Hà Nội', '2005-06-15', NULL),
(1, N'Chứng chỉ chuyên khoa HIV', N'Bộ Y tế', '2010-09-20', '2025-09-20'),
(2, N'Bằng cấp Y khoa', N'Đại học Y Hồ Chí Minh', '2007-07-10', NULL),
(2, N'Chứng chỉ điều trị HIV', N'Hội Y học Việt Nam', '2012-11-15', '2027-11-15');

-- Chèn dữ liệu vào bảng Category
INSERT INTO Category (category_name)
VALUES
(N'About Us'),
(N'HIV Education'),
(N'Stigma Reduction'),
(N'Experience Blog');

-- Chèn dữ liệu vào bảng Articles
INSERT INTO Articles (title, content, category_id, isActive, createdDate, user_id)
VALUES
(N'Giới thiệu về Red Ribbon Life', N'Red Ribbon Life là tổ chức hỗ trợ bệnh nhân HIV/AIDS với sứ mệnh cung cấp dịch vụ y tế, giáo dục và giảm kỳ thị. Chúng tôi cam kết mang lại cuộc sống tốt đẹp hơn cho cộng đồng.', 1, 1, '2025-06-01', 10), -- Nguyễn Văn J (Staff)
(N'Dịch vụ y tế tại Red Ribbon Life', N'Chúng tôi cung cấp tư vấn, xét nghiệm và điều trị HIV/AIDS với đội ngũ bác sĩ chuyên môn cao và cơ sở vật chất hiện đại.', 1, 1, '2025-06-02', 11), -- Trần Thị K (Staff)
(N'Hiểu biết cơ bản về HIV/AIDS', N'HIV là virus gây suy giảm miễn dịch ở người. Bài viết này giải thích cách lây truyền, phòng ngừa và điều trị HIV.', 2, 1, '2025-06-03', 10), -- Nguyễn Văn J (Staff)
(N'Phòng ngừa HIV trong cộng đồng', N'Hướng dẫn các biện pháp phòng ngừa HIV như sử dụng bao cao su, xét nghiệm định kỳ và sử dụng PrEP.', 2, 1, '2025-06-04', 11), -- Trần Thị K (Staff)
(N'Vượt qua kỳ thị: Câu chuyện của một bệnh nhân', N'Một bệnh nhân chia sẻ hành trình sống tạp với HIV và cách họ vượt qua định kiến xã hội.', 3, 1, '2025-06-05', 10), -- Nguyễn Văn J (Staff)
(N'Tại sao cần nói không với kỳ thị HIV', N'Bài viết thảo luận về tác động của kỳ thị và cách cộng đồng có thể hỗ trợ bệnh nhân HIV.', 3, 1, '2025-06-06', 11), -- Trần Thị K (Staff)
(N'Hành trình sống chung với HIV', N'Một bệnh nhân kể về trải nghiệm cá nhân, từ khi phát hiện bệnh đến việc duy trì lối sống tích cực.', 4, 1, '2025-06-07', 10), -- Nguyễn Văn J (Staff)
(N'Kinh nghiệm hỗ trợ bệnh nhân HIV từ bác sĩ', N'Bác sĩ chia sẻ những bài học và câu chuyện từ quá trình làm việc với bệnh nhân HIV.', 4, 1, '2025-06-08', 11); -- Trần Thị K (Staff)

-- Chèn dữ liệu vào bảng ARVComponents
INSERT INTO ARVComponents (component_name, description)
VALUES
('TDF', N'Tenofovir (TDF) - Thuốc kháng retrovirus nucleot(s)ide reverse transcriptase inhibitor (NRTI).'),
('3TC', N'Lamivudine (3TC) - Thuốc kháng retrovirus nucleot(s)ide reverse transcriptase inhibitor (NRTI).'),
('DTG', N'Dolutegravir (DTG) - Thuốc kháng retrovirus integrase strand transfer inhibitor (INSTI).'),
('AZT', N'Zidovudine (AZT) - Thuốc kháng retrovirus nucleot(s)ide reverse transcriptase inhibitor (NRTI).'),
('NVP', N'Nevirapine (NVP) - Thuốc kháng retrovirus non-nucleoside reverse transcriptase inhibitor (NNRTI).'),
('ABC', N'Abacavir (ABC) - Thuốc kháng retrovirus nucleot(s)ide reverse transcriptase inhibitor (NRTI).'),
('EFV', N'Efavirenz (EFV) - Thuốc kháng retrovirus non-nucleoside reverse transcriptase inhibitor (NNRTI).'),
('RAL', N'Raltegravir (RAL) - Thuốc kháng retrovirus integrase strand transfer inhibitor (INSTI).'),
('DRV', N'Darunavir (DRV) - Thuốc kháng retrovirus protease inhibitor (PI).'),
('RTV', N'Ritonavir (RTV) - Thuốc kháng retrovirus protease inhibitor (PI), thường dùng làm chất tăng cường dược động học.');

-- Chèn dữ liệu vào bảng ARVRegimens (ví dụ)
INSERT INTO ARVRegimens (regimen_name, component1_id, component2_id, component3_id, component4_id, description, suitable_for, side_effects, usage_instructions, frequency, isActive, isCustomized)
VALUES
(N'Phác đồ TDF + 3TC + DTG Chuẩn',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'TDF'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'DTG'),
    NULL,
    N'Phác đồ điều trị HIV cho người lớn và trẻ em trên 10 tuổi, được khuyến nghị rộng rãi do hiệu quả và dung nạp tốt.',
    N'Người lớn, trẻ em trên 10 tuổi',
    N'Buồn nôn, nhức đầu, mệt mỏi nhẹ, mất ngủ (ít gặp).',
    N'Uống 1 viên/ngày vào buổi sáng, có thể uống cùng hoặc không cùng thức ăn.',
    1, 1, 0),

(N'Phác đồ AZT + 3TC + NVP Chuẩn',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'AZT'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'NVP'),
    NULL,
    N'Phác đồ điều trị HIV thường dùng cho phụ nữ mang thai để dự phòng lây truyền từ mẹ sang con.',
    N'Phụ nữ mang thai',
    N'Thiếu máu, buồn nôn, đau đầu, phát ban (cần theo dõi hội chứng quá mẫn).',
    N'AZT + 3TC: Uống 2 viên/ngày (sáng và tối). NVP: Uống tăng liều dần theo chỉ dẫn của bác sĩ.',
    2, 1, 0),

(N'Phác đồ ABC + 3TC + EFV Chuẩn',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'ABC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'EFV'),
    NULL,
    N'Phác đồ điều trị HIV cho trẻ em hoặc người lớn có chống chỉ định với TDF. Cần sàng lọc quá mẫn với Abacavir.',
    N'Trẻ em, người lớn',
    N'Phát ban, phản ứng quá mẫn (với ABC), chóng mặt, ác mộng (với EFV).',
    N'Uống 1 viên/ngày vào buổi tối để giảm tác dụng phụ thần kinh của EFV.',
    1, 1, 0),

(N'Phác đồ tùy chỉnh cho bệnh nhân A',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'TDF'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'RAL'),
    NULL,
    N'Phác đồ tùy chỉnh cho bệnh nhân A, thay thế DTG bằng RAL do tác dụng phụ không mong muốn.',
    N'Bệnh nhân A',
    N'Tác dụng phụ tương tự TDF, 3TC. RAL ít tác dụng phụ toàn thân hơn DTG.',
    N'Uống TDF+3TC 1 viên/ngày. RAL 2 viên/ngày, sáng và tối.',
    2, 1, 1),

(N'Phác đồ tùy chỉnh cho bệnh nhân B (Phụ nữ mang thai)',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'AZT'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'DRV'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'RTV'),
    N'Phác đồ tùy chỉnh cho bệnh nhân B (phụ nữ mang thai), sử dụng DRV/RTV thay vì NVP do tiền sử phát ban với NVP.',
    N'Bệnh nhân B (Phụ nữ mang thai)',
    N'Buồn nôn, tiêu chảy, tăng lipid máu (do DRV/RTV).',
    N'AZT+3TC: Uống 2 viên/ngày. DRV/RTV: Theo chỉ định của bác sĩ.',
    2, 1, 1),

-- Thêm các phác đồ tùy chỉnh cho các bệnh nhân khác
(N'Phác đồ tùy chỉnh cho bệnh nhân C',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'ABC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'DTG'),
    NULL,
    N'Phác đồ tùy chỉnh cho bệnh nhân C, thay thế TDF bằng ABC do suy thận.',
    N'Bệnh nhân C',
    N'Phác đồ an toàn cho bệnh nhân có vấn đề về thận.',
    N'Uống 1 viên/ngày vào buổi sáng.',
    1, 1, 1),

(N'Phác đồ tùy chỉnh cho bệnh nhân D',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'TDF'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'EFV'),
    NULL,
    N'Phác đồ tùy chỉnh cho bệnh nhân D, sử dụng EFV thay vì DTG do tương tác thuốc.',
    N'Bệnh nhân D',
    N'Chóng mặt, ác mộng (với EFV) - uống buổi tối để giảm tác dụng phụ.',
    N'Uống 1 viên/ngày vào buổi tối.',
    1, 1, 1),

(N'Phác đồ tùy chỉnh cho bệnh nhân E',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'AZT'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'RAL'),
    NULL,
    N'Phác đồ tùy chỉnh cho bệnh nhân E, sử dụng RAL do không dung nạp được với DTG.',
    N'Bệnh nhân E',
    N'Ít tác dụng phụ với RAL.',
    N'AZT+3TC: 2 viên/ngày. RAL: 2 viên/ngày.',
    2, 1, 1),

(N'Phác đồ tùy chỉnh cho bệnh nhân F',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'TDF'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'NVP'),
    NULL,
    N'Phác đồ tùy chỉnh cho bệnh nhân F, sử dụng NVP thay vì DTG do chi phí.',
    N'Bệnh nhân F',
    N'Cần theo dõi phát ban với NVP.',
    N'TDF+3TC: 1 viên/ngày. NVP: tăng liều dần.',
    1, 1, 1),

(N'Phác đồ tùy chỉnh cho bệnh nhân G',
    (SELECT component_id FROM ARVComponents WHERE component_name = 'ABC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = '3TC'),
    (SELECT component_id FROM ARVComponents WHERE component_name = 'RAL'),
    NULL,
    N'Phác đồ tùy chỉnh cho bệnh nhân G, phác đồ an toàn cho bệnh nhân có đa bệnh lý.',
    N'Bệnh nhân G',
    N'Phác đồ ít tương tác thuốc.',
    N'ABC+3TC: 1 viên/ngày. RAL: 2 viên/ngày.',
    2, 1, 1);

-- Chèn dữ liệu vào bảng TestType (PHẢI INSERT TRƯỚC KHI DÙNG TRONG APPOINTMENTS)
INSERT INTO TestType (test_type_name, unit, normal_range)
VALUES
(N'Xét nghiệm kháng thể HIV (Antibody Test)', 'S/C', N'< 0.90 S/C Units (Non-reactive - âm tính)'),
(N'Xét nghiệm kháng nguyên p24 (Antigen Test)', 'IU/mL', N'< 0.43 IU/mL (giói hạn phát hiện ~0.27 - 0.58 IU/mL)'),
(N'Xét nghiệm tải lượng virus HIV (Viral Load Test)', 'copies/mL', N'Khong co gia tri "binh thuong" cho nguoi khong nhiem; ngưỡng phát hiện: < 20 copies/mL (undetectable)'),
(N'Đếm tế bào CD4+ (CD4 Count)', 'cells/mm³', N'700 - 1100 cells/mm³ (người bình thường không nhiễm HIV)');

-- Chèn dữ liệu vào bảng DoctorSchedules
INSERT INTO DoctorSchedules (doctor_id, work_day, start_time, end_time)
VALUES
-- Doctor 1 làm việc thứ 2, 4, 6 (Monday, Wednesday, Friday)
(1, 'Monday', '08:00:00', '17:00:00'),
(1, 'Wednesday', '08:00:00', '17:00:00'),
(1, 'Friday', '08:00:00', '17:00:00'),
-- Doctor 2 làm việc thứ 3, 5, 7 (Tuesday, Thursday, Saturday)
(2, 'Tuesday', '08:00:00', '17:00:00'),
(2, 'Thursday', '08:00:00', '17:00:00'),
(2, 'Saturday', '08:00:00', '17:00:00');

-- Chèn dữ liệu vào bảng Appointments (SAU KHI ĐÃ CÓ TESTTYPE)
-- Tạo appointments từ tháng 2 đến tháng 8/2025, mỗi tháng 3-7 bệnh nhân
-- Doctor 1: Monday, Wednesday, Friday | Doctor 2: Tuesday, Thursday, Saturday
INSERT INTO Appointments (patient_id, doctor_id, appointment_date, appointment_time, appointment_type, status, test_type_id, isAnonymous)
VALUES
-- THÁNG 2/2025
(1, 1, '2025-02-03', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Monday
(2, 2, '2025-02-04', '10:00:00', 'Medication', 'Completed', 3, 0), -- Tuesday
(3, 1, '2025-02-05', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Wednesday
(4, 2, '2025-02-06', '14:00:00', 'Medication', 'Completed', 4, 0), -- Thursday
(5, 1, '2025-02-07', '15:00:00', 'Appointment', 'Completed', NULL, 0), -- Friday

-- THÁNG 3/2025
(6, 2, '2025-03-04', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Tuesday
(7, 1, '2025-03-05', '10:00:00', 'Medication', 'Completed', 1, 0), -- Wednesday
(1, 2, '2025-03-06', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Thursday
(2, 1, '2025-03-07', '14:00:00', 'Medication', 'Completed', 2, 0), -- Friday
(3, 2, '2025-03-08', '15:00:00', 'Appointment', 'Completed', NULL, 0), -- Saturday
(4, 1, '2025-03-10', '16:00:00', 'Medication', 'Completed', 3, 0), -- Monday

-- THÁNG 4/2025
(5, 2, '2025-04-01', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Tuesday
(6, 1, '2025-04-02', '10:00:00', 'Medication', 'Completed', 4, 0), -- Wednesday
(7, 2, '2025-04-03', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Thursday
(1, 1, '2025-04-04', '14:00:00', 'Medication', 'Completed', 1, 0), -- Friday
(2, 2, '2025-04-05', '15:00:00', 'Appointment', 'Completed', NULL, 0), -- Saturday
(3, 1, '2025-04-07', '16:00:00', 'Medication', 'Completed', 2, 0), -- Monday
(4, 2, '2025-04-08', '09:30:00', 'Appointment', 'Completed', NULL, 0), -- Tuesday

-- THÁNG 5/2025
(5, 1, '2025-05-02', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Friday
(6, 2, '2025-05-06', '10:00:00', 'Medication', 'Completed', 3, 0), -- Tuesday
(7, 1, '2025-05-07', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Wednesday
(1, 2, '2025-05-08', '14:00:00', 'Medication', 'Completed', 4, 0), -- Thursday
(2, 1, '2025-05-09', '15:00:00', 'Appointment', 'Completed', NULL, 0), -- Friday

-- THÁNG 6/2025
(3, 2, '2025-06-03', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Tuesday
(4, 1, '2025-06-04', '10:00:00', 'Medication', 'Completed', 1, 0), -- Wednesday
(5, 2, '2025-06-05', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Thursday
(6, 1, '2025-06-06', '14:00:00', 'Medication', 'Completed', 2, 0), -- Friday
(7, 2, '2025-06-07', '15:00:00', 'Appointment', 'Completed', NULL, 0), -- Saturday
(1, 1, '2025-06-09', '16:00:00', 'Medication', 'Completed', 3, 0), -- Monday

-- THÁNG 7/2025  
(2, 2, '2025-07-01', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Tuesday
(3, 1, '2025-07-02', '10:00:00', 'Medication', 'Completed', 4, 0), -- Wednesday
(4, 2, '2025-07-03', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Thursday
(5, 1, '2025-07-04', '14:00:00', 'Medication', 'Completed', 1, 0), -- Friday
(6, 2, '2025-07-05', '15:00:00', 'Appointment', 'Completed', NULL, 0), -- Saturday
(7, 1, '2025-07-07', '16:00:00', 'Medication', 'Completed', 2, 0), -- Monday
(1, 2, '2025-07-08', '09:30:00', 'Appointment', 'Completed', NULL, 0), -- Tuesday

-- THÁNG 8/2025
(2, 1, '2025-08-01', '09:00:00', 'Appointment', 'Completed', NULL, 0), -- Friday
(3, 2, '2025-08-05', '10:00:00', 'Medication', 'Completed', 3, 0), -- Tuesday
(4, 1, '2025-08-06', '11:00:00', 'Appointment', 'Completed', NULL, 0), -- Wednesday
(5, 2, '2025-08-07', '14:00:00', 'Medication', 'Completed', 4, 0), -- Thursday
(6, 1, '2025-08-08', '15:00:00', 'Appointment', 'Completed', NULL, 0); -- Friday

-- Chèn dữ liệu vào bảng TestResults (quan hệ 1-1 với Appointments)
INSERT INTO TestResults (appointment_id, patient_id, doctor_id, test_type_id, result_value, notes)
VALUES
-- THÁNG 2/2025 (appointments 1-5)
(1, 1, 1, 3, '850', N'Xét nghiệm tải lượng virus HIV - Kết quả cao, cần bắt đầu điều trị'),
(2, 2, 2, 3, '450', N'Xét nghiệm tải lượng virus HIV - Kết quả trung bình'),
(3, 3, 1, 4, '550', N'Xét nghiệm đếm tế bào CD4+ - Kết quả thấp, cần điều trị'),
(4, 4, 2, 4, '750', N'Xét nghiệm đếm tế bào CD4+ - Kết quả bình thường'),
(5, 5, 1, 1, '1.2', N'Xét nghiệm kháng thể HIV - Dương tính'),

-- THÁNG 3/2025 (appointments 6-11)
(6, 6, 2, 2, '0.6', N'Xét nghiệm kháng nguyên p24 - Dương tính'),
(7, 7, 1, 1, '0.95', N'Xét nghiệm kháng thể HIV - Dương tính'),
(8, 1, 2, 4, '600', N'Xét nghiệm CD4+ - Theo dõi điều trị'),
(9, 2, 1, 2, '0.5', N'Xét nghiệm kháng nguyên p24 - Theo dõi điều trị'),
(10, 3, 2, 3, '200', N'Xét nghiệm viral load - Giảm sau điều trị'),
(11, 4, 1, 3, '100', N'Xét nghiệm viral load - Đáp ứng tốt'),

-- THÁNG 4/2025 (appointments 12-18)
(12, 5, 2, 4, '650', N'Xét nghiệm CD4+ - Cải thiện sau điều trị'),
(13, 6, 1, 4, '700', N'Xét nghiệm CD4+ - Ổn định'),
(14, 7, 2, 3, '150', N'Xét nghiệm viral load - Tiếp tục giảm'),
(15, 1, 1, 1, '0.8', N'Xét nghiệm kháng thể HIV - Theo dõi'),
(16, 2, 2, 4, '680', N'Xét nghiệm CD4+ - Tăng tốt'),
(17, 3, 1, 2, '0.4', N'Xét nghiệm kháng nguyên p24 - Giảm'),
(18, 4, 2, 3, '50', N'Xét nghiệm viral load - Đạt undetectable'),

-- THÁNG 5/2025 (appointments 19-23)
(19, 5, 1, 3, '80', N'Xét nghiệm viral load - Duy trì undetectable'),
(20, 6, 2, 3, '120', N'Xét nghiệm viral load - Giảm tốt'),
(21, 7, 1, 4, '720', N'Xét nghiệm CD4+ - Phục hồi miễn dịch tốt'),
(22, 1, 2, 4, '650', N'Xét nghiệm CD4+ - Ổn định'),
(23, 2, 1, 3, '30', N'Xét nghiệm viral load - Undetectable'),

-- THÁNG 6/2025 (appointments 24-29)
(24, 3, 2, 4, '680', N'Xét nghiệm CD4+ - Tiếp tục cải thiện'),
(25, 4, 1, 1, '0.75', N'Xét nghiệm kháng thể HIV - Ổn định'),
(26, 5, 2, 3, '40', N'Xét nghiệm viral load - Duy trì undetectable'),
(27, 6, 1, 2, '0.3', N'Xét nghiệm kháng nguyên p24 - Giảm mạnh'),
(28, 7, 2, 4, '750', N'Xét nghiệm CD4+ - Phục hồi tốt'),
(29, 1, 1, 3, '25', N'Xét nghiệm viral load - Undetectable stable'),

-- THÁNG 7/2025 (appointments 30-36)
(30, 2, 2, 4, '700', N'Xét nghiệm CD4+ - Tăng ổn định'),
(31, 3, 1, 4, '720', N'Xét nghiệm CD4+ - Duy trì tốt'),
(32, 4, 2, 3, '35', N'Xét nghiệm viral load - Undetectable'),
(33, 5, 1, 1, '0.7', N'Xét nghiệm kháng thể HIV - Ổn định'),
(34, 6, 2, 3, '45', N'Xét nghiệm viral load - Undetectable'),
(35, 7, 1, 2, '0.25', N'Xét nghiệm kháng nguyên p24 - Thấp'),
(36, 1, 2, 4, '680', N'Xét nghiệm CD4+ - Duy trì tốt'),

-- THÁNG 8/2025 (appointments 37-41)
(37, 2, 1, 3, '20', N'Xét nghiệm viral load - Undetectable excellent'),
(38, 3, 2, 3, '30', N'Xét nghiệm viral load - Undetectable'),
(39, 4, 1, 4, '730', N'Xét nghiệm CD4+ - Rất tốt'),
(40, 5, 2, 4, '710', N'Xét nghiệm CD4+ - Ổn định cao'),
(41, 6, 1, 3, '40', N'Xét nghiệm viral load - Duy trì undetectable');

-- Chèn dữ liệu vào bảng Treatment
INSERT INTO Treatment (test_result_id, regimen_id, start_date, end_date, status, notes)
VALUES
-- Treatments cho các kết quả xét nghiệm từ tháng 2/2025
(1, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-02-03', '2025-12-31', 'Active', N'Bắt đầu điều trị với phác đồ chuẩn - Bệnh nhân A'),
(2, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân B (Phụ nữ mang thai)'), '2025-02-04', '2025-12-31', 'Active', N'Điều trị cho phụ nữ mang thai - Bệnh nhân B'),
(3, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân C'), '2025-02-05', '2025-12-31', 'Active', N'Phác đồ tùy chỉnh thay thế TDF - Bệnh nhân C'),
(4, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân D'), '2025-02-06', '2025-12-31', 'Active', N'Phác đồ tùy chỉnh với EFV - Bệnh nhân D'),
(5, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân E'), '2025-02-07', '2025-12-31', 'Active', N'Phác đồ tùy chỉnh với RAL - Bệnh nhân E'),
(6, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân F'), '2025-03-04', '2025-12-31', 'Active', N'Phác đồ tùy chỉnh với NVP - Bệnh nhân F'),
(7, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân G'), '2025-03-05', '2025-12-31', 'Active', N'Phác đồ tùy chỉnh ABC+RAL - Bệnh nhân G'),

-- Treatments cho các lần tái khám và điều chỉnh phác đồ
(8, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-03-06', '2025-12-31', 'Active', N'Điều chỉnh liều lượng - Bệnh nhân A'),
(9, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân B (Phụ nữ mang thai)'), '2025-03-07', '2025-12-31', 'Active', N'Theo dõi thai kỳ - Bệnh nhân B'),
(10, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân C'), '2025-03-08', '2025-12-31', 'Active', N'Đáp ứng tốt với phác đồ - Bệnh nhân C'),
(11, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân D'), '2025-03-10', '2025-12-31', 'Active', N'Giảm tác dụng phụ - Bệnh nhân D'),

-- Tiếp tục với các treatments từ tháng 4-8
(12, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân E'), '2025-04-01', '2025-12-31', 'Active', N'Duy trì điều trị ổn định - Bệnh nhân E'),
(13, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân F'), '2025-04-02', '2025-12-31', 'Active', N'Không có phát ban với NVP - Bệnh nhân F'),
(14, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân G'), '2025-04-03', '2025-12-31', 'Active', N'Duy trì phác đồ an toàn - Bệnh nhân G'),
(15, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-04-04', '2025-12-31', 'Active', N'Viral load giảm tốt - Bệnh nhân A'),
(16, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ AZT + 3TC + NVP Chuẩn'), '2025-04-05', '2025-12-31', 'Active', N'Chuyển sang phác đồ chuẩn - Bệnh nhân B'),
(17, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân C'), '2025-04-07', '2025-12-31', 'Active', N'Kết quả xét nghiệm tốt - Bệnh nhân C'),
(18, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân D'), '2025-04-08', '2025-12-31', 'Active', N'Đạt undetectable - Bệnh nhân D'),

-- Treatments tháng 5-8 (duy trì điều trị)
(19, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân E'), '2025-05-02', '2025-12-31', 'Active', N'Duy trì undetectable - Bệnh nhân E'),
(20, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân F'), '2025-05-06', '2025-12-31', 'Active', N'Điều trị ổn định - Bệnh nhân F'),
(21, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân G'), '2025-05-07', '2025-12-31', 'Active', N'Phục hồi miễn dịch tốt - Bệnh nhân G'),
(22, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-05-08', '2025-12-31', 'Active', N'CD4 ổn định - Bệnh nhân A'),
(23, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ AZT + 3TC + NVP Chuẩn'), '2025-05-09', '2025-12-31', 'Active', N'Đạt undetectable - Bệnh nhân B'),

-- Treatments cuối (tháng 6-8)
(24, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân C'), '2025-06-03', '2025-12-31', 'Active', N'Tiếp tục cải thiện - Bệnh nhân C'),
(25, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân D'), '2025-06-04', '2025-12-31', 'Active', N'Kết quả ổn định - Bệnh nhân D'),
(26, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân E'), '2025-06-05', '2025-12-31', 'Active', N'Duy trì undetectable - Bệnh nhân E'),
(27, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân F'), '2025-06-06', '2025-12-31', 'Active', N'Giảm mạnh kháng nguyên - Bệnh nhân F'),
(28, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân G'), '2025-06-07', '2025-12-31', 'Active', N'Phục hồi tốt - Bệnh nhân G'),
(29, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-06-09', '2025-12-31', 'Active', N'Undetectable ổn định - Bệnh nhân A'),

-- Treatments tháng 7-8 (tiếp tục duy trì)
(30, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ AZT + 3TC + NVP Chuẩn'), '2025-07-01', '2025-12-31', 'Active', N'CD4 tăng ổn định - Bệnh nhân B'),
(31, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân C'), '2025-07-02', '2025-12-31', 'Active', N'Duy trì tốt - Bệnh nhân C'),
(32, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân D'), '2025-07-03', '2025-12-31', 'Active', N'Undetectable - Bệnh nhân D'),
(33, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân E'), '2025-07-04', '2025-12-31', 'Active', N'Kết quả ổn định - Bệnh nhân E'),
(34, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân F'), '2025-07-05', '2025-12-31', 'Active', N'Undetectable - Bệnh nhân F'),
(35, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân G'), '2025-07-07', '2025-12-31', 'Active', N'Kháng nguyên thấp - Bệnh nhân G'),
(36, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-07-08', '2025-12-31', 'Active', N'Duy trì tốt - Bệnh nhân A'),

-- Treatments tháng 8
(37, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ AZT + 3TC + NVP Chuẩn'), '2025-08-01', '2025-12-31', 'Active', N'Undetectable xuất sắc - Bệnh nhân B'),
(38, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân C'), '2025-08-05', '2025-12-31', 'Active', N'Undetectable - Bệnh nhân C'),
(39, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân D'), '2025-08-06', '2025-12-31', 'Active', N'CD4 rất tốt - Bệnh nhân D'),
(40, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân E'), '2025-08-07', '2025-12-31', 'Active', N'Ổn định cao - Bệnh nhân E'),
(41, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ tùy chỉnh cho bệnh nhân F'), '2025-08-08', '2025-12-31', 'Active', N'Duy trì undetectable - Bệnh nhân F');

---- Chèn dữ liệu vào bảng Notifications
--INSERT INTO Notifications (user_id, appointment_id, treatment_id, notification_type, scheduled_time, status)
--VALUES
---- Thông báo lịch hẹn
--(1, 1, NULL, 'Appointment', '2025-06-29 15:00:00', 'Pending'),
--(2, 2, NULL, 'Appointment', '2025-06-30 10:00:00', 'Pending'),
--(1, 3, NULL, 'Appointment', '2025-07-04 14:00:00', 'Pending'),
---- Thông báo uống thuốc (sử dụng treatment_id thực tế)
--(1, NULL, 1, 'Medication', '2025-06-26 08:00:00', 'Pending'),
--(1, NULL, 1, 'Medication', '2025-06-27 08:00:00', 'Pending'),
--(2, NULL, 2, 'Medication', '2025-06-26 07:00:00', 'Pending'),
--(2, NULL, 2, 'Medication', '2025-06-26 19:00:00', 'Pending'),
--(2, NULL, 2, 'Medication', '2025-06-27 07:00:00', 'Pending'),
--(2, NULL, 2, 'Medication', '2025-06-27 19:00:00', 'Pending'),
---- Thông báo chung
--(1, NULL, NULL, 'General', '2025-06-25 09:00:00', 'Sent'),
--(2, NULL, NULL, 'General', '2025-06-25 09:30:00', 'Sent');
