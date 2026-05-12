-- ============================================================
-- Migration: awisk.Common 2.0.13 -> 2.0.14
-- Change:    UpdatedOn / DeletedOn columns become nullable
-- Database:  MySQL
-- Run once per table that inherits from BaseEntity, BaseGuidEntity,
--            BaseIntEntity, or BaseLongEntity.
-- Replace `YourTable` with each actual table name.
-- ============================================================

-- Step 1: Make columns nullable (MySQL uses MODIFY COLUMN)
ALTER TABLE `YourTable` MODIFY COLUMN UpdatedOn DATETIME NULL;
ALTER TABLE `YourTable` MODIFY COLUMN DeletedOn DATETIME NULL;

-- Step 2: Convert sentinel 1900-01-01 values to NULL
UPDATE `YourTable` SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01 00:00:00';
UPDATE `YourTable` SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01 00:00:00';

-- ============================================================
-- Example for multiple tables in one script:
-- ============================================================
-- ALTER TABLE `Orders`      MODIFY COLUMN UpdatedOn DATETIME NULL;
-- ALTER TABLE `Orders`      MODIFY COLUMN DeletedOn DATETIME NULL;
-- UPDATE `Orders` SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01 00:00:00';
-- UPDATE `Orders` SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01 00:00:00';
--
-- ALTER TABLE `OrderLines`  MODIFY COLUMN UpdatedOn DATETIME NULL;
-- ALTER TABLE `OrderLines`  MODIFY COLUMN DeletedOn DATETIME NULL;
-- UPDATE `OrderLines` SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01 00:00:00';
-- UPDATE `OrderLines` SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01 00:00:00';
