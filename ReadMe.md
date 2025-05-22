To run redis
docker compose up -d

add-migration Initial-Migration -StartupProject API -Project Infrastructure
update-database 0 -StartupProject API -Project Infrastructure (para revertir la primer migracion impactada en la BD)
update-database -StartupProject API
remove-migration -StartupProject API -Project Infrastructure