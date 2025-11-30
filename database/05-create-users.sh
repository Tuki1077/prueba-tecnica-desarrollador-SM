#!/bin/bash

echo "Esperando que el backend y la base de datos estén listos..."
sleep 30

MAX_RETRIES=30
RETRY_COUNT=0

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if curl -s http://backend:5001/health > /dev/null 2>&1; then
        echo "Backend disponible, creando usuarios..."
        sleep 5
        
        # Crear usuario gerente con contraseña: gerente123
        curl -X POST http://backend:5001/api/auth/register \
          -H "Content-Type: application/json" \
          -d '{"nombreUsuario":"gerente","password":"gerente123","rol":"GERENTE"}' \
          2>/dev/null
        
        echo ""
        echo "Usuario gerente creado"
        
        # Crear usuario admin con contraseña: admin123
        curl -X POST http://backend:5001/api/auth/register \
          -H "Content-Type: application/json" \
          -d '{"nombreUsuario":"admin","password":"admin123","rol":"ADMIN"}' \
          2>/dev/null
        
        echo ""
        echo "Usuario admin creado"
        echo "Usuarios creados exitosamente"
        exit 0
    fi
    
    RETRY_COUNT=$((RETRY_COUNT + 1))
    echo "Esperando backend... ($RETRY_COUNT/$MAX_RETRIES)"
    sleep 2
done

echo "Error: Backend no disponible después de $MAX_RETRIES intentos"
exit 1
