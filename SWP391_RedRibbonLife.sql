-- Tạo cơ sở dữ liệu SWP391_RedRibbonLife
CREATE DATABASE SWP391_RedRibbonLife;
GO

-- Sử dụng cơ sở dữ liệu vừa tạo
USE SWP391_RedRibbonLife;
GO

-- Phần 1: Tạo các bảng

-- Bảng vai trò người dùng
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng người dùng
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Address NVARCHAR(200),
    Phone VARCHAR(20),
    Email VARCHAR(100),
    IsAnonymous BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng thông tin bác sĩ
CREATE TABLE Doctors (
    DoctorCode VARCHAR(20) PRIMARY KEY,
    UserID INT NOT NULL,
    Specialization NVARCHAR(100),
    LicenseNumber VARCHAR(50),
    Qualification NVARCHAR(200),
    Experience INT, -- Số năm kinh nghiệm
    Biography NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng bệnh nhân
CREATE TABLE Patients (
    PatientCode VARCHAR(20) PRIMARY KEY,
    UserID INT NOT NULL,
    InsuranceNumber VARCHAR(50),
    EmergencyContact NVARCHAR(100),
    EmergencyPhone VARCHAR(20),
    BloodType VARCHAR(5),
    Weight DECIMAL(5,2),
    Height DECIMAL(5,2),
    IsHIVPositive BIT DEFAULT 0,
    HIVDiagnosisDate DATE,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng lịch làm việc của bác sĩ
CREATE TABLE DoctorSchedules (
    ScheduleID INT PRIMARY KEY IDENTITY(1,1),
    DoctorCode VARCHAR(20) NOT NULL,
    WeekDay INT, -- 1: Thứ hai, 2: Thứ ba, ...
    StartTime TIME,
    EndTime TIME,
    MaxAppointments INT,
    IsAvailable BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng phác đồ ARV
CREATE TABLE ARVRegimens (
    RegimenID INT PRIMARY KEY IDENTITY(1,1),
    RegimenName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Medications NVARCHAR(255),
    UseCase NVARCHAR(200), -- Ví dụ: phụ nữ mang thai, trẻ em...
    SideEffects NVARCHAR(500),
    Recommendations NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng lịch khám điều trị
CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    PatientCode VARCHAR(20) NOT NULL,
    DoctorCode VARCHAR(20) NOT NULL,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    Purpose NVARCHAR(200),
    Status NVARCHAR(20) DEFAULT 'Scheduled', -- Scheduled, Completed, Cancelled
    Notes NVARCHAR(MAX),
    IsOnline BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng kết quả xét nghiệm
CREATE TABLE TestResults (
    TestID INT PRIMARY KEY IDENTITY(1,1),
    PatientCode VARCHAR(20) NOT NULL,
    AppointmentID INT,
    TestType NVARCHAR(50) NOT NULL, -- CD4, Viral Load, etc.
    TestDate DATE NOT NULL,
    Results NVARCHAR(100),
    Units NVARCHAR(20),
    NormalRange NVARCHAR(50),
    Interpretation NVARCHAR(MAX),
    PerformedBy INT, -- UserID của người thực hiện xét nghiệm
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng hồ sơ điều trị HIV
CREATE TABLE HIVTreatments (
    TreatmentID INT PRIMARY KEY IDENTITY(1,1),
    PatientCode VARCHAR(20) NOT NULL,
    DoctorCode VARCHAR(20) NOT NULL,
    RegimenID INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE,
    Status NVARCHAR(20) DEFAULT 'Active', -- Active, Completed, Changed
    Dosage NVARCHAR(100),
    Frequency NVARCHAR(50),
    SideEffectsObserved NVARCHAR(500),
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng theo dõi CD4 và tải lượng HIV
CREATE TABLE HIVMonitoring (
    MonitoringID INT PRIMARY KEY IDENTITY(1,1),
    PatientCode VARCHAR(20) NOT NULL,
    TestDate DATE NOT NULL,
    CD4Count INT,
    ViralLoad DECIMAL(10,2),
    Notes NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng nhắc nhở uống thuốc
CREATE TABLE MedicationReminders (
    ReminderID INT PRIMARY KEY IDENTITY(1,1),
    PatientCode VARCHAR(20) NOT NULL,
    TreatmentID INT NOT NULL,
    ReminderTime TIME NOT NULL,
    Frequency NVARCHAR(50), -- Daily, Twice daily, etc.
    IsActive BIT DEFAULT 1,
    LastSent DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng nhắc nhở tái khám
CREATE TABLE AppointmentReminders (
    ReminderID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT NOT NULL,
    PatientCode VARCHAR(20) NOT NULL,
    ReminderDate DATE NOT NULL,
    IsSent BIT DEFAULT 0,
    SentDate DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng tài liệu giáo dục HIV
CREATE TABLE EducationalMaterials (
    MaterialID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX),
    Category NVARCHAR(50), -- Prevention, Treatment, Awareness
    PublishedDate DATE,
    Author INT, -- UserID
    IsPublished BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Bảng bài viết blog
CREATE TABLE BlogPosts (
    PostID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX),
    Author INT NOT NULL, -- UserID
    PublishedDate DATETIME,
    IsPublished BIT DEFAULT 0,
    ViewCount INT DEFAULT 0,
    Tags NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE()
);

-- Phần 2: Tạo các khóa ngoại

-- Khóa ngoại cho bảng Users
ALTER TABLE Users
ADD CONSTRAINT FK_Users_Roles
FOREIGN KEY (RoleID) REFERENCES Roles(RoleID);

-- Khóa ngoại cho bảng Doctors
ALTER TABLE Doctors
ADD CONSTRAINT FK_Doctors_Users
FOREIGN KEY (UserID) REFERENCES Users(UserID);

-- Khóa ngoại cho bảng Patients
ALTER TABLE Patients
ADD CONSTRAINT FK_Patients_Users
FOREIGN KEY (UserID) REFERENCES Users(UserID);

-- Khóa ngoại cho bảng DoctorSchedules
ALTER TABLE DoctorSchedules
ADD CONSTRAINT FK_DoctorSchedules_Doctors
FOREIGN KEY (DoctorCode) REFERENCES Doctors(DoctorCode);

-- Khóa ngoại cho bảng Appointments
ALTER TABLE Appointments
ADD CONSTRAINT FK_Appointments_Patients
FOREIGN KEY (PatientCode) REFERENCES Patients(PatientCode);

-- Khóa ngoại cho bảng Appointments
ALTER TABLE Appointments
ADD CONSTRAINT FK_Appointments_Doctors
FOREIGN KEY (DoctorCode) REFERENCES Doctors(DoctorCode);

-- Khóa ngoại cho bảng TestResults
ALTER TABLE TestResults
ADD CONSTRAINT FK_TestResults_Patients
FOREIGN KEY (PatientCode) REFERENCES Patients(PatientCode);

ALTER TABLE TestResults
ADD CONSTRAINT FK_TestResults_Appointments
FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID);

ALTER TABLE TestResults
ADD CONSTRAINT FK_TestResults_Users
FOREIGN KEY (PerformedBy) REFERENCES Users(UserID);

-- Khóa ngoại cho bảng HIVTreatments
ALTER TABLE HIVTreatments
ADD CONSTRAINT FK_HIVTreatments_Patients
FOREIGN KEY (PatientCode) REFERENCES Patients(PatientCode);

-- Khóa ngoại cho bảng HIVTreatments
ALTER TABLE HIVTreatments
ADD CONSTRAINT FK_HIVTreatments_Doctors
FOREIGN KEY (DoctorCode) REFERENCES Doctors(DoctorCode);

ALTER TABLE HIVTreatments
ADD CONSTRAINT FK_HIVTreatments_ARVRegimens
FOREIGN KEY (RegimenID) REFERENCES ARVRegimens(RegimenID);

-- Khóa ngoại cho bảng HIVMonitoring
ALTER TABLE HIVMonitoring
ADD CONSTRAINT FK_HIVMonitoring_Patients
FOREIGN KEY (PatientCode) REFERENCES Patients(PatientCode);

-- Khóa ngoại cho bảng MedicationReminders
ALTER TABLE MedicationReminders
ADD CONSTRAINT FK_MedicationReminders_Patients
FOREIGN KEY (PatientCode) REFERENCES Patients(PatientCode);

ALTER TABLE MedicationReminders
ADD CONSTRAINT FK_MedicationReminders_HIVTreatments
FOREIGN KEY (TreatmentID) REFERENCES HIVTreatments(TreatmentID);

-- Khóa ngoại cho bảng AppointmentReminders
ALTER TABLE AppointmentReminders
ADD CONSTRAINT FK_AppointmentReminders_Appointments
FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID);

ALTER TABLE AppointmentReminders
ADD CONSTRAINT FK_AppointmentReminders_Patients
FOREIGN KEY (PatientCode) REFERENCES Patients(PatientCode);

-- Khóa ngoại cho bảng EducationalMaterials
ALTER TABLE EducationalMaterials
ADD CONSTRAINT FK_EducationalMaterials_Users
FOREIGN KEY (Author) REFERENCES Users(UserID);

-- Khóa ngoại cho bảng BlogPosts
ALTER TABLE BlogPosts
ADD CONSTRAINT FK_BlogPosts_Users
FOREIGN KEY (Author) REFERENCES Users(UserID);