-- =========================================================
-- StepUp: схема БД (SQL Server / T-SQL)
-- =========================================================

-- ---------------------------------------------------------
-- users
-- ---------------------------------------------------------
CREATE TABLE dbo.users (
    id                UNIQUEIDENTIFIER NOT NULL,
    full_name         NVARCHAR(200)    NOT NULL,
    email             NVARCHAR(256)    NOT NULL,
    password          NVARCHAR(256)    NOT NULL,           -- TODO: хранить хеш, а не пароль
    birth_date        DATETIME2        NOT NULL,
    gender            NVARCHAR(20)     NOT NULL,           -- enum Gender: 'Male' / 'Female'
    weight            FLOAT            NOT NULL,
    height            FLOAT            NOT NULL,
    calorie_plan      INT              NOT NULL,
    carbon_percent    INT              NOT NULL,
    protein_percent   INT              NOT NULL,
    fat_percent       INT              NOT NULL,
    sport_level       NVARCHAR(20)     NOT NULL,           -- enum SportLevel
    registration_date DATETIME2        NOT NULL,

    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT ak_users_email UNIQUE (email),

    CONSTRAINT ck_users_full_name   CHECK (LEN(LTRIM(RTRIM(full_name))) > 0),
    CONSTRAINT ck_users_email       CHECK (LEN(LTRIM(RTRIM(email))) > 0),
    CONSTRAINT ck_users_password    CHECK (LEN(LTRIM(RTRIM(password))) > 0),

    CONSTRAINT ck_users_gender      CHECK (gender IN ('Male', 'Female')),
    CONSTRAINT ck_users_sport_level CHECK (sport_level IN ('No', 'Low', 'Mid', 'High', 'Extreme')),

    CONSTRAINT ck_users_weight       CHECK (weight  >= 30  AND weight  <= 300),
    CONSTRAINT ck_users_height       CHECK (height  >= 100 AND height  <= 250),
    CONSTRAINT ck_users_calorie_plan CHECK (calorie_plan >= 1300),

    CONSTRAINT ck_users_protein_pct CHECK (protein_percent BETWEEN 1 AND 100),
    CONSTRAINT ck_users_fat_pct     CHECK (fat_percent     BETWEEN 1 AND 100),
    CONSTRAINT ck_users_carbon_pct  CHECK (carbon_percent  BETWEEN 1 AND 100),
    CONSTRAINT ck_users_macros_sum  CHECK (protein_percent + fat_percent + carbon_percent = 100)
);
GO

-- ---------------------------------------------------------
-- day_results
-- ---------------------------------------------------------
CREATE TABLE dbo.day_results (
    id            UNIQUEIDENTIFIER NOT NULL,
    user_id       UNIQUEIDENTIFIER NOT NULL,
    [date]        DATETIME2        NOT NULL,               -- date — зарезервированное слово, экранируем
    calorie_plan  INT              NOT NULL,
    weight        FLOAT            NOT NULL,

    CONSTRAINT pk_day_results PRIMARY KEY (id),
    CONSTRAINT ak_day_results_user_date UNIQUE (user_id, [date]),

    CONSTRAINT fk_day_results_users
        FOREIGN KEY (user_id) REFERENCES dbo.users(id) ON DELETE CASCADE,

    CONSTRAINT ck_day_results_calorie_plan CHECK (calorie_plan >= 1300),
    CONSTRAINT ck_day_results_weight       CHECK (weight >= 30 AND weight <= 300)
);
GO

CREATE INDEX ix_day_results_user_id ON dbo.day_results(user_id);
CREATE INDEX ix_day_results_date    ON dbo.day_results([date]);
GO

-- ---------------------------------------------------------
-- meals
-- ---------------------------------------------------------
CREATE TABLE dbo.meals (
    id             UNIQUEIDENTIFIER NOT NULL,
    day_result_id  UNIQUEIDENTIFIER NOT NULL,
    meal_type      NVARCHAR(20)     NOT NULL,

    CONSTRAINT pk_meals PRIMARY KEY (id),
    CONSTRAINT ak_meals_day_result_type UNIQUE (day_result_id, meal_type),

    CONSTRAINT fk_meals_day_results
        FOREIGN KEY (day_result_id) REFERENCES dbo.day_results(id) ON DELETE CASCADE,

    CONSTRAINT ck_meals_meal_type CHECK (meal_type IN ('Breakfast', 'Lunch', 'Dinner', 'Snack'))
);
GO

CREATE INDEX ix_meals_day_result_id ON dbo.meals(day_result_id);
GO

-- ---------------------------------------------------------
-- food_items
-- ---------------------------------------------------------
CREATE TABLE dbo.food_items (
    id              UNIQUEIDENTIFIER NOT NULL,
    meal_id         UNIQUEIDENTIFIER NOT NULL,
    product_id      NVARCHAR(100)    NOT NULL,             -- ссылка на OpenFoodFacts, без FK
    name            NVARCHAR(300)    NOT NULL,
    barcode         NVARCHAR(50)     NULL,
    amount_g        FLOAT            NOT NULL,

    kcal_100g       FLOAT            NOT NULL,
    protein_100g    FLOAT            NOT NULL,
    fat_100g        FLOAT            NOT NULL,
    carbon_100g     FLOAT            NOT NULL,

    CONSTRAINT pk_food_items PRIMARY KEY (id),

    CONSTRAINT fk_food_items_meals
        FOREIGN KEY (meal_id) REFERENCES dbo.meals(id) ON DELETE CASCADE,

    CONSTRAINT ck_food_items_amount_g      CHECK (amount_g >= 0),
    CONSTRAINT ck_food_items_product_id    CHECK (LEN(LTRIM(RTRIM(product_id))) > 0),
    CONSTRAINT ck_food_items_name          CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT ck_food_items_kcal_100g     CHECK (kcal_100g    >= 0),
    CONSTRAINT ck_food_items_protein_100g  CHECK (protein_100g >= 0),
    CONSTRAINT ck_food_items_fat_100g      CHECK (fat_100g     >= 0),
    CONSTRAINT ck_food_items_carbon_100g   CHECK (carbon_100g  >= 0)
);
GO

CREATE INDEX ix_food_items_meal_id    ON dbo.food_items(meal_id);
CREATE INDEX ix_food_items_product_id ON dbo.food_items(product_id);
CREATE INDEX ix_food_items_barcode    ON dbo.food_items(barcode) WHERE barcode IS NOT NULL;
GO

-- ---------------------------------------------------------
-- sports
-- ---------------------------------------------------------
CREATE TABLE dbo.sports (
    id             UNIQUEIDENTIFIER NOT NULL,
    day_result_id  UNIQUEIDENTIFIER NOT NULL,
    name           NVARCHAR(200)    NOT NULL,
    burnt_kcals    INT              NOT NULL,

    CONSTRAINT pk_sports PRIMARY KEY (id),

    CONSTRAINT fk_sports_day_results
        FOREIGN KEY (day_result_id) REFERENCES dbo.day_results(id) ON DELETE CASCADE,

    CONSTRAINT ck_sports_name        CHECK (LEN(LTRIM(RTRIM(name))) > 0),
    CONSTRAINT ck_sports_burnt_kcals CHECK (burnt_kcals >= 0)
);
GO

CREATE INDEX ix_sports_day_result_id ON dbo.sports(day_result_id);
GO