/* =========================================================
   06_offer_package_module.sql
   SmartStay - offer & package Module
   ========================================================= */

/* =========================
   TABLE: tbl_Offers
   ========================= */
GO
CREATE TABLE tbl_Offers
(
    OfferId INT IDENTITY(1,1) PRIMARY KEY,

    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,

    DiscountType NVARCHAR(20) NOT NULL, -- Percentage / Flat
    DiscountValue DECIMAL(10,2) NOT NULL,

    HotelId INT NULL,
    RoomId INT NULL,

    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (HotelId) REFERENCES tbl_Hotels(HotelId),
    FOREIGN KEY (RoomId) REFERENCES tbl_Rooms(RoomId)
);


/* =========================
   TABLE: tbl_Packages
   ========================= */
GO
CREATE TABLE tbl_Packages
(
    PackageId INT IDENTITY(1,1) PRIMARY KEY,

    HotelId INT NOT NULL,

    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,

    Price DECIMAL(10,2) NOT NULL,

    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (HotelId) REFERENCES tbl_Hotels(HotelId)
);


/* =========================
   SP: create offer
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Offer_Create
(
    @Title NVARCHAR(150),
    @Description NVARCHAR(500),
    @DiscountType NVARCHAR(20),
    @DiscountValue DECIMAL(10,2),
    @HotelId INT = NULL,
    @RoomId INT = NULL,
    @StartDate DATE,
    @EndDate DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @DiscountType NOT IN ('Percentage','Flat')
    BEGIN
        SELECT 0 AS Success, 'Invalid discount type' AS Message;
        RETURN;
    END

    INSERT INTO tbl_Offers
    (Title, Description, DiscountType, DiscountValue, HotelId, RoomId, StartDate, EndDate)
    VALUES
    (@Title, @Description, @DiscountType, @DiscountValue, @HotelId, @RoomId, @StartDate, @EndDate);

    SELECT 1 AS Success, 'Offer created successfully' AS Message;
END;


/* =========================
   SP: get active offer
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Offer_GetActive
AS
BEGIN
    SELECT *
    FROM tbl_Offers
    WHERE IsActive = 1
    AND CAST(GETDATE() AS DATE) BETWEEN StartDate AND EndDate
    ORDER BY CreatedAt DESC;
END;


/* =========================
   SP: create package
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Package_Create
(
    @HotelId INT,
    @Title NVARCHAR(150),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @StartDate DATE,
    @EndDate DATE
)
AS
BEGIN
    INSERT INTO tbl_Packages
    (HotelId, Title, Description, Price, StartDate, EndDate)
    VALUES
    (@HotelId, @Title, @Description, @Price, @StartDate, @EndDate);

    SELECT 1 AS Success, 'Package created successfully' AS Message;
END;


/* =========================
   SP: get package by hotel
   ========================= */
GO
CREATE OR ALTER PROCEDURE sp_Package_GetByHotel
(
    @HotelId INT
)
AS
BEGIN
    SELECT *
    FROM tbl_Packages
    WHERE HotelId = @HotelId
    AND IsActive = 1
    AND CAST(GETDATE() AS DATE) BETWEEN StartDate AND EndDate
END;