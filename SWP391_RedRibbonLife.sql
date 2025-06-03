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
    isActive BIT DEFAULT 1 NOT NULL,
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
    category_name NVARCHAR(100) NOT NULL
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

-- 11. Bảng DoctorSchedules (Lưu lịch làm việc của bác sĩ)
CREATE TABLE DoctorSchedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT,
    work_day NVARCHAR(50) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    CONSTRAINT chk_work_day CHECK (work_day IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'))
);

-- 12. Bảng TreatmentHistories (Lưu lịch sử phác đồ điều trị của bệnh nhân)
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

-- 14. Bảng MedicationSchedules (Sử dụng để báo người dùng uống thuốc)
CREATE TABLE MedicationSchedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    prescription_id INT,
    patient_id INT,
    medication_time INT NOT NULL,
    sent_at DATETIME NULL
);

-- FOREIGN KEY
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

ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE Appointments
ADD CONSTRAINT fk_appointments_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE Reminders
ADD CONSTRAINT fk_reminders_appointments
FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_appointments
FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE TestResults
ADD CONSTRAINT fk_test_results_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE DoctorSchedules
ADD CONSTRAINT fk_schedules_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE TreatmentHistories
ADD CONSTRAINT fk_treatment_patients
FOREIGN KEY (patient_id) REFERENCES Patients(patient_id);

ALTER TABLE TreatmentHistories
ADD CONSTRAINT fk_treatment_doctors
FOREIGN KEY (doctor_id) REFERENCES Doctors(doctor_id);

ALTER TABLE Prescriptions
ADD CONSTRAINT fk_prescriptions_treatment
FOREIGN KEY (treatment_id) REFERENCES TreatmentHistories(treatment_id);

ALTER TABLE Prescriptions
ADD CONSTRAINT fk_prescriptions_regimen
FOREIGN KEY (regimen_id) REFERENCES ARVRegimens(regimen_id);

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
INSERT INTO Articles (title, content, thumbnail_image, category_id, isActive)
VALUES
(N'Giới thiệu về Red Ribbon Life', N'Red Ribbon Life là tổ chức hỗ trợ bệnh nhân HIV/AIDS với sứ mệnh cung cấp dịch vụ y tế, giáo dục và giảm kỳ thị. Chúng tôi cam kết mang lại cuộc sống tốt đẹp hơn cho cộng đồng.', 'about_us.jpg', 1, 1),
(N'Dịch vụ y tế tại Red Ribbon Life', N'Chúng tôi cung cấp tư vấn, xét nghiệm và điều trị HIV/AIDS với đội ngũ bác sĩ chuyên môn cao và cơ sở vật chất hiện đại.', 'services.jpg', 1, 1),
(N'Hiểu biết cơ bản về HIV/AIDS', N'HIV là virus gây suy giảm miễn dịch ở người. Bài viết này giải thích cách lây truyền, phòng ngừa và điều trị HIV.', 'hiv_education.jpg', 2, 1),
(N'Phòng ngừa HIV trong cộng đồng', N'Hướng dẫn các biện pháp phòng ngừa HIV như sử dụng bao cao su, xét nghiệm định kỳ và sử dụng PrEP.', 'prevention.jpg', 2, 1),
(N'Vượt qua kỳ thị: Câu chuyện của một bệnh nhân', N'Một bệnh nhân chia sẻ hành trình sống chung với HIV và cách họ vượt qua định kiến xã hội.', 'stigma_reduction.jpg', 3, 1),
(N'Tại sao cần nói không với kỳ thị HIV', N'Bài viết thảo luận về tác động của kỳ thị và cách cộng đồng có thể hỗ trợ bệnh nhân HIV.', 'no_stigma.jpg', 3, 1),
(N'Hành trình sống chung với HIV', N'Một bệnh nhân kể về trải nghiệm cá nhân, từ khi phát hiện bệnh đến việc duy trì lối sống tích cực.', 'blog1.jpg', 4, 1),
(N'Kinh nghiệm hỗ trợ bệnh nhân HIV từ bác sĩ', N'Bác sĩ chia sẻ những bài học và câu chuyện từ quá trình làm việc với bệnh nhân HIV.', 'doctor_blog.jpg', 4, 1);

