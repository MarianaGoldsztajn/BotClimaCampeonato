CREATE TABLE POSITIONS(
ID INT NOT NULL,
TEAMNAME VARCHAR(50) NOT NULL,
POSITION INT NOT NULL,
)

INSERT INTO POSITIONS VALUES (001, 'URUGUAY', 1);
INSERT INTO POSITIONS VALUES (002, 'BRASIL', 2);
INSERT INTO POSITIONS VALUES (003, 'ARGENTINA', 3);
INSERT INTO POSITIONS VALUES (004, 'COLOMBIA', 4);

SELECT * FROM POSITIONS;