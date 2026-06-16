-- 001_SeedData.sql
-- Custom SQL Seed Data Script for SalesCRM

START TRANSACTION;

-- 1. Seed Roles if not exist
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
VALUES 
('d60a5e8c-8f1e-4cb8-8c10-534bc7e4cbb5', 'Admin', 'ADMIN', 'd60a5e8c-8f1e-4cb8-8c10-534bc7e4cbb5', 'Vai trò Admin')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
VALUES 
('ef034b26-e17f-43fb-bc28-98e1245be5ab', 'Manager', 'MANAGER', 'ef034b26-e17f-43fb-bc28-98e1245be5ab', 'Vai trò Manager')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
VALUES 
('3ab10e9f-7db9-467a-9a99-8d77f3a8b273', 'Staff', 'STAFF', '3ab10e9f-7db9-467a-9a99-8d77f3a8b273', 'Vai trò Staff')
ON CONFLICT ("Id") DO NOTHING;

-- 2. Seed Branches if not exist
INSERT INTO "Branches" ("Id", "Name", "Address", "Phone", "IsActive", "CreatedAt", "CreatedBy")
VALUES 
('a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a', 'Chi nhánh Trung tâm', '123 Đường Láng, Hà Nội', '0987654321', true, NOW(), 'System')
ON CONFLICT ("Id") DO NOTHING;

-- 3. Seed Admin User if not exist
INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "FullName", "IsActive", "CreatedAt", "BranchId")
VALUES 
('c4a11b11-2c2c-4d2d-8e2e-4f2f2a2a2a2a', 'admin', 'ADMIN', 'admin@salescrm.com', 'ADMIN@SALESCRM.COM', true, '{{ADMIN_PASSWORD_HASH}}', 'c4a11b11-2c2c-4d2d-8e2e-4f2f2a2a2a2a', 'c4a11b11-2c2c-4d2d-8e2e-4f2f2a2a2a2a', false, false, true, 0, 'System Administrator', true, NOW(), NULL)
ON CONFLICT ("Id") DO NOTHING;

-- 4. Seed Staff User if not exist
INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "FullName", "IsActive", "CreatedAt", "BranchId")
VALUES 
('b3a11c11-3c3c-4d3d-8e3e-4f3f3a3a3a3a', 'staff', 'STAFF', 'staff@salescrm.com', 'STAFF@SALESCRM.COM', true, '{{STAFF_PASSWORD_HASH}}', 'b3a11c11-3c3c-4d3d-8e3e-4f3f3a3a3a3a', 'b3a11c11-3c3c-4d3d-8e3e-4f3f3a3a3a3a', false, false, true, 0, 'Nhân viên Chi Nhánh 1', true, NOW(), 'a8b2a1a8-c2b2-4d2a-8d2f-4e2e2a2a2a2a')
ON CONFLICT ("Id") DO NOTHING;

-- 5. Link Admin to Admin Role
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES 
('c4a11b11-2c2c-4d2d-8e2e-4f2f2a2a2a2a', 'd60a5e8c-8f1e-4cb8-8c10-534bc7e4cbb5')
ON CONFLICT ("UserId", "RoleId") DO NOTHING;

-- 6. Link Staff to Staff Role
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES 
('b3a11c11-3c3c-4d3d-8e3e-4f3f3a3a3a3a', '3ab10e9f-7db9-467a-9a99-8d77f3a8b273')
ON CONFLICT ("UserId", "RoleId") DO NOTHING;

COMMIT;
