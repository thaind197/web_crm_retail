-- 002_AddProductCodeUniqueIndex.sql
-- Add unique index on Products.Code to fix ON CONFLICT resolution

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Products_Code" ON "Products" ("Code");
