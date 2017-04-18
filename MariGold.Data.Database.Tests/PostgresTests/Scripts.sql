CREATE TABLE person
(
    id integer NOT NULL,
    name text,
    date_of_birth date,
    ssn bigint,
    bank_account numeric,
    no_of_cars smallint,
    is_premium boolean,
    CONSTRAINT "person_pkey" PRIMARY KEY ("id")
)

INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (1, 'James', '19731101', 1000001, 75000.00, 2, false);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (2, 'Tomy', '19840617', 1000002, 115000.00, 5, true);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (3, 'Max', '19950119', 1000007, 500.00, 0, false);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (4, 'Matthew', '19651212', 1000010, 50000.00, 0, true);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (5, 'Thomas', '20010210', 1000011, 150000.00, 1, true);