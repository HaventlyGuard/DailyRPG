# DailyRPG
Пользователь управляет своим списком дел через календарь и дашборд, но каждая задача привязана к прокачиваемой характеристике — например, «Здоровье», «Интеллект» или «Связи» (их можно создавать самому). Завершая задачи, игрок получает опыт в соответствующей характеристике и игровую валюту трёх типов: золото, серебро, бронза. Отдельная премиум-валюта — алмазы — начисляется за особо важные задания или когда за день выполнены абсолютно все запланированные задачи.

Накопленные валюты можно потратить в магазине:

Бустеры ускоряют получение опыта или денег.

Предметы отображаются в профиле и влияют на персонажа.

Разрешения на развлечения покупаются за алмазы, добавляя элемент «заслуженного отдыха».

Личный профиль собирает все кастомные характеристики, инвентарь и прогресс, делая самоорганизацию наглядной и мотивирующей. В будущем проект планируется расширить мультиплеерными режимами — кланами и дуэлями между игроками.

Одной фразой: это «RPG для твоих реальных задач», где рутина превращается в приключение с прокачкой, наградами и долгосрочными целями.



## 1. Верхнеуровневая архитектура (контекстная карта)
Основные бизнес-способности:

Управление пользователями и их RPG-характеристиками – кастомные атрибуты, уровни, опыт.

Планирование задач – создание, привязка к датам календаря, выбор категории (характеристики).

Система наград и валют – начисление золота, серебра, бронзы, алмазов; проверка «всех задач за день».

Магазин и инвентарь – покупка бустеров, предметов, разрешений.

Активные эффекты – бустеры, временно усиливающие заработок валют или опыта.

Микросервисы выделяем по бизнес-доменам. Каждый сервис владеет своими данными, общается через синхронные REST/gRPC API и асинхронную шину событий.

text
[Клиент (Web/App)] 
    -> [API Gateway (YARP)]
          -> [Identity Service] (аутентификация/авторизация)
          -> [Character Service] (профиль, характеристики, опыт)
          -> [Task Service] (задачи, календарь)
          -> [Reward Service] (валюты, алмазы, начисление)
          -> [Store Service] (витрина, покупки)
          -> [Inventory Service] (предметы, разрешения)
          -> [Booster Service] (активные бустеры, модификаторы)

Event Bus (RabbitMQ + MassTransit):
    TaskCompleted, TaskListCompleted, 
    ExperienceGained, CurrencyAwarded,
    ItemPurchased, BoosterActivated…
## 2. Микросервисы и их ответственности
### 2.1. Identity Service
Регистрация, вход, JWT-токены (access + refresh).

Роли, права доступа.

Хранение базового профиля (Id, Email, DisplayName).

Технологии: ASP.NET Core Identity, EF Core + PostgreSQL, JWT Bearer.

### 2.2. Character Service
Управление созданными пользователем характеристиками: Name, Category, ExperiencePoints, Level.

При создании задачи пользователь выбирает одну из своих характеристик (категорию).

Обработка события ExperienceGained – увеличивает опыт в соответствующей характеристике.

Просмотр профиля с характеристиками и экипированными предметами (read-side проекция из Inventory).

Данные: UserCharacter, CharacterAttribute.

### 2.3. Task Service
CRUD задач: Title, Description, DueDate, CategoryId (ссылка на характеристику), Difficulty, IsCompleted.

Календарное отображение (выборка задач по дням/месяцам).

Дашборд: общая сводка по задачам на сегодня, прогресс.

При отметке задачи выполненной публикует событие TaskCompleted с информацией: UserId, TaskId, CategoryId, Difficulty, CompletionDate.

Содержит логику проверки выполнения всех задач на календарный день: если после завершения очередной задачи все задачи дня выполнены, публикует AllDailyTasksCompleted.

### 2.4. Reward Service
Хранит балансы: Gold, Silver, Bronze, Diamonds.

Подписан на TaskCompleted: вычисляет базовое количество валют (по сложности) и запрашивает у Booster Service активные множители, после чего начисляет итоговую сумму. Публикует CurrencyAwarded.

Подписан на AllDailyTasksCompleted: начисляет алмазы (фиксированное число или по формуле), публикует DiamondsAwarded.

Важные задания (флаг IsImportant в задаче) также могут давать алмазы – это обрабатывается отдельным путём в событии TaskCompleted с проверкой важности.

API для чтения баланса.

Сага для покупок: получает команду списания, проверяет баланс, публикует событие списания/подтверждения.

### 2.5. Store Service
Каталог товаров: ItemId, Type (Booster/Item/Entertainment), Cost (в разной валюте), Effect.

Обрабатывает запрос на покупку: инициирует сагу с Reward Service (проверка и списание валюты) и добавление предмета/активацию бустера.

После успешной покупки публикует ItemPurchased.

### 2.6. Inventory Service
Хранит владения пользователя: какие предметы куплены, количество, полученные разрешения на развлечения.

