-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Хост: localhost
-- Время создания: Май 16 2024 г., 12:53
-- Версия сервера: 10.4.32-MariaDB
-- Версия PHP: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `ra3_mainlogs`
--

DELIMITER $$
--
-- Процедуры
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `addLogsData` (IN `in_table` VARCHAR(32), IN `in_where` VARCHAR(500), IN `in_what` VARCHAR(1000))  COMMENT 'Добавление данных в логи' BEGIN
	SET @s = CONCAT('INSERT INTO ', in_table, ' (', in_where, ') VALUES (', in_what,')');
	PREPARE stm FROM @s;
	EXECUTE stm;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `updLogsData` (IN `in_table` VARCHAR(32), IN `in_datas` VARCHAR(1000), IN `in_where` VARCHAR(500))  COMMENT 'Обновление данных в логах' BEGIN
    SET @s = CONCAT('UPDATE ', in_table, ' SET ', in_datas, ' WHERE ', in_where);
    PREPARE stm FROM @s;
    EXECUTE stm;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `acclog`
--

CREATE TABLE `acclog` (
  `time` datetime NOT NULL,
  `login` varchar(50) NOT NULL,
  `hwid` varchar(256) NOT NULL,
  `ip` varchar(256) NOT NULL,
  `sclub` varchar(50) NOT NULL,
  `action` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Структура таблицы `addinfo`
--

CREATE TABLE `addinfo` (
  `time` datetime NOT NULL,
  `action` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `adminlog`
--

CREATE TABLE `adminlog` (
  `time` datetime NOT NULL,
  `admin` varchar(50) NOT NULL,
  `action` varchar(350) NOT NULL,
  `player` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Структура таблицы `arrestlog`
--

CREATE TABLE `arrestlog` (
  `time` datetime DEFAULT NULL,
  `player` int(11) DEFAULT NULL,
  `target` int(11) DEFAULT NULL,
  `reason` varchar(300) DEFAULT NULL,
  `stars` int(11) DEFAULT NULL,
  `pnick` varchar(60) DEFAULT NULL,
  `tnick` varchar(60) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `banlog`
--

CREATE TABLE `banlog` (
  `time` datetime NOT NULL,
  `admin` int(11) NOT NULL,
  `player` int(11) NOT NULL,
  `login` varchar(50) DEFAULT NULL,
  `until` datetime NOT NULL,
  `reason` varchar(300) NOT NULL,
  `ishard` tinyint(4) NOT NULL,
  `rgscemailhash` varchar(128) DEFAULT '-'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `casinolog`
--

CREATE TABLE `casinolog` (
  `roulette` bigint(20) DEFAULT 0,
  `horses` bigint(20) DEFAULT 0,
  `spins` bigint(20) DEFAULT 0,
  `bj` bigint(20) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `client_tc`
--

CREATE TABLE `client_tc` (
  `time` datetime DEFAULT NULL,
  `path` varchar(50) DEFAULT NULL,
  `callback` varchar(100) DEFAULT NULL,
  `message` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `deletelog`
--

CREATE TABLE `deletelog` (
  `time` datetime DEFAULT NULL,
  `uuid` int(11) DEFAULT NULL,
  `name` varchar(100) DEFAULT NULL,
  `account` varchar(50) DEFAULT NULL,
  `bank` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `events`
--

CREATE TABLE `events` (
  `ID` int(11) DEFAULT NULL,
  `Event` text DEFAULT NULL,
  `Calls` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `eventslog`
--

CREATE TABLE `eventslog` (
  `ID` int(12) NOT NULL,
  `AdminStarted` varchar(40) NOT NULL,
  `AdminClosed` varchar(40) DEFAULT NULL,
  `EventName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Members` smallint(4) UNSIGNED NOT NULL DEFAULT 0,
  `MembersLimit` smallint(4) UNSIGNED NOT NULL,
  `Winner` varchar(40) NOT NULL DEFAULT 'Undefined',
  `Reward` int(6) NOT NULL DEFAULT 0,
  `RewardLimit` int(6) UNSIGNED NOT NULL DEFAULT 0,
  `Started` datetime NOT NULL,
  `Ended` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `fraclog`
--

CREATE TABLE `fraclog` (
  `time` datetime DEFAULT NULL,
  `frac` varchar(25) DEFAULT NULL,
  `player` int(11) DEFAULT NULL,
  `target` int(11) DEFAULT NULL,
  `pname` varchar(50) DEFAULT NULL,
  `tname` varchar(50) DEFAULT NULL,
  `action` varchar(350) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `idlog`
--

CREATE TABLE `idlog` (
  `in` datetime NOT NULL,
  `out` datetime DEFAULT NULL,
  `uuid` int(11) NOT NULL,
  `id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `sclub` varchar(50) DEFAULT NULL,
  `hwid` varchar(256) DEFAULT NULL,
  `ip` varchar(30) DEFAULT NULL,
  `login` varchar(50) DEFAULT NULL,
  `reason` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Структура таблицы `itemslog`
--

CREATE TABLE `itemslog` (
  `time` datetime NOT NULL,
  `from` varchar(50) NOT NULL,
  `to` varchar(50) NOT NULL,
  `type` int(4) NOT NULL,
  `amount` int(11) NOT NULL,
  `data` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Структура таблицы `killlog`
--

CREATE TABLE `killlog` (
  `time` datetime DEFAULT NULL,
  `killer` varchar(50) DEFAULT NULL,
  `weapon` varchar(50) DEFAULT NULL,
  `victim` varchar(50) DEFAULT NULL,
  `pos` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `moneylog`
--

CREATE TABLE `moneylog` (
  `time` datetime NOT NULL,
  `from` varchar(50) NOT NULL,
  `to` varchar(50) NOT NULL,
  `amount` bigint(20) NOT NULL,
  `comment` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `namelog`
--

CREATE TABLE `namelog` (
  `time` datetime NOT NULL,
  `uuid` int(11) NOT NULL,
  `old` varchar(50) NOT NULL,
  `new` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `stocklog`
--

CREATE TABLE `stocklog` (
  `time` datetime NOT NULL,
  `frac` int(5) NOT NULL,
  `uuid` int(8) NOT NULL,
  `name` varchar(50) NOT NULL DEFAULT '-1',
  `type` varchar(35) NOT NULL,
  `amount` int(11) NOT NULL,
  `in` tinyint(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin ROW_FORMAT=DYNAMIC;

-- --------------------------------------------------------

--
-- Структура таблицы `ticketlog`
--

CREATE TABLE `ticketlog` (
  `time` datetime DEFAULT NULL,
  `player` int(11) DEFAULT NULL,
  `target` int(11) DEFAULT NULL,
  `sum` int(11) DEFAULT NULL,
  `reason` varchar(300) DEFAULT NULL,
  `pnick` varchar(60) DEFAULT NULL,
  `tnick` varchar(60) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- --------------------------------------------------------

--
-- Структура таблицы `unique`
--

CREATE TABLE `unique` (
  `time` datetime DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `maxplayers` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `acclog`
--
ALTER TABLE `acclog`
  ADD KEY `time` (`time`);

--
-- Индексы таблицы `adminlog`
--
ALTER TABLE `adminlog`
  ADD KEY `time` (`time`);

--
-- Индексы таблицы `banlog`
--
ALTER TABLE `banlog`
  ADD KEY `time` (`time`);

--
-- Индексы таблицы `eventslog`
--
ALTER TABLE `eventslog`
  ADD PRIMARY KEY (`ID`);

--
-- Индексы таблицы `idlog`
--
ALTER TABLE `idlog`
  ADD KEY `in` (`in`);

--
-- Индексы таблицы `itemslog`
--
ALTER TABLE `itemslog`
  ADD KEY `time` (`time`);

--
-- Индексы таблицы `moneylog`
--
ALTER TABLE `moneylog`
  ADD KEY `time` (`time`);

--
-- Индексы таблицы `namelog`
--
ALTER TABLE `namelog`
  ADD KEY `time` (`time`);

--
-- Индексы таблицы `stocklog`
--
ALTER TABLE `stocklog`
  ADD KEY `time` (`time`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `eventslog`
--
ALTER TABLE `eventslog`
  MODIFY `ID` int(12) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=854;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
