CREATE DATABASE  IF NOT EXISTS `account_zf` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `account_zf`;
-- MySQL dump 10.13  Distrib 8.0.24, for Win64 (x86_64)
--
-- Host: localhost    Database: account_zf
-- ------------------------------------------------------
-- Server version	5.7.34-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account` (
  `AccountID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Username` varchar(16) NOT NULL,
  `Password` varchar(70) NOT NULL,
  `Salt` varchar(45) DEFAULT NULL,
  `AuthorityID` smallint(6) unsigned NOT NULL DEFAULT '1',
  `StatusID` smallint(6) unsigned NOT NULL DEFAULT '1',
  `IPAddress` varchar(45) DEFAULT NULL,
  `MacAddress` varchar(64) NOT NULL DEFAULT '',
  `Registered` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `VipLevel` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `ParentIdentity` int(4) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`AccountID`) USING BTREE,
  UNIQUE KEY `AccountID_UNIQUE` (`AccountID`) USING BTREE,
  UNIQUE KEY `Username_UNIQUE` (`Username`) USING BTREE,
  KEY `fk_account_account_authority_idx` (`AuthorityID`) USING BTREE,
  KEY `fk_account_account_status_idx` (`StatusID`) USING BTREE,
  CONSTRAINT `fk_account_account_authority` FOREIGN KEY (`AuthorityID`) REFERENCES `account_authority` (`AuthorityID`),
  CONSTRAINT `fk_account_account_status` FOREIGN KEY (`StatusID`) REFERENCES `account_status` (`StatusID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `account_passwordhash` BEFORE INSERT ON `account` FOR EACH ROW BEGIN
    --
	-- Name:   Password Hash
	-- Author: Gareth Jensen (Spirited)
	-- Date:   2018-09-25
	--
	-- Description:
	-- When a plain text password without a hash or salt has been inserted into the database 
	-- along with a new account, then the plain text password will be hashed and a salt will
	-- be generated from a random MD5 string. Due to client limitations, passwords cannot be
    -- longer than 16 characters.
	-- 
	IF (NEW.`Salt` IS NULL) THEN
    
		IF (LENGTH(NEW.`Password`) > 16) THEN
			SET NEW.`Password` = NULL;
        END IF;
        
        SET NEW.`Salt` = MD5(RAND());
        SET NEW.`Password` = SHA2(CONCAT(NEW.`Password`, NEW.`Salt`), 256);
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `account_authority`
--

DROP TABLE IF EXISTS `account_authority`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account_authority` (
  `AuthorityID` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `AuthorityName` varchar(45) NOT NULL,
  PRIMARY KEY (`AuthorityID`) USING BTREE,
  UNIQUE KEY `AuthorityID_UNIQUE` (`AuthorityID`) USING BTREE,
  UNIQUE KEY `AuthorityName_UNIQUE` (`AuthorityName`) USING BTREE,
  KEY `AuthorityID` (`AuthorityID`) USING BTREE,
  KEY `AuthorityID_2` (`AuthorityID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `account_ban`
--

DROP TABLE IF EXISTS `account_ban`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account_ban` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Type` int(10) unsigned NOT NULL DEFAULT '0',
  `AccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `BanTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ExpireTime` datetime NOT NULL DEFAULT '2199-12-31 23:59:59',
  `BannedBy` int(10) unsigned NOT NULL DEFAULT '0',
  `Reason` varchar(255) NOT NULL DEFAULT '',
  `UpdatedAt` datetime DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `account_status`
--

DROP TABLE IF EXISTS `account_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account_status` (
  `StatusID` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `StatusName` varchar(45) NOT NULL,
  PRIMARY KEY (`StatusID`) USING BTREE,
  UNIQUE KEY `StatusID_UNIQUE` (`StatusID`) USING BTREE,
  UNIQUE KEY `StatusName_UNIQUE` (`StatusName`) USING BTREE,
  KEY `StatusID` (`StatusID`) USING BTREE,
  KEY `StatusID_2` (`StatusID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `articles`
--

DROP TABLE IF EXISTS `articles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `articles` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `user_id` int(4) unsigned NOT NULL DEFAULT '0',
  `section_id` int(4) unsigned NOT NULL DEFAULT '0',
  `create_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `edit_date` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `flag` bigint(16) unsigned NOT NULL DEFAULT '0',
  `del_date` datetime DEFAULT NULL,
  `thumb` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `articles_category`
