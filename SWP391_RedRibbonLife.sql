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
('patient1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs1@gmail.com', '0123456789', N'Nguyễn Văn A', '1990-01-01', 'Male', N'Hà Nội', 'Patient', 1, 0),
('patient2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs2@gmail.com', '0987654321', N'Trần Thị B', '1995-05-05', 'Female', N'Hồ Chí Minh', 'Patient', 1, 1);

-- Chèn dữ liệu cho vai trò Doctor
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('doctor1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs3@gmail', '0123456780', N'Lê Văn C', '1980-02-02', 'Male', N'Đà Nẵng', 'Doctor', 1, 1),
('doctor2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs4@gmail', '0987654320', N'Phạm Thị D', '1985-06-06', 'Female', N'Hải Phòng', 'Doctor', 1, 1);

-- Chèn dữ liệu cho vai trò Staff
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('staff1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs5@gmail', '0123456781', N'Nguyễn Văn E', '1992-03-03', 'Male', N'Cần Thơ', 'Staff', 1, 1),
('staff2', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs6@gmail', '0987654322', N'Trần Thị F', '1997-07-07', 'Female', N'Vũng Tàu', 'Staff', 1, 1);

-- Chèn dữ liệu cho vai trò Manager
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('manager1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs7@gmail', '0123456782', N'Lê Văn G', '1988-04-04', 'Male', N'Nha Trang', 'Manager', 1, 1);

-- Chèn dữ liệu cho vai trò Admin
INSERT INTO Users (username, password, email, phone_number, full_name, date_of_birth, gender, address, user_role, isActive, isVerified)
VALUES
('admin1', 'k5NO/P09k0O5WkdqOLceCM8FWrxLD6FxcrYhtGGMoDw=', 'hnnthcs8@gmail', '0987654323', N'Phạm Thị H', '1990-08-08', 'Female', N'Huế', 'Admin', 1, 1);

-- Chèn dữ liệu vào bảng Patients (cho Patient)
INSERT INTO Patients (user_id, blood_type, is_pregnant, special_notes, createdAt)
VALUES
(1, 'A+', 0, N'Không có ghi chú đặc biệt', GETDATE()),
(2, 'B-', 1, N'Đang mang thai', GETDATE());

-- Chèn dữ liệu vào bảng Doctors (cho Doctor)
INSERT INTO Doctors (user_id, doctor_image, bio)
VALUES
(3, 'doctor1.jpg', N'Bác sĩ chuyên khoa HIV'),
(4, 'doctor2.jpg', N'Bác sĩ có kinh nghiệm trong điều trị HIV');

-- Chèn dữ liệu vào bảng DoctorCertificates (cho Doctor)
INSERT INTO DoctorCertificates (doctor_id, certificate_name, issued_by, issue_date, expiry_date, certificate_image)
VALUES
(1, N'Bằng cấp Y khoa', N'Đại học Y Hà Nội', '2005-06-15', NULL, 'cert1.jpg'),
(1, N'Chứng chỉ chuyên khoa HIV', N'Bộ Y tế', '2010-09-20', '2025-09-20', 'cert2.jpg'),
(2, N'Bằng cấp Y khoa', N'Đại học Y Hồ Chí Minh', '2007-07-10', NULL, 'cert3.jpg'),
(2, N'Chứng chỉ điều trị HIV', N'Hội Y học Việt Nam', '2012-11-15', '2027-11-15', 'cert4.jpg');

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
(N'Giới thiệu về Red Ribbon Life', N'Red Ribbon Life là tổ chức hỗ trợ bệnh nhân HIV/AIDS với sứ mệnh cung cấp dịch vụ y tế, giáo dục và giảm kỳ thị. Chúng tôi cam kết mang lại cuộc sống tốt đẹp hơn cho cộng đồng.', 1, 1, '2025-06-01', 5), -- Nguyễn Văn E (Staff)
(N'Dịch vụ y tế tại Red Ribbon Life', N'Chúng tôi cung cấp tư vấn, xét nghiệm và điều trị HIV/AIDS với đội ngũ bác sĩ chuyên môn cao và cơ sở vật chất hiện đại.', 1, 1, '2025-06-02', 6), -- Trần Thị F (Staff)
(N'Hiểu biết cơ bản về HIV/AIDS', N'HIV là virus gây suy giảm miễn dịch ở người. Bài viết này giải thích cách lây truyền, phòng ngừa và điều trị HIV.', 2, 1, '2025-06-03', 5), -- Nguyễn Văn E (Staff)
(N'Phòng ngừa HIV trong cộng đồng', N'Hướng dẫn các biện pháp phòng ngừa HIV như sử dụng bao cao su, xét nghiệm định kỳ và sử dụng PrEP.', 2, 1, '2025-06-04', 6), -- Trần Thị F (Staff)
(N'Vượt qua kỳ thị: Câu chuyện của một bệnh nhân', N'Một bệnh nhân chia sẻ hành trình sống tạp với HIV và cách họ vượt qua định kiến xã hội.', 3, 1, '2025-06-05', 5), -- Nguyễn Văn E (Staff)
(N'Tại sao cần nói không với kỳ thị HIV', N'Bài viết thảo luận về tác động của kỳ thị và cách cộng đồng có thể hỗ trợ bệnh nhân HIV.', 3, 1, '2025-06-06', 6), -- Trần Thị F (Staff)
(N'Hành trình sống chung với HIV', N'Một bệnh nhân kể về trải nghiệm cá nhân, từ khi phát hiện bệnh đến việc duy trì lối sống tích cực.', 4, 1, '2025-06-07', 5), -- Nguyễn Văn E (Staff)
(N'Kinh nghiệm hỗ trợ bệnh nhân HIV từ bác sĩ', N'Bác sĩ chia sẻ những bài học và câu chuyện từ quá trình làm việc với bệnh nhân HIV.', 4, 1, '2025-06-08', 6); -- Trần Thị F (Staff)

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
(1, 'Monday', '08:00:00', '12:00:00'),
(1, 'Wednesday', '13:00:00', '17:00:00'),
(2, 'Tuesday', '09:00:00', '12:00:00'),
(2, 'Thursday', '14:00:00', '18:00:00');

-- Chèn dữ liệu vào bảng Appointments (SAU KHI ĐÃ CÓ TESTTYPE)
INSERT INTO Appointments (patient_id, doctor_id, appointment_date, appointment_time, appointment_type, status, test_type_id, isAnonymous)
VALUES
(1, 1, '2025-06-30', '09:00:00', 'Appointment', 'Scheduled', NULL, 0),
(2, 2, '2025-07-01', '10:00:00', 'Appointment', 'Scheduled', NULL, 1),
(1, 2, '2025-07-05', '14:00:00', 'Medication', 'Scheduled', 3, 0);

-- Chèn dữ liệu vào bảng TestResults (quan hệ 1-1 với Appointments)
INSERT INTO TestResults (appointment_id, patient_id, doctor_id, test_type_id, result_value, notes)
VALUES
-- Mỗi appointment chỉ có 1 kết quả xét nghiệm
(1, 1, 1, 3, '500', N'Xét nghiệm tải lượng virus HIV - Kết quả cao, cần theo dõi'),
(2, 2, 2, 4, '600', N'Xét nghiệm đếm tế bào CD4+ - Kết quả bình thường'),
(3, 1, 2, 1, '0.85', N'Xét nghiệm kháng thể HIV - Kết quả âm tính');

-- Chèn dữ liệu vào bảng Treatment
INSERT INTO Treatment (test_result_id, regimen_id, start_date, end_date, status, notes)
VALUES
(1, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ TDF + 3TC + DTG Chuẩn'), '2025-06-01', '2025-12-31', 'Active', N'Đang điều trị với phác đồ TDF + 3TC + DTG'),
(2, (SELECT regimen_id FROM ARVRegimens WHERE regimen_name = N'Phác đồ AZT + 3TC + NVP Chuẩn'), '2025-06-05', '2025-12-31', 'Active', N'Đang điều trị với phác đồ AZT + 3TC + NVP');

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