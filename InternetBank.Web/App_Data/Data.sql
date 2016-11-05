-- REGIONS AND CITIES

INSERT INTO Regions (Name) VALUES ('Минская обл.')
GO
INSERT INTO Regions (Name) VALUES ('Могилевская обл.')
GO
INSERT INTO Cities (Name, RegionId) VALUES ('Минск', 1)
GO
INSERT INTO Cities (Name, RegionId) VALUES ('Могилев', 2)
GO

--SERVICES AND CATEGORIES

INSERT INTO ServiceCategories (Name, ServiceCategoryId, LocalityId) VALUES ('Система "Расчет" (ЕРИП)', NULL, NULL)
GO
	INSERT INTO ServiceCategories (Name, ServiceCategoryId, LocalityId) VALUES ('БЕЛТЕЛЕКОМ', 1, NULL)
GO
		INSERT INTO ServiceCategories (Name, ServiceCategoryId, LocalityId) VALUES ('Могилев и Могилевская область', 2, NULL)
GO
			INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('Интернет (BYFLY, ZALA, Максифон)', 3, NULL, '', '')
GO
			INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('Телефон', 3, NULL, '', '')
GO
	INSERT INTO ServiceCategories (Name, ServiceCategoryId, LocalityId) VALUES ('Интернет, Телевидение', 1, NULL)
GO
		INSERT INTO ServiceCategories (Name, ServiceCategoryId, LocalityId) VALUES ('ЭФИР ЗАО', 4, NULL)
GO
			INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('Интернет', 5, NULL, '', '')
GO
			INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('Телефон', 5, NULL, '', '')
GO
		INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('Деловая сеть', 4, NULL, '', '')
GO
		INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('Космос ТВ', 4, NULL, '', '')
GO
		INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('МТС - Домашний интернет', 4, NULL, '', '')
GO
INSERT INTO [ServiceCategories] (Name, ServiceCategoryId, LocalityId) VALUES ('Мобильный телефон', NULL, NULL)
GO
	INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('МТС', 6, NULL, '', '')
GO
	INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('life:)', 6, NULL, '', '')
GO
	INSERT INTO [Services] (Name, ServiceCategoryId, LocalityId, ScriptName, TemplateName) VALUES ('VELCOM', 6, NULL, '', '')
GO

--USERS

INSERT INTO [Users] ([Login], [Password], [PassportNumber], FIO, PassportData, IsBlocked, LoginAttempts, IsActivated, IsAdmin, CityId) VALUES ('admin', 'admin', '0', '', '', 0, 0, 1, 1, 2)
GO
INSERT INTO [Users] ([Login], [Password], [PassportNumber], FIO, PassportData, IsBlocked, LoginAttempts, IsActivated, IsAdmin, CityId) VALUES (NULL, NULL, '12345', '', '', 0, 0, 0, 0, 2)
GO
INSERT INTO [Users] ([Login], [Password], [PassportNumber], FIO, PassportData, IsBlocked, LoginAttempts, IsActivated, IsAdmin, CityId) VALUES ('user1', '123', '123', '', '', 0, 0, 1, 0, 2)
GO
