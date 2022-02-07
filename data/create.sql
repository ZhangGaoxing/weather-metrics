CREATE DATABASE "WeatherMetrics"
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8';

CREATE TABLE metrics (
   time TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT 'now()',
   device_id VARCHAR(50) NULL,
   weather_type VARCHAR(50) NULL,
   temperature DECIMAL(5, 2) NULL,
   humidity DECIMAL(5, 2) NULL,
   pressure DECIMAL(8, 2) NULL,
   image_base64 TEXT NULL
);

SELECT create_hypertable('metrics', 'time');
