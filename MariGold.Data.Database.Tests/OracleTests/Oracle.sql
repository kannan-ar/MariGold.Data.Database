CREATE TABLE person 
(
  id NUMBER NOT NULL 
, name VARCHAR2(50 BYTE) NOT NULL 
, date_of_birth DATE 
, ssn NUMBER 
, bank_account NUMBER(18, 2) 
, no_of_cars NUMBER(1, 0) 
, is_premium NUMBER(1, 0) 
, CONSTRAINT PERSON_PK PRIMARY KEY 
  (
    id
  )
)

INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (1, 'James', TO_DATE('19731101', 'yyyymmdd'), 1000001, 75000.00, 2, 0);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (2, 'Tomy', TO_DATE('19840617', 'yyyymmdd'), 1000002, 115000.00, 5, 1);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (3, 'Max', TO_DATE('19950119', 'yyyymmdd'), 1000007, 500.00, 0, 0);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (4, 'Matthew', TO_DATE('19651212', 'yyyymmdd'), 1000010, 50000.00, 0, 1);
INSERT INTO person(id, name, date_of_birth, ssn, bank_account, no_of_cars, is_premium) VALUES (5, 'Thomas', TO_DATE('20010210', 'yyyymmdd'), 1000011, 150000.00, 1, 1);
