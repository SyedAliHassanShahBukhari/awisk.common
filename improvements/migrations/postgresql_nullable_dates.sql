-- ============================================================
-- Migration: awisk.Common 2.0.13 -> 2.0.14
-- Change:    UpdatedOn / DeletedOn columns become nullable
-- Database:  PostgreSQL
-- Run once per table that inherits from BaseEntity, BaseGuidEntity,
--            BaseIntEntity, or BaseLongEntity.
-- Replace "YourTable" with each actual table name.
-- ============================================================

-- Step 1: Drop NOT NULL constraint (PostgreSQL uses ALTER COLUMN ... DROP NOT NULL)
ALTER TABLE "YourTable" ALTER COLUMN "UpdatedOn" DROP NOT NULL;
ALTER TABLE "YourTable" ALTER COLUMN "DeletedOn" DROP NOT NULL;

-- Step 2: Convert sentinel 1900-01-01 values to NULL
UPDATE "YourTable" SET "UpdatedOn" = NULL WHERE "UpdatedOn" = '1900-01-01 00:00:00';
UPDATE "YourTable" SET "DeletedOn" = NULL WHERE "DeletedOn" = '1900-01-01 00:00:00';

-- ============================================================
-- Example for multiple tables in one script:
-- ============================================================
-- ALTER TABLE "Orders"      ALTER COLUMN "UpdatedOn" DROP NOT NULL;
-- ALTER TABLE "Orders"      ALTER COLUMN "DeletedOn" DROP NOT NULL;
-- UPDATE "Orders" SET "UpdatedOn" = NULL WHERE "UpdatedOn" = '1900-01-01 00:00:00';
-- UPDATE "Orders" SET "DeletedOn" = NULL WHERE "DeletedOn" = '1900-01-01 00:00:00';
--
-- ALTER TABLE "OrderLines"  ALTER COLUMN "UpdatedOn" DROP NOT NULL;
-- ALTER TABLE "OrderLines"  ALTER COLUMN "DeletedOn" DROP NOT NULL;
-- UPDATE "OrderLines" SET "UpdatedOn" = NULL WHERE "UpdatedOn" = '1900-01-01 00:00:00';
-- UPDATE "OrderLines" SET "DeletedOn" = NULL WHERE "DeletedOn" = '1900-01-01 00:00:00';
