USE `conquer`;

ALTER TABLE `cq_user` 
CHANGE COLUMN `auto_exercise` `auto_exercise` SMALLINT(2) UNSIGNED NOT NULL DEFAULT '0' ;

ALTER TABLE `cq_user` 
ADD COLUMN `last_logout2` DATETIME NULL DEFAULT NULL AFTER `last_logout`;