-- Chèn dữ liệu vào bảng ARVRegimens
INSERT INTO ARVRegimens (regimen_name, regimen_code, components, description, suitable_for, side_effects, usage_instructions, isActive)
VALUES
(N'Phác đồ TDF + 3TC + DTG', 'TDF-3TC-DTG', N'Tenofovir (TDF), Lamivudine (3TC), Dolutegravir (DTG)', N'Phác đồ điều trị HIV cho người lớn và trẻ em trên 10 tuổi.', N'Người lớn, trẻ em trên 10 tuổi', N'Buồn nôn, nhức đầu, mệt mỏi', N'Uống 1 viên/ngày vào buổi sáng.', 1),
(N'Phác đồ AZT + 3TC + NVP', 'AZT-3TC-NVP', N'Zidovudine (AZT), Lamivudine (3TC), Nevirapine (NVP)', N'Phác đồ điều trị HIV cho phụ nữ mang thai.', N'Phụ nữ mang thai', N'Tiêu chảy, phát ban, thiếu máu', N'Uống 2 viên/ngày, sáng và tối.', 1),
(N'Phác đồ ABC + 3TC + EFV', 'ABC-3TC-EFV', N'Abacavir (ABC), Lamivudine (3TC), Efavirenz (EFV)', N'Phác đồ điều trị HIV cho trẻ em.', N'Trẻ em dưới 10 tuổi', N'Phát ban, nhức đầu, chóng mặt', N'Uống 1 viên/ngày vào buổi tối.', 1);

-- Chèn dữ liệu vào bảng DoctorSchedules
INSERT INTO DoctorSchedules (doctor_id, work_day, start_time, end_time)
VALUES
(1, 'Monday', '08:00:00', '12:00:00'),
(1, 'Wednesday', '13:00:00', '17:00:00'),
(2, 'Tuesday', '09:00:00', '12:00:00'),
(2, 'Thursday', '14:00:00', '18:00:00');

-- Chèn dữ liệu vào bảng Appointments
INSERT INTO Appointments (patient_id, doctor_id, appointment_date, appointment_time, appointment_type, status, isAnonymous)
VALUES
(1, 1, '2025-06-10', '09:00:00', 'Appointment', 'Scheduled', 0),
(2, 2, '2025-06-11', '10:00:00', 'Appointment', 'Scheduled', 1),
(1, 2, '2025-06-15', '14:00:00', 'Medication', 'Scheduled', 0);

-- Chèn dữ liệu vào bảng Reminders
INSERT INTO Reminders (appointment_id, reminder_time, reminder_type, status, sent_at)
VALUES
(1, '2025-06-09 09:00:00', 'Appointment', 'Pending', NULL),
(2, '2025-06-10 10:00:00', 'Appointment', 'Pending', NULL),
(3, '2025-06-14 14:00:00', 'Medication', 'Pending', NULL);

-- Chèn dữ liệu vào bảng TestResults
INSERT INTO TestResults (appointment_id, patient_id, doctor_id, test_type, result_value, unit, normal_range, notes)
VALUES
(1, 1, 1, N'Tải lượng HIV', '500', 'copies/mL', 'Dưới 200', N'Tải lượng virus cao, cần theo dõi'),
(1, 1, 1, N'CD4', '350', 'cells/mm³', '500-1500', N'Số lượng CD4 thấp'),
(2, 2, 2, N'Tải lượng HIV', '100', 'copies/mL', 'Dưới 200', N'Tải lượng virus trong ngưỡng kiểm soát'),
(2, 2, 2, N'CD4', '600', 'cells/mm³', '500-1500', N'Số lượng CD4 bình thường');

-- Chèn dữ liệu vào bảng TreatmentHistories
INSERT INTO TreatmentHistories (patient_id, doctor_id, start_date, end_date, status, notes)
VALUES
(1, 1, '2025-06-01', NULL, 'Active', N'Đang điều trị với phác đồ TDF + 3TC + DTG'),
(2, 2, '2025-06-05', NULL, 'Active', N'Đang điều trị với phác đồ AZT + 3TC + NVP');

-- Chèn dữ liệu vào bảng Prescriptions
INSERT INTO Prescriptions (treatment_id, regimen_id)
VALUES
(1, 1),  -- Phác đồ TDF + 3TC + DTG cho bệnh nhân 1
(2, 2);  -- Phác đồ AZT + 3TC + NVP cho bệnh nhân 2

-- Chèn dữ liệu vào bảng MedicationSchedules
INSERT INTO MedicationSchedules (prescription_id, patient_id, medication_time, sent_at)
VALUES
(1, 1, 8, NULL),  -- Uống thuốc lúc 8h sáng
(1, 1, 20, NULL), -- Uống thuốc lúc 8h tối
(2, 2, 7, NULL),  -- Uống thuốc lúc 7h sáng
(2, 2, 19, NULL); -- Uống thuốc lúc 7h tối