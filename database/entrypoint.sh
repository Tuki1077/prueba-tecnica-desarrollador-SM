#!/bin/bash

# Iniciar SQL Server en segundo plano
/opt/mssql/bin/sqlservr &

# Esperar a que SQL Server esté listo
echo "Esperando a que SQL Server inicie..."
sleep 30s

# Ejecutar scripts de inicialización
echo "Ejecutando scripts de inicialización..."

# Ejecutar scripts en orden
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i /docker-entrypoint-initdb.d/01-init.sql
if [ $? -eq 0 ]; then
    echo "✓ Script 01-init.sql ejecutado correctamente"
else
    echo "✗ Error ejecutando 01-init.sql"
fi

/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i /docker-entrypoint-initdb.d/02-functions.sql
if [ $? -eq 0 ]; then
    echo "✓ Script 02-functions.sql ejecutado correctamente"
else
    echo "✗ Error ejecutando 02-functions.sql"
fi

/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i /docker-entrypoint-initdb.d/03-data.sql
if [ $? -eq 0 ]; then
    echo "✓ Script 03-data.sql ejecutado correctamente"
else
    echo "✗ Error ejecutando 03-data.sql"
fi

/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i /docker-entrypoint-initdb.d/04-procedures.sql
if [ $? -eq 0 ]; then
    echo "✓ Script 04-procedures.sql ejecutado correctamente"
else
    echo "✗ Error ejecutando 04-procedures.sql"
fi

echo "Inicialización de base de datos completada"

# Mantener el proceso activo
wait
