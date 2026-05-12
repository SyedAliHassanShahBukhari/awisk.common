-- ============================================================
-- Migration: awisk.Common 2.0.13 -> 2.0.14
-- Change:    UpdatedOn / DeletedOn columns become nullable
-- Database:  SQL Server
-- Run once per table that inherits from BaseEntity, BaseGuidEntity,
--            BaseIntEntity, or BaseLongEntity.
-- Replace [YourTable] with each actual table name.
-- ============================================================

-- Step 1: Make columns nullable
ALTER TABLE [YourTable] ALTER COLUMN UpdatedOn DATETIME NULL;
ALTER TABLE [YourTable] ALTER COLUMN DeletedOn DATETIME NULL;

-- Step 2: Convert sentinel 1900-01-01 values to NULL
UPDATE [YourTable] SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01';
UPDATE [YourTable] SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01';

-- ============================================================
-- Example for multiple tables in one script:
-- ============================================================
-- ALTER TABLE [Orders]      ALTER COLUMN UpdatedOn DATETIME NULL;
-- ALTER TABLE [Orders]      ALTER COLUMN DeletedOn DATETIME NULL;
-- UPDATE [Orders] SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01';
-- UPDATE [Orders] SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01';
--
-- ALTER TABLE [OrderLines]  ALTER COLUMN UpdatedOn DATETIME NULL;
-- ALTER TABLE [OrderLines]  ALTER COLUMN DeletedOn DATETIME NULL;
-- UPDATE [OrderLines] SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01';
-- UPDATE [OrderLines] SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01';
