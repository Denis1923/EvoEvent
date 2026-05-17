# EvoEvent
Cервис для управления мероприятиями на ASP.NET Core Web API

## Запуск

### cmd
1. перейти в директорию проекта .\EvoEvent\EvoEvent.Web
2. выполнить команду dotnet run --project EvoEvent.Web
3. перейти по ссылке [https:SwaggerUI](https://localhost:7062/swagger/index.html)

### IDE Visual Studio
1. перейти в проект .\EvoEvent\EvoEvent.Web
2. нажать F5 
3. перейти по ссылке [https:SwaggerUI](https://localhost:7062/swagger/index.html)

## Остановить приложение

### cmd
- в командной строке где запущенно приложение выполнить комбинацию клавиш Ctrl+C

### IDE Visual Studio
- в IDE, где запущено приложение выполнить комбинацию клавиш Shift+F5

## Запуск тестов

### cmd
1. перейти в директорию проекта .\EvoEvent\EvoEvent.Web\
2. выполнить команду dotnet test

### IDE Visual Studio
- в IDE, где запущено приложение открыть Обозреватель тестов (Вкладка Тест -> Обозреватель тестов) 
- выполнить комбинацию клавиш Ctrl+R, Ctrl+A
- в окне Обозреватель тестов будет статистика о прохождение тестов

## PostgreSQL 
Для использования PostgreSQL, требуется nuget-пакеты:
 - Microsoft.EntityFrameworkCore — ядро EF Core;
 - Npgsql.EntityFrameworkCore.PostgreSQL — провайдер для PostgreSQL.
В тестовом проекте для uNit-тестов пакет:
 - Microsoft.EntityFrameworkCore.InMemory — InMemory-провайдер для юнит-тестов.
В тестовом проекте для интеграционных тестов пакет:
 - Testcontainers.PostgreSql - для запуска Docker-контенйнера под интеграционные тесты
   
Для подключния требуется в файде appsettings.json поправить значение DefaultConnection объекта ConnectionStrings. Значение должно заполняться по шаблону 
"Host=<value>;Port=<value>;Database=<value>;Username=<value>;Password=<value>", где:
 - Host - Адрес сервера БД	
 - Port - Порт PostgreSQL
 - Database	- Имя базы данных
 - Username	- Имя пользователя
 - Password	- Пароль
Схема БД создаётся автоматически при запуске через EnsureCreated
В uNit-тестах используется InMemory-провайдера
В Интеграционных-тестах используется Testcontainers для обращения к БД. Перед использованием требутся наличие Docker Desktop

## Создание миграций БД
- перейти в комнадную строку 
- перейти в проект .\EvoEvent\EvoEvent.Web
- выполнить команду dotnet ef migrations add <название миграции>
- в проекте EvoEvent.Web в Program.cs ьреуется прописать код 
"using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}"

## Описание методов 

### POST /events
опичание: создание события
входные параметры:
 - Title (string, обязательное) - название события
 - Description (string, опциональное) - описание
 - StartAt (DateTime, обязательное) - дата начала
 - EndAt (DateTime, обязательное) - дата окончания
 - TotalSeats (int?, обязательное) - кол-во мест

### GET /events
описание: получение событий
входные параметры:
 - title (string, опциональный) — поиск по названию (регистронезависимый, частичное совпадение)
 - from(DateTime, опциональный) — события, которые начинаются не раньше указанной даты
 - to(DateTime, опциональный) — события, которые заканчиваются не позже указанной даты
 - page (int, опциональный, c значением по умолчанию 1) — страница, которую необходимо вернуть
 - pageSize (int, опциональный, со значением по умолчанию 10) — количество элементов на странице

### POST /events/{id}/book
описание: создания брони. При отсутствии мест может возвращать 409 Conflict
входные параметры:
 - id (Guid, обязательное) — уникальный идентификатор события 

выходные параметры:
 - Id (Guid, обязательное) — уникальный идентификатор брони;
 - EventId (Guid, обязательное) — идентификатор события, к которому относится бронь;
 - Status (BookingStatus, обязательное) — текущий статус брони;

в header передаются:
 - Location - ссылка на созданную бронь
 - StatusCode - 202 Accepted 

### GET /bookings/{id}
описание: получения брони
входные параметры:
 - id (Guid, обязательное) — уникальный идентификатор брони 

выходные параметры:
 - Id (Guid, обязательное) — уникальный идентификатор брони;
 - EventId (Guid, обязательное) — идентификатор события, к которому относится бронь;
 - Status (BookingStatus, обязательное) — текущий статус брони;

## Описание фоновой обработки брони
Описание: обработка осуществляется только той брони что находятся в статусе Pending (в ожидании)
Пример: 
 - создайте событие через POST /events;
 - создайте бронь через POST /events/{id}/book — убедитесь, что получили 202 Accepted и Location;
 - сразу запросите GET /bookings/{id} — статус должен быть Pending;
 - подождите несколько секунд и повторите запрос — статус должен измениться на Confirmed или Rejected.

## Глоссарий

### Описание параметров ответа при ошибке
 - IsSuccess = false (указывает что запрос завершился неудачей)
 - Message = текст ошибки
 - StatusCode = выводит код ответа (пример: 400, 404, 500)

### Возможные коды ошибок
 - 400 Bad Request для ошибок валидации;
 - 404 Not Found для ситуаций, когда ресурс не найден;
 - 409 Conflict при отсутствии мест;
 - 500 Internal Server Error для непредвиденных ошибок.

### Описание BookingStatus
 - Pending — бронь создана, ожидает обработки;
 - Confirmed — бронь подтверждена;
 - Rejected — бронь отклонена.

## Описание моделей

### Event
 - Title (string) - название события
 - Description (string) - описание
 - StartAt (DateTime) - дата начала
 - EndAt (DateTime) - дата окончания
 - TotalSeats (int) — общее количество мест на событии;
 - AvailableSeats (int) — текущее количество свободных мест; при создании равно TotalSeats.

### Booking
 - Id (Guid) — уникальный идентификатор брони;
 - EventId (Guid) — идентификатор события, к которому относится бронь;
 - Status (BookingStatus) — текущий статус брони;
 - CreatedAt (DateTime) — дата и время создания брони;
 - ProcessedAt (DateTime?) — дата и время обработки брони.
