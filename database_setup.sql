/* 
Setup Docker:
    1. docker run --name tourplanner -p 5432:5432 -e POSTGRES_PASSWORD=******** -d postgres
    2. docker exec -it tourplanner psql -U postgres
    3. CREATE database tourplanner;
    4. \c tourplanner
*/

DROP TABLE tour_logs;
DROP TABLE tours;

CREATE TABLE IF NOT EXISTS tours (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(255) NOT NULL,
    from_location VARCHAR(50) NOT NULL,
    to_location VARCHAR(50) NOT NULL,
    transportation_type VARCHAR(50) NOT NULL,
    distance INT NOT NULL,
    estimated_time INT NOT NULL,
    route_information VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS tour_logs (
    id SERIAL PRIMARY KEY,
    tour_id INT NOT NULL,
    FOREIGN KEY (tour_id) REFERENCES tours(id),
    logdate TIMESTAMP NOT NULL,
    comment VARCHAR(255) NOT NULL,
    difficulty VARCHAR(50) NOT NULL,
    total_distance INT NOT NULL,
    total_time INT NOT NULL,
    rating INT NOT NULL
);

/* Tours */
INSERT INTO tours (name, description, from_location, to_location, transportation_type, distance, estimated_time, route_information) VALUES ('Ort1', 'Description1', 'From1', 'To1', 'Transportation1', 1000, 5, '');
INSERT INTO tours (name, description, from_location, to_location, transportation_type, distance, estimated_time, route_information) VALUES ('Test Ort', 'Test Description', 'Test from', 'Test to', 'Test Transportation', 2000, 10, '');

/* Tour logs */
INSERT INTO tour_logs (tour_id, logdate, comment, difficulty, total_distance, total_time, rating) VALUES (1, '2025-07-01', 'Comment1', 'Easy', 1000, 23, 10);
INSERT INTO tour_logs (tour_id, logdate, comment, difficulty, total_distance, total_time, rating) VALUES (1, '2025-07-02', 'Comment2', 'Medium', 2000, 23, 9);
INSERT INTO tour_logs (tour_id, logdate, comment, difficulty, total_distance, total_time, rating) VALUES (2, '2025-07-03', 'Comment3', 'Hard', 3000, 23, 8);
