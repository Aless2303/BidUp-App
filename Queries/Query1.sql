CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),      -- Auto-incrementing primary key
    FullName NVARCHAR(100) NOT NULL,           -- Full name of the user
    PasswordHash NVARCHAR(255) NOT NULL,       -- Hashed password
    Role NVARCHAR(20) CHECK (Role IN ('Bidder', 'Admin', 'Seller')),  -- Role with constraint
    Email NVARCHAR(100) NOT NULL UNIQUE,       -- Email, unique to prevent duplicates
    BirthDate DATE,                            -- Birth date of the user
    CreatedAt DATETIME DEFAULT GETDATE()       -- Date and time when the account was created
);

INSERT INTO Users (FullName, PasswordHash, Role, Email, BirthDate, CreatedAt)
VALUES ('Florea Gabriel', '88d6483ff8f9181b81583c007b4c859b0b561d328b2a178d22126219017a23a3', 'Bidder', 'flo@example.com', '2004-03-20', GETDATE());

