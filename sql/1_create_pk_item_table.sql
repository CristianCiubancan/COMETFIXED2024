USE `conquer`;

CREATE TABLE `cq_pk_item` (
  `id` int(4) NOT NULL AUTO_INCREMENT,
  `item` int(4) unsigned NOT NULL DEFAULT '0',
  `target` int(4) NOT NULL DEFAULT '0',
  `target_name` varchar(45) COLLATE utf8mb4_bin NOT NULL DEFAULT '',
  `hunter` int(4) NOT NULL DEFAULT '0',
  `hunter_name` varchar(45) COLLATE utf8mb4_bin NOT NULL DEFAULT '',
  `manhunt_time` int(4) NOT NULL DEFAULT '0',
  `bonus` smallint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE,
  KEY `idx_hunter_item` (`hunter`,`item`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;
