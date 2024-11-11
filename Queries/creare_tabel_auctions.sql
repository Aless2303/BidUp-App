CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    ProductImagePath NVARCHAR(MAX),
    Category NVARCHAR(50),
    CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
    SellerID INT NOT NULL,
    FOREIGN KEY (SellerID) REFERENCES Users(UserID)
);

CREATE TABLE Auctions (
    AuctionID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    ProductImagePath NVARCHAR(MAX),
    StartingPrice FLOAT NOT NULL,
    CurrentPrice FLOAT NOT NULL DEFAULT 0,
    CurrentBidderID INT NULL,
    SellerID INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    IsClosed BIT DEFAULT 0,
    FOREIGN KEY (SellerID) REFERENCES Users(UserID),
    FOREIGN KEY (CurrentBidderID) REFERENCES Users(UserID)
);

ALTER TABLE Auctions
ADD ProductID INT NOT NULL,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID);


