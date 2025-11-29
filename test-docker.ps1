#!/usr/bin/env pwsh
# Script para testear el despliegue de Firmeza con Docker

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Firmeza - Script de Testing Docker  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Agregar Docker al PATH
$env:Path += ";C:\Program Files\Docker\Docker\resources\bin"

# Verificar si Docker está instalado
Write-Host "1. Verificando instalación de Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "   ✓ $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Docker no está instalado o no se encuentra en el PATH" -ForegroundColor Red
    exit 1
}

# Verificar si Docker está corriendo
Write-Host "2. Verificando si Docker Desktop está corriendo..." -ForegroundColor Yellow
try {
    docker info | Out-Null
    Write-Host "   ✓ Docker Desktop está corriendo" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Docker Desktop NO está corriendo" -ForegroundColor Red
    Write-Host "   Por favor, inicia Docker Desktop e intenta nuevamente." -ForegroundColor Yellow
    exit 1
}

# Menú de opciones
Write-Host ""
Write-Host "¿Qué deseas hacer?" -ForegroundColor Cyan
Write-Host "1. Ejecutar pruebas unitarias localmente (sin Docker)"
Write-Host "2. Despliegue completo con Docker Compose"
Write-Host "3. Ver logs de los servicios"
Write-Host "4. Detener todos los servicios"
Write-Host "5. Ver estado de contenedores"
Write-Host "6. Limpiar todo (contenedores e imágenes)"
Write-Host ""

$choice = Read-Host "Ingresa tu elección (1-6)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "Ejecutando pruebas unitarias..." -ForegroundColor Yellow
        dotnet test Firmeza.Tests/Firmeza.Tests.csproj --logger "console;verbosity=detailed"
    }
    
    "2" {
        Write-Host ""
        Write-Host "Iniciando despliegue completo con Docker Compose..." -ForegroundColor Yellow
        Write-Host "Esto construirá las imágenes y levantará todos los servicios." -ForegroundColor Cyan
        Write-Host ""
        docker compose up --build
    }
    
    "3" {
        Write-Host ""
        Write-Host "¿Qué servicio deseas ver?" -ForegroundColor Cyan
        Write-Host "1. API (FirmezaAPI)"
        Write-Host "2. Blazor (firmeza-blazor)"
        Write-Host "3. Frontend (firmeza-frontend)"
        Write-Host "4. Tests"
        Write-Host "5. Todos los servicios"
        $serviceChoice = Read-Host "Ingresa tu elección (1-5)"
        
        switch ($serviceChoice) {
            "1" { docker compose logs -f api }
            "2" { docker compose logs -f firmeza-blazor }
            "3" { docker compose logs -f firmeza-frontend }
            "4" { docker compose logs -f tests }
            "5" { docker compose logs -f }
        }
    }
    
    "4" {
        Write-Host ""
        Write-Host "Deteniendo todos los servicios..." -ForegroundColor Yellow
        docker compose down
        Write-Host "✓ Servicios detenidos exitosamente" -ForegroundColor Green
    }
    
    "5" {
        Write-Host ""
        Write-Host "Estado de contenedores Docker:" -ForegroundColor Cyan
        docker ps -a
    }
    
    "6" {
        Write-Host ""
        Write-Host "⚠️  ADVERTENCIA: Esto eliminará todos los contenedores e imágenes." -ForegroundColor Red
        $confirm = Read-Host "¿Estás seguro? (S/N)"
        if ($confirm -eq "S" -or $confirm -eq "s") {
            Write-Host "Limpiando contenedores e imágenes..." -ForegroundColor Yellow
            docker compose down --rmi all -v
            Write-Host "✓ Limpieza completada" -ForegroundColor Green
        } else {
            Write-Host "Operación cancelada" -ForegroundColor Yellow
        }
    }
    
    default {
        Write-Host "Opción inválida" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Para más información consulta el README.md" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
