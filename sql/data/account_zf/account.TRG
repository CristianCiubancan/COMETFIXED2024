TYPE=TRIGGERS
triggers='CREATE DEFINER=`root`@`localhost` TRIGGER `account_passwordhash` BEFORE INSERT ON `account` FOR EACH ROW BEGIN\n    \n	\n	\n	\n	\n	\n	\n	\n	\n    \n	\n	IF (NEW.`Salt` IS NULL) THEN\n    \n		IF (LENGTH(NEW.`Password`) > 16) THEN\n			SET NEW.`Password` = NULL;\n        END IF;\n        \n        SET NEW.`Salt` = MD5(RAND());\n        SET NEW.`Password` = SHA2(CONCAT(NEW.`Password`, NEW.`Salt`), 256);\n    END IF;\nEND'
sql_modes=1344274432
definers='root@localhost'
client_cs_names='utf8mb4'
connection_cl_names='utf8mb4_general_ci'
db_cl_names='utf8mb4_general_ci'
created=1706397142447260
