-- Safety check: stop if this database already has data
IF EXISTS (SELECT 1 FROM Roles)
BEGIN
    RAISERROR('This database already has data. Do not run this seed script on it.', 16, 1);
    RETURN;
END


-- GSTFiling test data (Design B). Run once on a new, empty database.
INSERT INTO Roles (Name) VALUES ('Admin'), ('Centre'), ('Consultant'), ('Shop');

INSERT INTO Centres (Name, IsActive, ContactName, Email, MobileNumber, Address, UpdatedAt)
VALUES ('Kochi Centre',   1, 'Test Owner 1', 'kochi@test.com',   '9000000001', 'Kochi',   GETDATE()),
       ('Calicut Centre', 1, 'Test Owner 2', 'calicut@test.com', '9000000002', 'Calicut', GETDATE());

INSERT INTO Consultants (Name, Email, MobileNumber, IsActive, CentreId, UpdatedAt)
VALUES ('Anu Kochi',    'anu@test.com',  '9100000001', 1, (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'),   GETDATE()),
       ('Biju Kochi',   'biju@test.com', '9100000002', 1, (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'),   GETDATE()),
       ('Cini Calicut', 'cini@test.com', '9100000003', 1, (SELECT Id FROM Centres WHERE Name = 'Calicut Centre'), GETDATE());

INSERT INTO Customers (ShopName, Name, GstNumber, Email, MobileNumber, CentreId, ConsultantId, UpdatedAt)
VALUES ('Kochi Mart',    'Raj',     '32ABCDE1234F1Z5', 'mart@test.com',    '9200000001', (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'),   (SELECT Id FROM Consultants WHERE Email = 'anu@test.com'),  GETDATE()),
       ('Kochi Bakes',   'Meera',   '32ABCDE1234F2Z5', 'bakes@test.com',   '9200000002', (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'),   (SELECT Id FROM Consultants WHERE Email = 'biju@test.com'), GETDATE()),
       ('Kochi Mobiles', 'Arun',    '32ABCDE1234F3Z5', 'mobiles@test.com', '9200000003', (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'),   NULL,                                                       GETDATE()),
       ('Calicut Store', 'Fathima', '32ABCDE1234F4Z5', 'store@test.com',   '9200000004', (SELECT Id FROM Centres WHERE Name = 'Calicut Centre'), (SELECT Id FROM Consultants WHERE Email = 'cini@test.com'), GETDATE());

-- Logins: one Users row per account
INSERT INTO Users (UserName, Email, Password, RoleId, IsActive, CentreId, ConsultantId, ShopId, UpdatedAt)
SELECT 'Admin', 'admin@gmail.com', '1234', (SELECT Id FROM Roles WHERE Name = 'Admin'), 1, NULL, NULL, NULL, CONVERT(nvarchar(50), GETDATE(), 120)
UNION ALL
SELECT 'Kochi Centre', 'kochi@test.com', 'centre123', (SELECT Id FROM Roles WHERE Name = 'Centre'), 1, (SELECT Id FROM Centres WHERE Name = 'Kochi Centre'), NULL, NULL, CONVERT(nvarchar(50), GETDATE(), 120)
UNION ALL
SELECT c.Name, c.Email, 'test', (SELECT Id FROM Roles WHERE Name = 'Consultant'), c.IsActive, c.CentreId, c.Id, NULL, CONVERT(nvarchar(50), GETDATE(), 120)
FROM Consultants c
UNION ALL
SELECT s.ShopName, s.Email, 'test', (SELECT Id FROM Roles WHERE Name = 'Shop'), 1, s.CentreId, NULL, s.Id, CONVERT(nvarchar(50), GETDATE(), 120)
FROM Customers s;