-- --------------------------------------------------------
-- Хост:                         127.0.0.1
-- Версия сервера:               10.10.2-MariaDB - mariadb.org binary distribution
-- Операционная система:         Win64
-- HeidiSQL Версия:              12.1.0.6537
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Дамп структуры для таблица mainlogs.acclog
CREATE TABLE IF NOT EXISTS `acclog` (
  `time` datetime NOT NULL,
  `login` varchar(50) NOT NULL,
  `hwid` varchar(256) NOT NULL,
  `ip` varchar(256) NOT NULL,
  `sclub` varchar(50) NOT NULL,
  `action` varchar(100) NOT NULL,
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.addinfo
CREATE TABLE IF NOT EXISTS `addinfo` (
  `time` datetime NOT NULL,
  `action` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для процедура mainlogs.addLogsData
DELIMITER //
CREATE PROCEDURE `addLogsData`(
	IN `in_table` VARCHAR(32),
	IN `in_where` VARCHAR(500),
	IN `in_what` VARCHAR(1000)
)
    COMMENT 'Добавление данных в логи'
BEGIN
	SET @s = CONCAT('INSERT INTO ', in_table, ' (', in_where, ') VALUES (', in_what,')');
	PREPARE stm FROM @s;
	EXECUTE stm;
END//
DELIMITER ;

-- Дамп структуры для таблица mainlogs.adminlog
CREATE TABLE IF NOT EXISTS `adminlog` (
  `time` datetime NOT NULL,
  `admin` varchar(50) NOT NULL,
  `action` varchar(350) NOT NULL,
  `player` varchar(50) NOT NULL,
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.arrestlog
CREATE TABLE IF NOT EXISTS `arrestlog` (
  `time` datetime DEFAULT NULL,
  `player` int(11) DEFAULT NULL,
  `target` int(11) DEFAULT NULL,
  `reason` varchar(300) DEFAULT NULL,
  `stars` int(11) DEFAULT NULL,
  `pnick` varchar(60) DEFAULT NULL,
  `tnick` varchar(60) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.banlog
CREATE TABLE IF NOT EXISTS `banlog` (
  `time` datetime NOT NULL,
  `admin` int(11) NOT NULL,
  `player` int(11) NOT NULL,
  `login` varchar(50) DEFAULT NULL,
  `until` datetime NOT NULL,
  `reason` varchar(300) NOT NULL,
  `ishard` tinyint(4) NOT NULL,
  `rgscemailhash` varchar(128) DEFAULT '-',
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.casinolog
CREATE TABLE IF NOT EXISTS `casinolog` (
  `roulette` bigint(20) DEFAULT 0,
  `horses` bigint(20) DEFAULT 0,
  `spins` bigint(20) DEFAULT 0,
  `bj` bigint(20) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.client_tc
CREATE TABLE IF NOT EXISTS `client_tc` (
  `time` datetime DEFAULT NULL,
  `path` varchar(50) DEFAULT NULL,
  `callback` varchar(100) DEFAULT NULL,
  `message` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.deletelog
CREATE TABLE IF NOT EXISTS `deletelog` (
  `time` datetime DEFAULT NULL,
  `uuid` int(11) DEFAULT NULL,
  `name` varchar(100) DEFAULT NULL,
  `account` varchar(50) DEFAULT NULL,
  `bank` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.events
CREATE TABLE IF NOT EXISTS `events` (
  `ID` int(11) DEFAULT NULL,
  `Event` text DEFAULT NULL,
  `Calls` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.eventslog
CREATE TABLE IF NOT EXISTS `eventslog` (
  `ID` int(12) NOT NULL AUTO_INCREMENT,
  `AdminStarted` varchar(40) NOT NULL,
  `AdminClosed` varchar(40) DEFAULT NULL,
  `EventName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Members` smallint(4) unsigned NOT NULL DEFAULT 0,
  `MembersLimit` smallint(4) unsigned NOT NULL,
  `Winner` varchar(40) NOT NULL DEFAULT 'Undefined',
  `Reward` int(6) NOT NULL DEFAULT 0,
  `RewardLimit` int(6) unsigned NOT NULL DEFAULT 0,
  `Started` datetime NOT NULL,
  `Ended` datetime DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=854 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.fraclog
CREATE TABLE IF NOT EXISTS `fraclog` (
  `time` datetime DEFAULT NULL,
  `frac` varchar(25) DEFAULT NULL,
  `player` int(11) DEFAULT NULL,
  `target` int(11) DEFAULT NULL,
  `pname` varchar(50) DEFAULT NULL,
  `tname` varchar(50) DEFAULT NULL,
  `action` varchar(350) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.idlog
CREATE TABLE IF NOT EXISTS `idlog` (
  `in` datetime NOT NULL,
  `out` datetime DEFAULT NULL,
  `uuid` int(11) NOT NULL,
  `id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `sclub` varchar(50) DEFAULT NULL,
  `hwid` varchar(256) DEFAULT NULL,
  `ip` varchar(30) DEFAULT NULL,
  `login` varchar(50) DEFAULT NULL,
  `reason` varchar(50) DEFAULT NULL,
  KEY `in` (`in`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.itemslog
CREATE TABLE IF NOT EXISTS `itemslog` (
  `time` datetime NOT NULL,
  `from` varchar(50) NOT NULL,
  `to` varchar(50) NOT NULL,
  `type` int(4) NOT NULL,
  `amount` int(11) NOT NULL,
  `data` varchar(250) NOT NULL,
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.killlog
CREATE TABLE IF NOT EXISTS `killlog` (
  `time` datetime DEFAULT NULL,
  `killer` varchar(50) DEFAULT NULL,
  `weapon` varchar(50) DEFAULT NULL,
  `victim` varchar(50) DEFAULT NULL,
  `pos` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.moneylog
CREATE TABLE IF NOT EXISTS `moneylog` (
  `time` datetime NOT NULL,
  `from` varchar(50) NOT NULL,
  `to` varchar(50) NOT NULL,
  `amount` bigint(20) NOT NULL,
  `comment` varchar(50) NOT NULL,
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.namelog
CREATE TABLE IF NOT EXISTS `namelog` (
  `time` datetime NOT NULL,
  `uuid` int(11) NOT NULL,
  `old` varchar(50) NOT NULL,
  `new` varchar(50) NOT NULL,
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.stocklog
CREATE TABLE IF NOT EXISTS `stocklog` (
  `time` datetime NOT NULL,
  `frac` int(5) NOT NULL,
  `uuid` int(8) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '-1',
  `type` varchar(35) NOT NULL,
  `amount` int(11) NOT NULL,
  `in` tinyint(2) NOT NULL,
  KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.ticketlog
CREATE TABLE IF NOT EXISTS `ticketlog` (
  `time` datetime DEFAULT NULL,
  `player` int(11) DEFAULT NULL,
  `target` int(11) DEFAULT NULL,
  `sum` int(11) DEFAULT NULL,
  `reason` varchar(300) DEFAULT NULL,
  `pnick` varchar(60) DEFAULT NULL,
  `tnick` varchar(60) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- Экспортируемые данные не выделены.

-- Дамп структуры для таблица mainlogs.unique
CREATE TABLE IF NOT EXISTS `unique` (
  `time` datetime DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `maxplayers` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Экспортируемые данные не выделены.

-- Дамп структуры для процедура mainlogs.updLogsData
DELIMITER //
CREATE PROCEDURE `updLogsData`(
	IN `in_table` VARCHAR(32),
	IN `in_datas` VARCHAR(1000),
	IN `in_where` VARCHAR(500)
)
    COMMENT 'Обновление данных в логах'
BEGIN
    SET @s = CONCAT('UPDATE ', in_table, ' SET ', in_datas, ' WHERE ', in_where);
    PREPARE stm FROM @s;
    EXECUTE stm;
END//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
