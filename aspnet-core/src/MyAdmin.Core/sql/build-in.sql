-- AdminTemplate.Log definition

CREATE TABLE `Log` (
                       `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                       `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
                       `Level` int NOT NULL,
                       `UserName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `LogTime` datetime(6) NOT NULL,
                       `IpAddress` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                       `UserAgent` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                       `HttpMethod` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                       `Exceptions` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `Content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `Host` varchar(100) DEFAULT NULL,
                       `ContentType` varchar(100) DEFAULT NULL,
                       `Origin` varchar(100) DEFAULT NULL,
                       `Referer` varchar(100) DEFAULT NULL,
                       `Url` varchar(100) DEFAULT NULL,
                       `Trace` varchar(100) DEFAULT NULL,
                       `RequestBody` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `ResponseStatusCode` varchar(100) DEFAULT NULL,
                       `ResponseBody` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `Type` tinyint DEFAULT NULL COMMENT 'LogType:1Default;2Request',
                       PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;