CREATE TABLE public."Person"
(
    "Id" integer NOT NULL,
    "Name" text COLLATE pg_catalog."default",
    "DateOfBirth" date,
    "SSN" bigint,
    "BankAccount" numeric,
    "NoofCars" smallint,
    "IsPremium" boolean,
    CONSTRAINT "Person_pkey" PRIMARY KEY ("Id")
)

INSERT INTO public."Person"("Id", "Name", "DateOfBirth", "SSN", "BankAccount", "NoofCars", "IsPremium") VALUES (1, 'James', '19731101', 1000001, 75000.00, 2, false);
INSERT INTO public."Person"("Id", "Name", "DateOfBirth", "SSN", "BankAccount", "NoofCars", "IsPremium") VALUES (2, 'Tomy', '19840617', 1000002, 115000.00, 5, true)
INSERT INTO public."Person"("Id", "Name", "DateOfBirth", "SSN", "BankAccount", "NoofCars", "IsPremium") VALUES (3, 'Max', '19950119', 1000007, 500.00, 0, false)
INSERT INTO public."Person"("Id", "Name", "DateOfBirth", "SSN", "BankAccount", "NoofCars", "IsPremium") VALUES (4, 'Matthew', '19651212', 1000010, 50000.00, 0, true)
INSERT INTO public."Person"("Id", "Name", "DateOfBirth", "SSN", "BankAccount", "NoofCars", "IsPremium") VALUES (5, 'Thomas', '20010210', 1000011, 150000.00, 1, true)