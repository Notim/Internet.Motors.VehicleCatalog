-- Criar o banco de dados VEHICLE_CATALOG
CREATE DATABASE VEHICLE_CATALOG;
GO

-- Selecionar o banco de dados recém-criado
USE VEHICLE_CATALOG;
GO

-- Criar ou atualizar a tabela VEHICLE
CREATE TABLE VEHICLE (
    Id INT PRIMARY KEY IDENTITY(1,1),                    -- Numeric unique identifier
    VehicleId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- Unique identifier generated automatically    
    CarName NVARCHAR(100) NOT NULL,                      -- Car name
    Brand NVARCHAR(50) NOT NULL,                         -- Car brand
    Model NVARCHAR(50) NOT NULL,                         -- Car model
    Year INT NOT NULL,                                   -- Car year
    Color NVARCHAR(30) NULL,                             -- Car color
    FuelType NVARCHAR(20) NOT NULL,                      -- Fuel type
    NumberOfDoors INT NOT NULL DEFAULT 4,                -- Number of doors (default 4)
    Mileage DECIMAL(10, 2) NOT NULL,                     -- Car mileage
    Price DECIMAL(12, 2) NULL,                           -- Car price
    ManufacturingDate DATE NULL,                         -- Manufacturing date
    SaleDate DATE NULL,                                  -- Sale date
    SaleStatus NVARCHAR(20) NOT NULL DEFAULT 'Available',-- Sale status (default: 'Available')
    IsReserved BIT NOT NULL                              -- Reserved status (1 = reserved, 0 = not reserved)
);