# ==================================================
# Dockerfile para SQL Server con inicialización automática
# Los Ladrillos S.A. - Sistema de Proyección de Ventas
# ==================================================

FROM mcr.microsoft.com/azure-sql-edge:latest

# Variables de entorno
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=LosLadrillos2024!
ENV MSSQL_PID=Developer

# Instalar Python y dependencias para pyodbc
USER root
RUN apt-get update && \
    apt-get install -y python3 python3-pip unixodbc-dev && \
    pip3 install --no-cache-dir pyodbc && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Crear directorio para scripts
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

# Copiar scripts SQL
COPY 01-init.sql .
COPY 02-functions.sql .
COPY 03-data.sql .
COPY 04-procedures.sql .
COPY entrypoint.sh .

# Dar permisos de ejecución
RUN chmod +x entrypoint.sh

# Exponer puerto
EXPOSE 1433

# Ejecutar script de entrada personalizado
CMD ["/bin/bash", "./entrypoint.sh"]