--

DROP TABLE IF EXISTS `articles_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `articles_category` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `order` int(4) unsigned NOT NULL DEFAULT '0',
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `articles_content`
--

DROP TABLE IF EXISTS `articles_content`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `articles_content` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `article_id` int(4) unsigned NOT NULL,
  `creator_id` int(4) unsigned NOT NULL DEFAULT '0',
  `locale` varchar(8) NOT NULL,
  `title` varchar(255) NOT NULL,
  `content` longtext NOT NULL,
  `last_editor_id` int(4) unsigned NOT NULL DEFAULT '0',
  `creation_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `edit_date` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `article_set` (`article_id`) USING BTREE,
  KEY `article_owner` (`creator_id`) USING BTREE,
  KEY `article_editor` (`last_editor_id`) USING BTREE,
  CONSTRAINT `article_editor` FOREIGN KEY (`last_editor_id`) REFERENCES `ftw_account` (`id`),
  CONSTRAINT `article_owner` FOREIGN KEY (`creator_id`) REFERENCES `ftw_account` (`id`),
  CONSTRAINT `article_set` FOREIGN KEY (`article_id`) REFERENCES `articles` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `articles_read_control`
--

DROP TABLE IF EXISTS `articles_read_control`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `articles_read_control` (
  `Identity` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `UserIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `SessionIdentity` varchar(255) NOT NULL DEFAULT '',
  `UserAgent` varchar(255) NOT NULL DEFAULT '',
  `IpAddress` varchar(255) NOT NULL DEFAULT '0.0.0.0',
  `Referer` varchar(255) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `credit_card`
--

DROP TABLE IF EXISTS `credit_card`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `credit_card` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Type` int(10) unsigned NOT NULL DEFAULT '0',
  `OwnerId` int(10) unsigned NOT NULL DEFAULT '0',
  `CheckoutItemId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `Part1` smallint(4) unsigned zerofill NOT NULL DEFAULT '0000',
  `Part2` smallint(4) unsigned zerofill NOT NULL DEFAULT '0000',
  `Part3` smallint(4) unsigned zerofill NOT NULL DEFAULT '0000',
  `Part4` smallint(4) unsigned zerofill NOT NULL DEFAULT '0000',
  `Password` varchar(20) NOT NULL DEFAULT '',
  `CreatedAt` datetime NOT NULL,
  `UsedAt` datetime DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `credit_card_usage`
--

DROP TABLE IF EXISTS `credit_card_usage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `credit_card_usage` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CardId` int(10) unsigned NOT NULL DEFAULT '0',
  `TargetId` int(10) unsigned NOT NULL DEFAULT '0',
  `UsedAt` datetime NOT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE KEY `cardid` (`CardId`) USING BTREE,
  KEY `userid` (`TargetId`) USING BTREE,
  CONSTRAINT `ChkCard` FOREIGN KEY (`CardId`) REFERENCES `credit_card` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `credit_card_vip`
--

DROP TABLE IF EXISTS `credit_card_vip`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `credit_card_vip` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `CardId` int(10) unsigned NOT NULL DEFAULT '0',
  `CreationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `BoundServerId` int(10) unsigned NOT NULL DEFAULT '0',
  `BoundTargetId` int(10) unsigned NOT NULL DEFAULT '0',
  `BindDate` datetime DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `discord_channel`
--

DROP TABLE IF EXISTS `discord_channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `discord_channel` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ChannelId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(255) NOT NULL,
  `CreatedAt` int(11) NOT NULL DEFAULT '0',
  `Default` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `discord_message`
--

DROP TABLE IF EXISTS `discord_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `discord_message` (
  `Id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `CurrentUserName` varchar(255) NOT NULL,
  `ChannelId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `Message` text NOT NULL,
  `Timestamp` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `discord_user`
--

DROP TABLE IF EXISTS `discord_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `discord_user` (
  `Identity` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `DiscordUserId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `AccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `GameUserId` int(10) unsigned NOT NULL DEFAULT '0',
  `AccountName` varchar(64) NOT NULL,
  `GameName` varchar(16) NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Discriminator` varchar(255) NOT NULL,
  `CreatedAt` int(11) NOT NULL DEFAULT '0',
  `MessagesSent` bigint(20) unsigned NOT NULL DEFAULT '0',
  `CharactersSent` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `download`
--

DROP TABLE IF EXISTS `download`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `download` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `download_url`
--

DROP TABLE IF EXISTS `download_url`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `download_url` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `download_id` int(4) unsigned NOT NULL DEFAULT '0',
  `provider_name` varchar(255) NOT NULL,
  `url` varchar(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `download_id` (`download_id`) USING BTREE,
  CONSTRAINT `downloadidx` FOREIGN KEY (`download_id`) REFERENCES `download` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ftw_account`
--

DROP TABLE IF EXISTS `ftw_account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ftw_account` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `email` varchar(64) NOT NULL,
  `password` varchar(130) NOT NULL,
  `type` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `flag` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `salt` varchar(128) NOT NULL DEFAULT 'DEFAULT_PTZF_SALT',
  `hash` varchar(255) NOT NULL DEFAULT '',
  `vip` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `vip_coins` bigint(16) unsigned NOT NULL DEFAULT '0',
  `vip_points` int(4) unsigned NOT NULL DEFAULT '300',
  `security_code` bigint(16) unsigned NOT NULL DEFAULT '0',
  `security_question` int(4) NOT NULL DEFAULT '0',
  `security_answer` varchar(128) NOT NULL DEFAULT '',
  `country` varchar(64) NOT NULL DEFAULT '',
  `language` varchar(10) NOT NULL DEFAULT '',
  `real_name` varchar(128) NOT NULL DEFAULT '',
  `sex` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `age` date DEFAULT NULL,
  `phone` varchar(16) NOT NULL DEFAULT '',
  `netbar_ip` varchar(16) NOT NULL DEFAULT '',
  `creation_date` datetime DEFAULT NULL,
  `first_login` datetime DEFAULT NULL,
  `last_login` datetime DEFAULT NULL,
  `email_confirmed` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `phone_confirmed` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `twofactor_enabled` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `lockout_enabled` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `access_failed_count` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `lockout_end` timestamp NULL DEFAULT NULL,
  `last_activity` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `default_billing_info` int(10) unsigned NOT NULL DEFAULT '0',
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `email` (`id`) USING BTREE,
  UNIQUE KEY `idx` (`email`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ftw_account_vip`
--

DROP TABLE IF EXISTS `ftw_account_vip`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ftw_account_vip` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `VipCredits` bigint(20) unsigned NOT NULL DEFAULT '0',
  `VipPoints` int(10) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE KEY `AccountId` (`AccountId`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ftw_billing_information`
--

DROP TABLE IF EXISTS `ftw_billing_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ftw_billing_information` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(10) unsigned NOT NULL DEFAULT '0',
  `document` varchar(255) NOT NULL DEFAULT '',
  `first_name` varchar(64) NOT NULL,
  `last_name` varchar(64) NOT NULL,
  `address` varchar(128) NOT NULL,
  `complement` varchar(64) NOT NULL,
  `district` varchar(64) NOT NULL,
  `city` varchar(32) NOT NULL,
  `country` varchar(3) NOT NULL,
  `post_code` varchar(15) NOT NULL,
  `email` varchar(128) NOT NULL,
  `phone` varchar(64) NOT NULL,
  `additional_info` text NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ftw_security_question`
--

DROP TABLE IF EXISTS `ftw_security_question`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ftw_security_question` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `query_str` varchar(32) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `images`
--

DROP TABLE IF EXISTS `images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `images` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `folder` varchar(255) NOT NULL DEFAULT '',
  `img` varchar(255) NOT NULL DEFAULT '',
  `created_at` datetime NOT NULL,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `images_screenshots`
--

DROP TABLE IF EXISTS `images_screenshots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `images_screenshots` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `imgid` int(10) unsigned DEFAULT '0',
  `order` int(11) DEFAULT '999',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `log_activity`
--

DROP TABLE IF EXISTS `log_activity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `log_activity` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `UserId` int(10) unsigned NOT NULL DEFAULT '0',
  `Type` smallint(5) unsigned NOT NULL DEFAULT '0',
  `IdAddress` varchar(255) NOT NULL DEFAULT 'Unknown',
  `Json` text NOT NULL,
  `Timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `login_records`
--

DROP TABLE IF EXISTS `login_records`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `login_records` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(4) unsigned NOT NULL DEFAULT '0',
  `action` int(4) unsigned NOT NULL DEFAULT '0',
  `time` datetime NOT NULL,
  `ip_address` varchar(255) NOT NULL DEFAULT '0.0.0.0',
  `browser` varchar(255) NOT NULL DEFAULT 'Unknown Browser Information',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `permission_user`
--

DROP TABLE IF EXISTS `permission_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permission_user` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(4) unsigned NOT NULL,
  `permission_id` int(4) unsigned NOT NULL,
  `value` int(4) unsigned NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `permission_usergroup`
--

DROP TABLE IF EXISTS `permission_usergroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permission_usergroup` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `group_id` int(4) unsigned NOT NULL,
  `permission_id` int(4) unsigned NOT NULL,
  `value` int(4) unsigned NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `profile_information`
--

DROP TABLE IF EXISTS `profile_information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `profile_information` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(4) unsigned NOT NULL DEFAULT '0',
  `name` varchar(255) NOT NULL,
  `location` varchar(255) NOT NULL,
  `phone` varchar(255) NOT NULL,
  `facebook` varchar(255) NOT NULL,
  `instagram` varchar(255) NOT NULL,
  `twitter` varchar(255) NOT NULL,
  `youtube` varchar(255) NOT NULL,
  `twitch` varchar(255) NOT NULL,
  `about_me` text NOT NULL,
  `selected_character_id` int(4) unsigned NOT NULL DEFAULT '0',
  `post_count` int(4) unsigned NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE KEY `name` (`name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `profile_status_updates`
--

DROP TABLE IF EXISTS `profile_status_updates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `profile_status_updates` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(4) unsigned NOT NULL,
  `type` int(4) unsigned zerofill NOT NULL DEFAULT '0000',
  `reply_from` int(4) unsigned NOT NULL DEFAULT '0',
  `message` text NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `realm`
--

DROP TABLE IF EXISTS `realm`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `realm` (
  `RealmID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(16) COLLATE utf8_bin NOT NULL,
  `AuthorityID` smallint(6) unsigned NOT NULL DEFAULT '1' COMMENT 'Authority level required',
  `GameIPAddress` varchar(45) COLLATE utf8_bin NOT NULL DEFAULT '127.0.0.1',
  `RpcIPAddress` varchar(45) COLLATE utf8_bin NOT NULL DEFAULT '127.0.0.1',
  `GamePort` int(10) unsigned NOT NULL DEFAULT '5816',
  `RpcPort` int(10) unsigned NOT NULL DEFAULT '5817',
  `Status` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `Username` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT 'test',
  `Password` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT 'test',
  `LastPing` datetime DEFAULT NULL,
  `DatabaseHost` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `DatabaseUser` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `DatabasePass` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `DatabaseSchema` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  PRIMARY KEY (`RealmID`) USING BTREE,
  UNIQUE KEY `RealmID_UNIQUE` (`RealmID`) USING BTREE,
  UNIQUE KEY `Name_UNIQUE` (`Name`) USING BTREE,
  KEY `fk_realm_account_authority_idx` (`AuthorityID`) USING BTREE,
  CONSTRAINT `fk_realm_account_authority` FOREIGN KEY (`AuthorityID`) REFERENCES `account_authority` (`AuthorityID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_bin ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `realms_status`
--

DROP TABLE IF EXISTS `realms_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `realms_status` (
  `id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `realm_id` int(4) unsigned NOT NULL,
  `realm_name` varchar(255) NOT NULL,
  `old_status` tinyint(1) unsigned NOT NULL,
  `new_status` tinyint(1) unsigned NOT NULL,
  `time` datetime NOT NULL,
  `players_online` int(4) unsigned NOT NULL DEFAULT '0',
  `max_players_online` int(4) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE,
  KEY `Realms` (`realm_id`) USING BTREE,
  CONSTRAINT `Realms` FOREIGN KEY (`realm_id`) REFERENCES `realm` (`RealmID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `records_family`
--

DROP TABLE IF EXISTS `records_family`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `records_family` (
  `Identity` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `FamilyIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(64) NOT NULL DEFAULT '',
  `LeaderIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `Count` int(10) unsigned NOT NULL DEFAULT '0',
  `Money` bigint(20) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `ChallengeMap` int(10) unsigned NOT NULL DEFAULT '0',
  `DominatedMap` int(10) unsigned NOT NULL DEFAULT '0',
  `Level` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `BpTower` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `records_guild_war`
--

DROP TABLE IF EXISTS `records_guild_war`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `records_guild_war` (
  `Identity` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `SyndicateIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `LeaderIdentity` int(10) unsigned NOT NULL,
  `Date` datetime NOT NULL,
  PRIMARY KEY (`Identity`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `records_syndicate`
--

DROP TABLE IF EXISTS `records_syndicate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `records_syndicate` (
  `Id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `SyndicateIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(16) NOT NULL,
  `LeaderIdentity` int(4) unsigned NOT NULL DEFAULT '0',
  `Count` int(4) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `records_user`
--

DROP TABLE IF EXISTS `records_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `records_user` (
  `Id` int(4) unsigned NOT NULL AUTO_INCREMENT,
  `ServerIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `UserIdentity` int(11) unsigned NOT NULL,
  `AccountIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(16) NOT NULL,
  `MateId` int(4) unsigned NOT NULL,
  `Level` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `Experience` bigint(16) unsigned NOT NULL DEFAULT '0',
  `Profession` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `OldProfession` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `NewProfession` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `Metempsychosis` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `Strength` smallint(2) unsigned NOT NULL DEFAULT '0',
  `Agility` smallint(2) unsigned NOT NULL DEFAULT '0',
  `Vitality` smallint(2) unsigned NOT NULL DEFAULT '0',
  `Spirit` smallint(2) unsigned NOT NULL DEFAULT '0',
  `AdditionalPoints` smallint(2) unsigned NOT NULL DEFAULT '0',
  `SyndicateIdentity` int(4) unsigned NOT NULL DEFAULT '0',
  `SyndicatePosition` smallint(2) unsigned NOT NULL DEFAULT '0',
  `NobilityDonation` bigint(16) unsigned NOT NULL DEFAULT '0',
  `NobilityRank` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `SupermanCount` int(4) unsigned NOT NULL DEFAULT '0',
  `DeletedAt` datetime DEFAULT NULL,
  `Money` bigint(20) unsigned NOT NULL DEFAULT '0',
  `WarehouseMoney` int(10) unsigned NOT NULL DEFAULT '0',
  `ConquerPoints` int(10) unsigned NOT NULL DEFAULT '0',
  `FamilyIdentity` int(10) unsigned NOT NULL DEFAULT '0',
  `FamilyRank` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE KEY `IdIdx` (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reward_type`
--

DROP TABLE IF EXISTS `reward_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reward_type` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Type` int(10) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(255) NOT NULL DEFAULT 'StrUnknown',
  `ActionId` int(10) unsigned NOT NULL DEFAULT '0',
  `ItemType` int(10) unsigned NOT NULL DEFAULT '0',
  `Money` int(10) unsigned NOT NULL DEFAULT '0',
  `ConquerPoints` int(10) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reward_user`
--

DROP TABLE IF EXISTS `reward_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reward_user` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `AccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `RewardId` int(10) unsigned NOT NULL DEFAULT '0',
  `ClaimerId` int(10) unsigned NOT NULL DEFAULT '0',
  `ClaimedAt` datetime DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shop_checkout`
--

DROP TABLE IF EXISTS `shop_checkout`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shop_checkout` (
  `Id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `UserId` int(10) unsigned NOT NULL DEFAULT '0',
  `Comment` text,
  `BillingInformationId` int(10) unsigned NOT NULL DEFAULT '0',
  `PaymentType` int(10) unsigned NOT NULL DEFAULT '0',
  `TransactionToken` varchar(255) NOT NULL,
  `PaidAt` datetime DEFAULT NULL,
  `PaymentMethodType` int(10) unsigned NOT NULL DEFAULT '0',
  `PaymentMethodCode` int(10) unsigned NOT NULL DEFAULT '0',
  `PaymentUrl` varchar(255) NOT NULL,
  `TransactionStatus` int(11) NOT NULL DEFAULT '0',
  `ClientName` varchar(255) NOT NULL DEFAULT '',
  `ClientEmail` varchar(255) NOT NULL DEFAULT '',
  `ClientAddress` varchar(255) NOT NULL DEFAULT '',
  `ClientNumber` int(11) NOT NULL DEFAULT '0',
  `ClientComplement` varchar(255) NOT NULL DEFAULT '',
  `ClientDistrict` varchar(255) NOT NULL DEFAULT '',
  `ClientCity` varchar(255) NOT NULL DEFAULT '',
  `ClientState` varchar(255) NOT NULL DEFAULT '',
  `ClientPostalCode` varchar(255) NOT NULL DEFAULT '',
  `ClientPhone` varchar(255) NOT NULL DEFAULT '',
  `CancelationSource` varchar(255) NOT NULL DEFAULT '',
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=1000000034 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shop_checkout_items`
--

DROP TABLE IF EXISTS `shop_checkout_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shop_checkout_items` (
  `Id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `CheckoutId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `ProductId` int(10) unsigned NOT NULL DEFAULT '0',
  `Amount` int(10) unsigned NOT NULL DEFAULT '1',
  `Value` double NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  KEY `CheckoutId` (`CheckoutId`) USING BTREE,
  CONSTRAINT `ChkItems` FOREIGN KEY (`CheckoutId`) REFERENCES `shop_checkout` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shop_checkout_status_tracking`
--

DROP TABLE IF EXISTS `shop_checkout_status_tracking`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shop_checkout_status_tracking` (
  `Id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `CheckoutId` bigint(20) unsigned NOT NULL DEFAULT '0',
  `TransactionId` varchar(255) NOT NULL,
  `NotificationCode` varchar(255) NOT NULL DEFAULT '',
  `NewStatus` int(10) unsigned NOT NULL DEFAULT '0',
  `Date` datetime NOT NULL,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`) USING BTREE,
  KEY `CheckoutID` (`CheckoutId`) USING BTREE,
  CONSTRAINT `CheckoutID` FOREIGN KEY (`CheckoutId`) REFERENCES `shop_checkout` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shop_products`
--

DROP TABLE IF EXISTS `shop_products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shop_products` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `type` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `name` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `long_description` text NOT NULL,
  `value` double(20,2) unsigned NOT NULL DEFAULT '0.00',
  `data0` int(11) NOT NULL DEFAULT '0',
  `data1` int(11) NOT NULL DEFAULT '0',
  `data2` int(11) NOT NULL DEFAULT '0',
  `data3` int(11) NOT NULL DEFAULT '0',
  `data4` int(11) NOT NULL DEFAULT '0',
  `data5` int(11) NOT NULL DEFAULT '0',
  `data6` int(11) NOT NULL DEFAULT '0',
  `data7` int(11) NOT NULL DEFAULT '0',
  `img_thumb` int(10) unsigned NOT NULL DEFAULT '0',
  `img_main` int(10) unsigned NOT NULL DEFAULT '0',
  `flag` int(10) unsigned NOT NULL DEFAULT '0',
  `from` datetime DEFAULT NULL,
  `to` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=1000000052 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shop_products_img`
--

DROP TABLE IF EXISTS `shop_products_img`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shop_products_img` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `type` tinyint(1) NOT NULL,
  `folder` varchar(255) NOT NULL,
  `url` varchar(255) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `support_tickets`
--

DROP TABLE IF EXISTS `support_tickets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `support_tickets` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `UserId` int(10) unsigned NOT NULL DEFAULT '0',
  `Subject` varchar(255) NOT NULL DEFAULT '',
  `Content` text NOT NULL,
  `Urgency` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `RequireAdm` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastReply` datetime DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `Flag` int(11) NOT NULL DEFAULT '0',
  `Status` int(4) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `support_tickets_answers`
--

DROP TABLE IF EXISTS `support_tickets_answers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `support_tickets_answers` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `TicketId` int(10) unsigned DEFAULT '0',
  `AuthorId` int(10) unsigned DEFAULT '0',
  `Message` text,
  `Solution` tinyint(3) unsigned DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `DeletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `web_exception_handing`
--

DROP TABLE IF EXISTS `web_exception_handing`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `web_exception_handing` (
  `Id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Path` varchar(255) NOT NULL DEFAULT '',
  `Message` varchar(255) NOT NULL,
  `StackTrace` text NOT NULL,
  `Date` datetime NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'account_zf'
--

--
-- Dumping routines for database 'account_zf'
--
/*!50003 DROP PROCEDURE IF EXISTS `GetTopGuildWinners` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopGuildWinners`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	SELECT
		guild.`Name` `SyndicateName`,
		IFNULL( usr.`Name`, "StrUnknown" ) `LeaderName`,
		guild.Count `MemberCount`,
		( SELECT COUNT( gw.SyndicateIdentity ) FROM records_guild_war gw WHERE gw.SyndicateIdentity = guild.SyndicateIdentity ) `GuildWars` 
	FROM
		records_syndicate guild
		LEFT JOIN records_user usr ON usr.UserIdentity = guild.LeaderIdentity 
		WHERE (idServer = -1 OR guild.ServerIdentity = idServer)
		LIMIT maxLimit;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTopMoneybag` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopMoneybag`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		(ru0.Money + ru0.WarehouseMoney) `Moneybag`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.Id
		LEFT JOIN records_syndicate syn ON syn.Id=ru0.SyndicateIdentity
	WHERE 
		ru0.`Name` NOT LIKE "%[%]%"
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY (ru0.Money + ru0.WarehouseMoney) DESC
	LIMIT maxLimitFrom, maxLimit;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTopNoble` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopNoble`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		ru0.NobilityDonation `NobleDonation`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.Id
		LEFT JOIN records_syndicate syn ON syn.Id=ru0.SyndicateIdentity
	WHERE 
		ru0.NobilityDonation > 3000000
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.NobilityDonation DESC
	LIMIT maxLimitFrom, maxLimit;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTopPlayers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopPlayers`(IN minLevel INT, IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.UserIdentity
		LEFT JOIN records_syndicate syn ON syn.SyndicateIdentity=ru0.SyndicateIdentity
	WHERE 
		ru0.`Level` >= minLevel
		AND ru0.`Name` NOT LIKE "%[%]%"
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.`Level` DESC, ru0.Experience DESC
	LIMIT maxLimitFrom, maxLimit;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTopProfession` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopProfession`(IN minLevel INT, IN fromProf INT, IN toProf INT, IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.UserIdentity
		LEFT JOIN records_syndicate syn ON syn.SyndicateIdentity=ru0.SyndicateIdentity
	WHERE 
		ru0.Profession BETWEEN fromProf AND toProf
		AND ru0.`Level` >= minLevel		
		AND ru0.`Name` NOT LIKE "%[%]%"
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.`Level` DESC, ru0.Experience DESC
	LIMIT maxLimitFrom, maxLimit;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTopSuperman` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopSuperman`(IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		ru0.`Name` `PlayerName`,
		IFNULL( ru1.`Name`, "None" ) `MateName`,
		ru0.`Level` `Level`,
		ru0.Profession `Profession`,
		ru0.SupermanCount `SupermanCount`,
		IFNULL( syn.`Name`, "None" ) `SyndicateName`
	FROM
		records_user ru0
		LEFT JOIN records_user ru1 ON ru0.MateId = ru1.Id
		LEFT JOIN records_syndicate syn ON syn.Id=ru0.SyndicateIdentity
	WHERE 
		ru0.`Name` NOT LIKE "%[%]%" AND ru0.SupermanCount > 0		
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY ru0.`SupermanCount` DESC
	LIMIT maxLimitFrom, maxLimit;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetTopSyndicate` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTopSyndicate`(IN minMembers INT, IN maxLimit INT, IN maxLimitFrom INT, IN idServer INT)
BEGIN
	#Routine body goes here...
	SELECT
		syn.`Name` `Name`,
		usr.`Name` `LeaderName`,
		syn.Count `SyndicateCount`,
		(SELECT COUNT(*) FROM records_guild_war rgw WHERE rgw.SyndicateIdentity=syn.SyndicateIdentity) `GuildWarCount`
	FROM
		records_syndicate syn
		LEFT JOIN records_user usr ON usr.UserIdentity=syn.LeaderIdentity AND usr.`Name` NOT LIKE "%[%]%"		
	WHERE 
		syn.Count >= minMembers
		AND ISNULL(syn.DeletedAt)
		AND (idServer = -1 OR ru0.ServerIdentity = idServer)
	ORDER BY syn.`Count` DESC
	LIMIT maxLimitFrom, maxLimit;	
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-06-18 16:01:40
