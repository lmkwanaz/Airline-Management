IF DB_ID('SkyFlowDb') IS NULL
BEGIN
    CREATE DATABASE SkyFlowDb;
END;
GO

USE SkyFlowDb;
GO

IF OBJECT_ID('dbo.Booking', 'U') IS NOT NULL DROP TABLE dbo.Booking;
IF OBJECT_ID('dbo.Passenger', 'U') IS NOT NULL DROP TABLE dbo.Passenger;
IF OBJECT_ID('dbo.Flight', 'U') IS NOT NULL DROP TABLE dbo.Flight;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
GO

CREATE TABLE dbo.[User]
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO

CREATE TABLE dbo.Flight
(
    FlightId INT IDENTITY(1,1) PRIMARY KEY,
    FlightNumber NVARCHAR(10) NOT NULL,
    Origin NVARCHAR(50) NOT NULL,
    Destination NVARCHAR(50) NOT NULL,
    DepartureTime DATETIME2 NOT NULL,
    Capacity INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    CurrentOccupancy INT NOT NULL DEFAULT 0,
    GateAgentId INT NULL,
    CONSTRAINT FK_Flight_GateAgent FOREIGN KEY (GateAgentId) REFERENCES dbo.[User](UserId)
);
GO

CREATE TABLE dbo.Passenger
(
    PassengerId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    FirstName NVARCHAR(80) NOT NULL,
    LastName NVARCHAR(80) NOT NULL,
    PassportNumber NVARCHAR(20) NOT NULL UNIQUE,
    DateOfBirth DATE NULL,
    Nationality NVARCHAR(50) NULL,
    ContactNumber NVARCHAR(20) NOT NULL,
    CONSTRAINT FK_Passenger_User FOREIGN KEY (UserId) REFERENCES dbo.[User](UserId)
);
GO

CREATE TABLE dbo.Booking
(
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    FlightId INT NOT NULL,
    PassengerId INT NOT NULL,
    SeatNumber NVARCHAR(10) NOT NULL,
    BookingStatus NVARCHAR(20) NOT NULL,
    BookingDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CheckInTime DATETIME2 NULL,
    BoardingTime DATETIME2 NULL,
    CONSTRAINT FK_Booking_Flight FOREIGN KEY (FlightId) REFERENCES dbo.Flight(FlightId),
    CONSTRAINT FK_Booking_Passenger FOREIGN KEY (PassengerId) REFERENCES dbo.Passenger(PassengerId)
);
GO

INSERT INTO dbo.[User] (Username, PasswordHash, Role, Email, FirstName, LastName)
VALUES
('admin', 'admin123', 'Administrator', 'admin@skyflow.local', 'Avery', 'Mokoena'),
('gate1', 'gate123', 'GateAgent', 'gate1@skyflow.local', 'Lebo', 'Khumalo');

INSERT INTO dbo.Flight (FlightNumber, Origin, Destination, DepartureTime, Capacity, Status, CurrentOccupancy, GateAgentId)
VALUES
('SF102', 'CPT', 'JHB', DATEADD(HOUR, 3, SYSDATETIME()), 180, 'Boarding', 1, 2),
('SF221', 'DBN', 'CPT', DATEADD(HOUR, 5, SYSDATETIME()), 150, 'Scheduled', 0, 2),
('SF305', 'JHB', 'PLZ', DATEADD(HOUR, 7, SYSDATETIME()), 120, 'Departed', 120, 2);

INSERT INTO dbo.Passenger (FirstName, LastName, PassportNumber, DateOfBirth, Nationality, ContactNumber)
VALUES
('Neroshen', 'Govender', 'PASSPORT12345', '1998-03-12', 'South African', '+27820001001'),
('Aisha', 'Khan', 'PASSPORT99887', '1995-11-25', 'Pakistani', '+27820001002'),
('Liam', 'Okafor', 'PASSPORT11223', '1991-06-18', 'Nigerian', '+27820001003');

INSERT INTO dbo.Booking (FlightId, PassengerId, SeatNumber, BookingStatus, CheckInTime, BoardingTime)
VALUES
(1, 1, '12A', 'CheckedIn', DATEADD(MINUTE, -40, SYSDATETIME()), NULL),
(1, 2, '12B', 'Booked', NULL, NULL),
(2, 3, '18C', 'Booked', NULL, NULL);

UPDATE f
SET f.CurrentOccupancy = x.Occupancy
FROM dbo.Flight f
INNER JOIN
(
    SELECT FlightId, COUNT(*) AS Occupancy
    FROM dbo.Booking
    WHERE BookingStatus IN ('CheckedIn', 'Boarded')
    GROUP BY FlightId
) x ON x.FlightId = f.FlightId;
GO
