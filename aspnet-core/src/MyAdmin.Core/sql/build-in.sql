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
                       `RequestBody` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `ResponseStatusCode` varchar(100) DEFAULT NULL,
                       `ResponseBody` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                       `Type` tinyint DEFAULT NULL COMMENT 'LogType:1Default;2Request',
                       PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `UserRole` (
                            `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                            `RoleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                            PRIMARY KEY (`UserId`,`RoleId`),
                            KEY `UserRole_MaRole_FK` (`RoleId`),
                            CONSTRAINT `UserRole_MaRole_FK` FOREIGN KEY (`RoleId`) REFERENCES `MaRole` (`Id`),
                            CONSTRAINT `UserRole_MaUser_FK` FOREIGN KEY (`UserId`) REFERENCES `MaUser` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `RolePermission` (
                                  `RoleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                  `PermissionId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                  PRIMARY KEY (`RoleId`,`PermissionId`),
                                  KEY `RolePermission_MaPermission_FK` (`PermissionId`),
                                  CONSTRAINT `RolePermission_MaPermission_FK` FOREIGN KEY (`PermissionId`) REFERENCES `MaPermission` (`Id`),
                                  CONSTRAINT `RolePermission_MaRole_FK` FOREIGN KEY (`RoleId`) REFERENCES `MaRole` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `MaUser` (
                          `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `IsEnabled` tinyint(1) NOT NULL,
                          `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                          `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                          `Account` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                          `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                          `Mobile` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                          `TenantId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `CreationTime` datetime(6) NOT NULL,
                          `CreatorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `LastModificationTime` datetime(6) DEFAULT NULL,
                          `LastModifierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `IsDeleted` tinyint(1) NOT NULL,
                          `DeleterId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `DeletionTime` datetime(6) DEFAULT NULL,
                          PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `MaRole` (
                          `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `IsEnabled` tinyint(1) NOT NULL,
                          `TenantId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                          `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                          `CreationTime` datetime(6) NOT NULL,
                          `CreatorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `LastModificationTime` datetime(6) DEFAULT NULL,
                          `LastModifierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `IsDeleted` tinyint(1) NOT NULL,
                          `DeleterId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                          `DeletionTime` datetime(6) DEFAULT NULL,
                          PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `MaPermission` (
                                `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                `IsEnabled` tinyint(1) NOT NULL,
                                `TenantId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
                                `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
                                `CreationTime` datetime(6) NOT NULL,
                                `CreatorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                `LastModificationTime` datetime(6) DEFAULT NULL,
                                `LastModifierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                `IsDeleted` tinyint(1) NOT NULL,
                                `DeleterId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
                                `DeletionTime` datetime(6) DEFAULT NULL,
                                PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;