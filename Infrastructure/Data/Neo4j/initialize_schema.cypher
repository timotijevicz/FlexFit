// Constraints for unique identifiers
CREATE CONSTRAINT member_id IF NOT EXISTS FOR (m:Member) REQUIRE m.id IS UNIQUE;
CREATE CONSTRAINT card_id IF NOT EXISTS FOR (c:Card) REQUIRE c.id IS UNIQUE;
CREATE CONSTRAINT resource_id IF NOT EXISTS FOR (r:Resource) REQUIRE r.id IS UNIQUE;
CREATE CONSTRAINT employee_id IF NOT EXISTS FOR (e:Employee) REQUIRE e.id IS UNIQUE;
CREATE CONSTRAINT gym_id IF NOT EXISTS FOR (g:Gym) REQUIRE g.id IS UNIQUE;
CREATE CONSTRAINT booking_id IF NOT EXISTS FOR (b:Booking) REQUIRE b.id IS UNIQUE;
CREATE CONSTRAINT penalty_id IF NOT EXISTS FOR (p:Penalty) REQUIRE p.id IS UNIQUE;

// Relationship structure
// (:Member)-[:HAS_CARD]->(:Card)
// (:Member)-[:BOOKED]->(:Resource)
// (:Employee)-[:CHECKED]->(:Member)
// (:Resource)-[:BELONGS_TO]->(:Gym)
// (:Booking)-[:FOR]->(:Resource)
// (:Penalty)-[:ASSIGNED_TO]->(:Member)
// (:Card)-[:VALID_IN]->(:Gym)

// Note: This script sets up the schema. Data should be populated via the application or data import.
