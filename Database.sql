CREATE DATABASE PaymentDB;

CREATE TABLE PaymentHistory
(
    SenderName VARCHAR(50) NOT NULL,
    AmountInEuros DECIMAL,
    IBANNumber VARCHAR(22) NOT NULL,
    PaymentTime DATETIME
)
