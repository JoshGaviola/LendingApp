/*
SQLyog Community v13.3.0 (64 bit)
MySQL - 8.0.43 : Database - lendingapp
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`lendingapp` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `lendingapp`;

/*Table structure for table `audit_log` */

DROP TABLE IF EXISTS `audit_log`;

CREATE TABLE `audit_log` (
  `log_id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `action` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `table_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `record_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `old_values` json DEFAULT NULL,
  `new_values` json DEFAULT NULL,
  `ip_address` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`log_id`),
  KEY `idx_audit_user` (`user_id`),
  KEY `idx_audit_action` (`action`),
  KEY `idx_audit_timestamp` (`timestamp`),
  CONSTRAINT `fk_audit_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `audit_log` */

/*Table structure for table `collections` */

DROP TABLE IF EXISTS `collections`;

CREATE TABLE `collections` (
  `collection_id` int NOT NULL AUTO_INCREMENT,
  `loan_id` int NOT NULL,
  `customer_id` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL,
  `due_date` date NOT NULL,
  `amount_due` decimal(18,2) NOT NULL,
  `days_overdue` int NOT NULL DEFAULT '0',
  `priority` enum('Low','Medium','High','Critical') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Medium',
  `status` enum('Pending','Contacted','PromiseToPay','Paid','Escalated') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `last_contact_date` date DEFAULT NULL,
  `last_contact_method` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `notes` text COLLATE utf8mb4_unicode_ci,
  `promise_date` date DEFAULT NULL,
  `assigned_officer_id` int DEFAULT NULL,
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`collection_id`),
  KEY `idx_collections_loan` (`loan_id`),
  KEY `idx_collections_status` (`status`),
  KEY `idx_collections_priority` (`priority`),
  KEY `idx_collections_officer` (`assigned_officer_id`),
  KEY `fk_collections_customer` (`customer_id`),
  CONSTRAINT `fk_collections_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `fk_collections_loan` FOREIGN KEY (`loan_id`) REFERENCES `loans` (`loan_id`),
  CONSTRAINT `fk_collections_officer` FOREIGN KEY (`assigned_officer_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `collections` */

/*Table structure for table `customers` */

DROP TABLE IF EXISTS `customers`;

CREATE TABLE `customers` (
  `customer_id` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL,
  `first_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `last_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `middle_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `gender` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `civil_status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nationality` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT 'Filipino',
  `email_address` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mobile_number` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telephone_number` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `present_address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `permanent_address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `city` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `province` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `zip_code` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `sss_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tin_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `passport_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `drivers_license_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `umid_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `philhealth_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `pagibig_number` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `employment_status` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `company_name` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `position` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `department` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `company_address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `company_phone` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `bank_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `bank_account_number` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `initial_credit_score` int NOT NULL DEFAULT '0',
  `credit_limit` decimal(18,2) NOT NULL DEFAULT '0.00',
  `emergency_contact_name` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emergency_contact_relationship` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emergency_contact_number` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emergency_contact_address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `customer_type` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'New',
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Active',
  `registration_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` int DEFAULT NULL,
  `last_modified_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `last_modified_by` int DEFAULT NULL,
  `remarks` text COLLATE utf8mb4_unicode_ci,
  `valid_id1_path` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `valid_id2_path` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `proof_of_income_path` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `proof_of_address_path` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `signature_image_path` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`customer_id`),
  KEY `idx_customers_name` (`last_name`,`first_name`),
  KEY `idx_customers_mobile` (`mobile_number`),
  KEY `idx_customers_email` (`email_address`),
  KEY `idx_customers_status` (`status`),
  KEY `fk_customers_created_by` (`created_by`),
  KEY `fk_customers_modified_by` (`last_modified_by`),
  CONSTRAINT `fk_customers_created_by` FOREIGN KEY (`created_by`) REFERENCES `users` (`user_id`),
  CONSTRAINT `fk_customers_modified_by` FOREIGN KEY (`last_modified_by`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `customers` */

/*Table structure for table `loan_applications` */

DROP TABLE IF EXISTS `loan_applications`;

CREATE TABLE `loan_applications` (
  `application_id` int NOT NULL AUTO_INCREMENT,
  `application_number` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `customer_id` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_id` int NOT NULL,
  `requested_amount` decimal(18,2) NOT NULL,
  `preferred_term` int NOT NULL,
  `purpose` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `desired_release_date` date DEFAULT NULL,
  `status` enum('Pending','Review','Approved','Rejected','Released','Cancelled') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `priority` enum('Low','Medium','High','Critical') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Medium',
  `rejection_reason` text COLLATE utf8mb4_unicode_ci,
  `application_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `approved_date` datetime DEFAULT NULL,
  `assigned_officer_id` int DEFAULT NULL,
  `approved_by` int DEFAULT NULL,
  PRIMARY KEY (`application_id`),
  UNIQUE KEY `application_number` (`application_number`),
  KEY `idx_applications_customer` (`customer_id`),
  KEY `idx_applications_status` (`status`),
  KEY `idx_applications_date` (`application_date`),
  KEY `idx_applications_officer` (`assigned_officer_id`),
  KEY `fk_applications_product` (`product_id`),
  KEY `fk_applications_approved_by` (`approved_by`),
  CONSTRAINT `fk_applications_approved_by` FOREIGN KEY (`approved_by`) REFERENCES `users` (`user_id`),
  CONSTRAINT `fk_applications_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `fk_applications_officer` FOREIGN KEY (`assigned_officer_id`) REFERENCES `users` (`user_id`),
  CONSTRAINT `fk_applications_product` FOREIGN KEY (`product_id`) REFERENCES `loan_products` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `loan_applications` */

/*Table structure for table `loan_products` */

DROP TABLE IF EXISTS `loan_products`;

CREATE TABLE `loan_products` (
  `product_id` int NOT NULL AUTO_INCREMENT,
  `product_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `min_amount` decimal(18,2) NOT NULL DEFAULT '5000.00',
  `max_amount` decimal(18,2) NOT NULL DEFAULT '500000.00',
  `min_term_months` int NOT NULL DEFAULT '6',
  `max_term_months` int NOT NULL DEFAULT '36',
  `interest_rate` decimal(5,2) NOT NULL DEFAULT '12.00',
  `processing_fee_pct` decimal(5,2) NOT NULL DEFAULT '3.00',
  `penalty_rate` decimal(5,2) NOT NULL DEFAULT '2.00',
  `grace_period_days` int NOT NULL DEFAULT '7',
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `product_name` (`product_name`),
  KEY `idx_loan_products_active` (`is_active`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `loan_products` */

insert  into `loan_products`(`product_id`,`product_name`,`description`,`min_amount`,`max_amount`,`min_term_months`,`max_term_months`,`interest_rate`,`processing_fee_pct`,`penalty_rate`,`grace_period_days`,`is_active`,`created_date`) values 
(1,'Personal Loan','General purpose personal loan',10000.00,200000.00,6,24,12.00,3.00,2.00,7,1,'2025-12-31 23:51:12'),
(2,'Salary Loan','Loan against monthly salary',5000.00,100000.00,3,12,10.00,2.50,2.00,7,1,'2025-12-31 23:51:12'),
(3,'Emergency Loan','Quick disbursement emergency loan',5000.00,50000.00,3,6,15.00,2.00,2.00,7,1,'2025-12-31 23:51:12'),
(4,'Business Loan','Small business financing',50000.00,500000.00,12,36,14.00,3.50,2.00,7,1,'2025-12-31 23:51:12');

/*Table structure for table `loans` */

DROP TABLE IF EXISTS `loans`;

CREATE TABLE `loans` (
  `loan_id` int NOT NULL AUTO_INCREMENT,
  `loan_number` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `application_id` int NOT NULL,
  `customer_id` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_id` int NOT NULL,
  `principal_amount` decimal(18,2) NOT NULL,
  `interest_rate` decimal(5,2) NOT NULL,
  `term_months` int NOT NULL,
  `monthly_payment` decimal(18,2) NOT NULL,
  `processing_fee` decimal(18,2) NOT NULL DEFAULT '0.00',
  `total_payable` decimal(18,2) NOT NULL,
  `outstanding_balance` decimal(18,2) NOT NULL,
  `total_paid` decimal(18,2) NOT NULL DEFAULT '0.00',
  `total_interest_paid` decimal(18,2) NOT NULL DEFAULT '0.00',
  `total_penalty_paid` decimal(18,2) NOT NULL DEFAULT '0.00',
  `status` enum('Active','Paid','Defaulted','Restructured','WrittenOff') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Active',
  `days_overdue` int NOT NULL DEFAULT '0',
  `release_date` date NOT NULL,
  `first_due_date` date NOT NULL,
  `next_due_date` date DEFAULT NULL,
  `maturity_date` date NOT NULL,
  `last_payment_date` date DEFAULT NULL,
  `release_mode` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `released_by` int DEFAULT NULL,
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_updated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`loan_id`),
  UNIQUE KEY `loan_number` (`loan_number`),
  UNIQUE KEY `application_id` (`application_id`),
  KEY `idx_loans_customer` (`customer_id`),
  KEY `idx_loans_status` (`status`),
  KEY `idx_loans_due_date` (`next_due_date`),
  KEY `idx_loans_overdue` (`days_overdue`),
  KEY `fk_loans_product` (`product_id`),
  KEY `fk_loans_released_by` (`released_by`),
  CONSTRAINT `fk_loans_application` FOREIGN KEY (`application_id`) REFERENCES `loan_applications` (`application_id`),
  CONSTRAINT `fk_loans_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `fk_loans_product` FOREIGN KEY (`product_id`) REFERENCES `loan_products` (`product_id`),
  CONSTRAINT `fk_loans_released_by` FOREIGN KEY (`released_by`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `loans` */

/*Table structure for table `payments` */

DROP TABLE IF EXISTS `payments`;

CREATE TABLE `payments` (
  `payment_id` int NOT NULL AUTO_INCREMENT,
  `receipt_number` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `loan_id` int NOT NULL,
  `customer_id` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL,
  `amount` decimal(18,2) NOT NULL,
  `interest_portion` decimal(18,2) NOT NULL DEFAULT '0.00',
  `principal_portion` decimal(18,2) NOT NULL DEFAULT '0.00',
  `penalty_portion` decimal(18,2) NOT NULL DEFAULT '0.00',
  `balance_after` decimal(18,2) NOT NULL,
  `payment_method` enum('Cash','GCash','Bank','Check') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Cash',
  `reference_number` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `remarks` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `payment_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `processed_by` int NOT NULL,
  PRIMARY KEY (`payment_id`),
  UNIQUE KEY `receipt_number` (`receipt_number`),
  KEY `idx_payments_loan` (`loan_id`),
  KEY `idx_payments_customer` (`customer_id`),
  KEY `idx_payments_date` (`payment_date`),
  KEY `idx_payments_receipt` (`receipt_number`),
  KEY `fk_payments_processed_by` (`processed_by`),
  CONSTRAINT `fk_payments_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `fk_payments_loan` FOREIGN KEY (`loan_id`) REFERENCES `loans` (`loan_id`),
  CONSTRAINT `fk_payments_processed_by` FOREIGN KEY (`processed_by`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `payments` */

/*Table structure for table `system_settings` */

DROP TABLE IF EXISTS `system_settings`;

CREATE TABLE `system_settings` (
  `setting_id` int NOT NULL AUTO_INCREMENT,
  `setting_key` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `setting_value` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `last_modified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `modified_by` int DEFAULT NULL,
  PRIMARY KEY (`setting_id`),
  UNIQUE KEY `setting_key` (`setting_key`),
  KEY `fk_settings_modified_by` (`modified_by`),
  CONSTRAINT `fk_settings_modified_by` FOREIGN KEY (`modified_by`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `system_settings` */

/*Table structure for table `tasks` */

DROP TABLE IF EXISTS `tasks`;

CREATE TABLE `tasks` (
  `task_id` int NOT NULL AUTO_INCREMENT,
  `assigned_to` int NOT NULL,
  `loan_id` int DEFAULT NULL,
  `customer_id` varchar(32) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `task_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `scheduled_time` datetime NOT NULL,
  `status` enum('Pending','InProgress','Completed','Cancelled') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `completed_date` datetime DEFAULT NULL,
  PRIMARY KEY (`task_id`),
  KEY `idx_tasks_assigned` (`assigned_to`),
  KEY `idx_tasks_date` (`scheduled_time`),
  KEY `idx_tasks_status` (`status`),
  KEY `fk_tasks_loan` (`loan_id`),
  KEY `fk_tasks_customer` (`customer_id`),
  CONSTRAINT `fk_tasks_assigned_to` FOREIGN KEY (`assigned_to`) REFERENCES `users` (`user_id`),
  CONSTRAINT `fk_tasks_customer` FOREIGN KEY (`customer_id`) REFERENCES `customers` (`customer_id`),
  CONSTRAINT `fk_tasks_loan` FOREIGN KEY (`loan_id`) REFERENCES `loans` (`loan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `tasks` */

/*Table structure for table `users` */

DROP TABLE IF EXISTS `users`;

CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `first_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `last_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `role` enum('Admin','LoanOfficer','Cashier') COLLATE utf8mb4_unicode_ci NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_login` datetime DEFAULT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `username` (`username`),
  KEY `idx_users_username` (`username`),
  KEY `idx_users_role` (`role`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*Data for the table `users` */

insert  into `users`(`user_id`,`username`,`password_hash`,`email`,`first_name`,`last_name`,`role`,`is_active`,`created_date`,`last_login`) values 
(1,'admin','CHANGE_THIS_HASH',NULL,'System','Administrator','Admin',1,'2025-12-31 23:51:12',NULL);

/*Table structure for table `loan_application_evaluations` */

DROP TABLE IF EXISTS `loan_application_evaluations`;

CREATE TABLE `loan_application_evaluations` (
  `evaluation_id` BIGINT NOT NULL AUTO_INCREMENT,

  `application_id` INT NOT NULL,

  `c1_input` DECIMAL(5,2) NOT NULL,
  `c2_input` DECIMAL(5,2) NOT NULL,
  `c3_input` DECIMAL(5,2) NOT NULL,
  `c4_input` DECIMAL(5,2) NOT NULL,

  `w1_pct` DECIMAL(5,2) NOT NULL,
  `w2_pct` DECIMAL(5,2) NOT NULL,
  `w3_pct` DECIMAL(5,2) NOT NULL,
  `w4_pct` DECIMAL(5,2) NOT NULL,

  `c1_weighted` DECIMAL(6,2) NOT NULL,
  `c2_weighted` DECIMAL(6,2) NOT NULL,
  `c3_weighted` DECIMAL(6,2) NOT NULL,
  `c4_weighted` DECIMAL(6,2) NOT NULL,
  `total_score`  DECIMAL(6,2) NOT NULL,

  `decision` ENUM('Approve','Approve with Conditions','Reject') NOT NULL,

  `interest_method` ENUM('Diminishing Balance','Flat Rate','Add-on Rate') DEFAULT NULL,
  `interest_rate_pct` DECIMAL(6,3) DEFAULT NULL,
  `service_fee_pct`   DECIMAL(6,3) DEFAULT NULL,
  `term_months`       INT DEFAULT NULL,

  `approval_level` ENUM('Level 1','Level 2','Level 3') DEFAULT NULL,

  `require_comaker`        TINYINT(1) NOT NULL DEFAULT 0,
  `reduce_amount`          TINYINT(1) NOT NULL DEFAULT 0,
  `shorten_term`           TINYINT(1) NOT NULL DEFAULT 0,
  `additional_collateral`  TINYINT(1) NOT NULL DEFAULT 0,

  `rejection_reason` VARCHAR(150) DEFAULT NULL,
  `remarks` TEXT DEFAULT NULL,

  `evaluated_by` INT DEFAULT NULL,
  `status_after` ENUM('Pending','Review','Approved','Rejected','Released','Cancelled') DEFAULT NULL,

  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (`evaluation_id`),

  KEY `idx_eval_application_id` (`application_id`),
  KEY `idx_eval_created_at` (`created_at`),
  KEY `idx_eval_decision` (`decision`),

  CONSTRAINT `fk_eval_application`
    FOREIGN KEY (`application_id`)
    REFERENCES `loan_applications` (`application_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,

  CONSTRAINT `fk_eval_user`
    FOREIGN KEY (`evaluated_by`)
    REFERENCES `users` (`user_id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/* ============================================================
   Schema Expansion: Loan Product Config + Loan Rules
   ============================================================ */

ALTER TABLE `loan_products`
  ADD COLUMN `interest_type` ENUM('Fixed','Variable')
      COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Fixed' AFTER `interest_rate`,
  ADD COLUMN `interest_period` ENUM('Month','Year')
      COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Year' AFTER `interest_type`,
  ADD COLUMN `service_fee_fixed_amount` DECIMAL(18,2) NULL DEFAULT NULL AFTER `processing_fee_pct`,
  ADD COLUMN `penalty_period` ENUM('Day','Week','Month')
      COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Month' AFTER `penalty_rate`,
  ADD COLUMN `requires_collateral` TINYINT(1) NOT NULL DEFAULT 0 AFTER `grace_period_days`;

DROP TABLE IF EXISTS `loan_product_terms`;
CREATE TABLE `loan_product_terms` (
  `product_id` INT NOT NULL,
  `term_months` INT NOT NULL,
  PRIMARY KEY (`product_id`, `term_months`),
  KEY `idx_product_terms_term` (`term_months`),
  CONSTRAINT `fk_product_terms_product`
    FOREIGN KEY (`product_id`)
    REFERENCES `loan_products` (`product_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `loan_product_requirements`;
CREATE TABLE `loan_product_requirements` (
  `requirement_id` INT NOT NULL AUTO_INCREMENT,
  `product_id` INT NOT NULL,
  `requirement_key` VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `requirement_text` VARCHAR(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_required` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`requirement_id`),
  UNIQUE KEY `uq_product_requirement` (`product_id`, `requirement_key`),
  KEY `idx_req_product` (`product_id`),
  CONSTRAINT `fk_product_requirements_product`
    FOREIGN KEY (`product_id`)
    REFERENCES `loan_products` (`product_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DROP TABLE IF EXISTS `loan_rules`;
CREATE TABLE `loan_rules` (
  `rules_id` INT NOT NULL AUTO_INCREMENT,
  `product_id` INT NOT NULL,

  `interest_method` ENUM('Diminishing Balance','Flat Rate','Add-On Rate','Compound Interest')
      COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Diminishing Balance',

  `payment_history_weight_pct` DECIMAL(5,2) NOT NULL DEFAULT 35.00,
  `payment_history_min_score`  DECIMAL(5,2) NOT NULL DEFAULT 60.00,

  `credit_util_weight_pct`     DECIMAL(5,2) NOT NULL DEFAULT 30.00,
  `credit_util_min_score`      DECIMAL(5,2) NOT NULL DEFAULT 50.00,

  `length_history_weight_pct`  DECIMAL(5,2) NOT NULL DEFAULT 15.00,
  `length_history_min_score`   DECIMAL(5,2) NOT NULL DEFAULT 40.00,

  `income_stability_weight_pct` DECIMAL(5,2) NOT NULL DEFAULT 20.00,
  `income_stability_min_score`  DECIMAL(5,2) NOT NULL DEFAULT 55.00,

  `min_approval_score` DECIMAL(5,2) NOT NULL DEFAULT 70.00,

  `approval_workflow` ENUM('Loan Officer','Officer + Supervisor','Officer + Supervisor + Manager')
      COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Loan Officer',

  `penalty_rate_pct` DECIMAL(6,3) NOT NULL DEFAULT 0.500,
  `apply_after_grace_period` TINYINT(1) NOT NULL DEFAULT 1,
  `penalty_cap_pct_of_principal` DECIMAL(6,2) NOT NULL DEFAULT 10.00,

  `max_restructures` INT NOT NULL DEFAULT 3,
  `min_days_delayed_for_restructure` INT NOT NULL DEFAULT 30,

  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (`rules_id`),
  UNIQUE KEY `uq_rules_product` (`product_id`),
  KEY `idx_rules_product` (`product_id`),

  CONSTRAINT `fk_rules_product`
    FOREIGN KEY (`product_id`)
    REFERENCES `loan_products` (`product_id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `loan_rules` (`product_id`)
SELECT p.`product_id`
FROM `loan_products` p
LEFT JOIN `loan_rules` r ON r.`product_id` = p.`product_id`
WHERE r.`product_id` IS NULL;

SET FOREIGN_KEY_CHECKS = 1;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
