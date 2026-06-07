#!/bin/bash
# Wait for Postgres to be available then start the dotnet app
set -e

DB_CONN_STRING="${ConnectionStrings__DefaultConnection:-Host=db;Port=5432;Database=RealmOfLegendsDb;Username=postgres;Password=Your_password123}"
PGHOST=$(echo $DB_CONN_STRING | sed -n 's/.*Host=\([^;]*\).*/\1/p')
PGPORT=$(echo $DB_CONN_STRING | sed -n 's/.*Port=\([^;]*\).*/\1/p')
PGHOST=${PGHOST:-db}
PGPORT=${PGPORT:-5432}

echo "Waiting for Postgres at $PGHOST:$PGPORT..."
for i in {1..60}; do
  echo "Checking Postgres readiness (attempt $i)..."
  if pg_isready -h "$PGHOST" -p "$PGPORT" -U "postgres" >/dev/null 2>&1; then
    echo "Postgres is ready"
    break
  fi
  # show a brief status using psql if available
  if command -v psql >/dev/null 2>&1; then
    echo "pg_isready failed; trying psql connectivity check..."
    PSQL_CONN="postgresql://postgres:Your_password123@$PGHOST:$PGPORT/RealmOfLegendsDb"
    psql "$PSQL_CONN" -c '\\conninfo' >/dev/null 2>&1 && { echo "psql can connect"; break; } || echo "psql cannot connect yet"
  fi
  echo "Postgres not ready yet ($i/60). Sleeping 2s..."
  sleep 2
done

# Start the app
echo "Starting app"
exec dotnet RealmOfLegends.Web.dll
