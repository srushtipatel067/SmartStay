CREATE DATABASE SmartStayDB;
GO

USE SmartStayDB;
GO

---------USER table------------
CREATE TABLE tbl_Users
(
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    DateOfBirth DATE NULL,
    Address NVARCHAR(250) NULL,
    ProfileImage NVARCHAR(500) NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Customer',
    IsActive BIT NOT NULL DEFAULT 1,

    OtpCode NVARCHAR(10) NULL,
    OtpExpiry DATETIME NULL,

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    IsOtpVerified BIT NOT NULL DEFAULT 0,
    OtpVerifiedAt DATETIME NULL
);
GO


--------------Stored Procedures------------

---------sp_User_Register----------
CREATE OR ALTER PROCEDURE sp_User_Register
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Role NVARCHAR(20) = 'Customer'
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM tbl_Users WHERE Email = @Email)
    BEGIN
        SELECT 0 AS Success, 'Email already exists' AS Message;
        RETURN;
    END

    INSERT INTO tbl_Users
    (FullName, Email, PasswordHash, PhoneNumber, Role, IsActive, CreatedAt)
    VALUES
    (@FullName, @Email, @PasswordHash, @PhoneNumber, @Role, 1, GETDATE());

    SELECT 
        1 AS Success,
        'User registered successfully' AS Message,
        UserID,
        FullName,
        Email,
        PhoneNumber,
        ProfileImage,
        Role,
        IsActive,
        CreatedAt
    FROM tbl_Users
    WHERE UserID = SCOPE_IDENTITY();
END
GO

---------sp_User_Login---------
CREATE OR ALTER PROCEDURE sp_User_Login
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        UserID,
        FullName,
        Email,
        PasswordHash,
        PhoneNumber,
        DateOfBirth,
        Address,
        ProfileImage,
        Role,
        IsActive
    FROM tbl_Users
    WHERE Email = @Email;
END
GO


------------sp_User_ForgotPassword---------------
CREATE OR ALTER PROCEDURE sp_User_ForgotPassword
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM tbl_Users WHERE Email = @Email)
    BEGIN
        SELECT 0 AS Success, 'Email not found' AS Message;
        RETURN;
    END

    DECLARE @Otp NVARCHAR(6);
    SET @Otp = RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS VARCHAR(6)), 6);

    UPDATE tbl_Users
    SET 
        OtpCode = @Otp,
        OtpExpiry = DATEADD(MINUTE, 10, GETDATE()),
        UpdatedAt = GETDATE()
    WHERE Email = @Email;

    SELECT 
        1 AS Success,
        'OTP generated successfully' AS Message,
        @Otp AS OtpCode;
END
GO

------------sp_User_VerifyOtp----------------
CREATE OR ALTER PROCEDURE sp_User_VerifyOtp
    @Email NVARCHAR(100),
    @OtpCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM tbl_Users
        WHERE Email = @Email
          AND OtpCode = @OtpCode
          AND OtpExpiry >= GETDATE()
    )
    BEGIN
        UPDATE tbl_Users
        SET
            OtpCode = NULL,
            OtpExpiry = NULL,
            IsOtpVerified = 1,
            OtpVerifiedAt = GETDATE(),
            UpdatedAt = GETDATE()
        WHERE Email = @Email;

        SELECT 1 AS Success, 'OTP verified successfully' AS Message;
    END
    ELSE
    BEGIN
        SELECT 0 AS Success, 'Invalid or expired OTP' AS Message;
    END
END
GO

-----------sp_User_ResetPassword-------------
CREATE OR ALTER PROCEDURE sp_User_ResetPassword
    @Email NVARCHAR(100),
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM tbl_Users
        WHERE Email = @Email
          AND IsOtpVerified = 1
          AND OtpVerifiedAt >= DATEADD(MINUTE, -5, GETDATE())
    )
    BEGIN
        SELECT 0 AS Success, 'OTP not verified or verification expired' AS Message;
        RETURN;
    END

    UPDATE tbl_Users
    SET
        PasswordHash = @NewPasswordHash,
        IsOtpVerified = 0,
        OtpVerifiedAt = NULL,
        UpdatedAt = GETDATE()
    WHERE Email = @Email;

    SELECT 1 AS Success, 'Password reset successfully' AS Message;
END
GO

-------------sp_User_GetProfile---------------
CREATE OR ALTER PROCEDURE sp_User_GetProfile
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        UserID,
        FullName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        ProfileImage,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM tbl_Users
    WHERE UserID = @UserID;
END
GO

-------------sp_User_UpdateProfile--------------
CREATE OR ALTER PROCEDURE sp_User_UpdateProfile
    @UserID INT,
    @FullName NVARCHAR(100) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @DateOfBirth DATE = NULL,
    @Address NVARCHAR(250) = NULL,
    @ProfileImage NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM tbl_Users WHERE UserID = @UserID)
    BEGIN
        SELECT 0 AS Success, 'User not found' AS Message;
        RETURN;
    END

    UPDATE tbl_Users
    SET
        FullName = ISNULL(@FullName, FullName),
        PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),
        DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth),
        Address = ISNULL(@Address, Address),
        ProfileImage = ISNULL(@ProfileImage, ProfileImage),
        UpdatedAt = GETDATE()
    WHERE UserID = @UserID;

    SELECT 
        1 AS Success,
        'Profile updated successfully' AS Message;

    SELECT 
        UserID,
        FullName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        ProfileImage,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM tbl_Users
    WHERE UserID = @UserID;
END
GO