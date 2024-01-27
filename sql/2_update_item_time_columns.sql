USE `conquer`;

ALTER TABLE `cq_item` 
CHANGE COLUMN `del_time` `del_time` INT NOT NULL DEFAULT 0 ;