Обрабатывает ItemPurchased – добавляет запись.

Предоставляет API для получения инвентаря (для отображения в профиле).

### 2.7. Booster Service
Управляет активными бустерами пользователя: тип бустера (опыт/деньги), коэффициент, время начала и длительность.

Подписан на ItemPurchased, если товар – бустер – активирует его (создаёт запись активного бустера с таймером).

Предоставляет API GetActiveMultipliers(userId) для Reward и Character сервисов, чтобы при начислении учесть все активные бустеры.

Может сам истекать бустеры по фоновому механизму (Quartz.NET) и публиковать BoosterExpired.

## 3. Взаимодействие сервисов (сценарии)
### 3.1. Выполнение задачи
Клиент вызывает POST /api/tasks/{id}/complete → Task Service.

Task Service меняет статус, публикует TaskCompleted.

Reward Service получает TaskCompleted:

вызывает Booster Service GET /boosters/multipliers?userId=...&type=Currency (HTTP/gRPC),

вычисляет награду: base * multiplier,

сохраняет баланс, публикует CurrencyAwarded.

Character Service получает TaskCompleted (или ExperienceGained отдельно от Reward, либо тоже может быть в событии):

вызывает Booster Service для мультипликатора опыта,

начисляет опыт в характеристику CategoryId.

Task Service проверяет все задачи дня – если все выполнены, публикует AllDailyTasksCompleted.

Reward Service начисляет алмазы.

### 3.2. Покупка бустера
Клиент POST /api/store/purchase { itemId } → Store Service.

Store Service запускает распределённую транзакцию (saga) через MassTransit:

отправляет ReserveFunds в Reward Service,

Reward Service проверяет баланс, резервирует/списывает, отвечает FundsReserved,

Store Service генерирует ItemPurchased,

Inventory Service принимает ItemPurchased и добавляет предмет,

для бустера Booster Service активирует эффект по ItemPurchased.

В случае ошибок на любом этапе – компенсирующие действия.

### 3.3. Календарь и дашборд (чтение)
Task Service предоставляет API для получения задач по диапазону дат.

Для дашборда, объединяющего баланс, текущие бустеры и задачи дня, фронтенд делает несколько запросов к разным сервисам либо используется BFF (Backend for Frontend) на стороне API Gateway, агрегирующий ответы.

## 4. Технологический стек
Компонент	Технология
Язык	C# 12, .NET 8
Веб-фреймворк	ASP.NET Core Minimal API / Controllers
Коммуникация синхронная	REST (JSON) + gRPC для критичных вызовов (Booster)
Асинхронная шина	RabbitMQ + MassTransit
БД	PostgreSQL для каждого сервиса
ORM	Entity Framework Core (Code First)
Аутентификация	ASP.NET Core Identity + JWT (или Duende IdentityServer)
API Gateway	YARP (Reverse Proxy)
CQRS/Mediator	MediatR (внутри сервисов)
Фоновые задачи	Quartz.NET для истекающих бустеров
Контейнеризация	Docker, оркестрация – Kubernetes
Логирование, трейсинг	Serilog + OpenTelemetry, Jaeger
Мониторинг	Prometheus, Grafana
5. Детализация моделей и API (ключевые сущности)
Character Service
CharacterAttribute: Id, UserId, Name, Experience, Level.
API: POST /attributes, GET /attributes, GET /profile (агрегирует инвентарь через Inventory Service).

Task Service
TaskItem: Id, UserId, Title, DueDate, CategoryId, Difficulty, IsImportant, IsCompleted.
API: GET /tasks?date=, POST /tasks, PUT /tasks/{id}, POST /tasks/{id}/complete.

Reward Service
Wallet: UserId, Gold, Silver, Bronze, Diamonds.
API: GET /wallet, внутренний POST /wallet/debit для саг.

Store Service
CatalogItem: Id, Name, Type, CostGold, CostDiamonds, EffectJson.
API: GET /store/items, POST /store/purchase.

Booster Service
ActiveBooster: Id, UserId, BoosterType, Multiplier, ExpiresAt.
API: GET /boosters/active, GET /boosters/multipliers?userId=&type=.

Inventory Service
UserItem: Id, UserId, ItemId, Quantity.
API: GET /inventory.

## 6. Безопасность и устойчивость
JWT токен передаётся через API Gateway во все сервисы; сервисы валидируют его самостоятельно (доверенный issuer).

Саги с компенсациями для покупок гарантируют консистентность.

Повторные доставки событий обрабатываются идемпотентно (по ID события/корреляции).

БД каждого сервиса отделена, миграции управляются через CI/CD.

## 7. Эволюция для мультиплеера
Clan Service: создание/роспуск кланов, участники, клановые задачи/цели. Общается через события о вкладе в клан.

Duel Service: пошаговые PvP-сражения на основе характеристик. Использует SignalR для real-time взаимодействия. Может вызывать Character Service для получения статов и записывать историю дуэлей.

API Gateway остаётся точкой входа, добавляются новые маршруты.
