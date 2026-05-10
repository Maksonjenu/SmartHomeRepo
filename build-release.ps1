#!/usr/bin/env pwsh

# SmartHome Release Script
# Упаковывает приложение для распространения студентам

$releaseDir = "releases"
$version = "1.0.0"
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmm"

if (-not (Test-Path $releaseDir)) {
    New-Item -ItemType Directory -Path $releaseDir | Out-Null
}

Write-Host "🚀 SmartHome Release Builder v$version" -ForegroundColor Cyan
Write-Host "📦 Создаю релизные пакеты..." -ForegroundColor Yellow

# 1. Windows Self-Contained
Write-Host "`n📦 Упаковываю Windows Self-Contained..." -ForegroundColor Cyan

$winPath = "$releaseDir/SmartHomeServer-win-x64-$timestamp"
dotnet publish -c Release -r win-x64 --self-contained --no-restore -o $winPath/publish

if ($LASTEXITCODE -eq 0) {
    # Создаем батник для удобного запуска
    @"
@echo off
cd /d "%~dp0"
SmartHomeRepo.exe
pause
"@ | Out-File -Encoding ASCII "$winPath/run.bat"

    # Архивируем
    Compress-Archive -Path "$winPath/publish" -DestinationPath "$releaseDir/SmartHomeServer-win-x64-$timestamp.zip" -Force
    Remove-Item -Recurse -Force $winPath
    
    Write-Host "✅ Windows .exe готов: SmartHomeServer-win-x64-$timestamp.zip" -ForegroundColor Green
} else {
    Write-Host "❌ Ошибка при сборке Windows версии" -ForegroundColor Red
}

# 2. Linux x64
Write-Host "`n📦 Упаковываю Linux x64..." -ForegroundColor Cyan

$linuxPath = "$releaseDir/SmartHomeServer-linux-x64-$timestamp"
dotnet publish -c Release -r linux-x64 --self-contained --no-restore -o $linuxPath/publish

if ($LASTEXITCODE -eq 0) {
    # Создаем bash скрипт
    @"
#!/bin/bash
cd "$$(dirname "$$0")"
./SmartHomeRepo
"@ | Out-File -Encoding ASCII "$linuxPath/run.sh"
    
    Compress-Archive -Path "$linuxPath/publish" -DestinationPath "$releaseDir/SmartHomeServer-linux-x64-$timestamp.zip" -Force
    Remove-Item -Recurse -Force $linuxPath
    
    Write-Host "✅ Linux версия готова: SmartHomeServer-linux-x64-$timestamp.zip" -ForegroundColor Green
} else {
    Write-Host "❌ Ошибка при сборке Linux версии" -ForegroundColor Red
}

# 3. macOS arm64
Write-Host "`n📦 Упаковываю macOS arm64..." -ForegroundColor Cyan

$macPath = "$releaseDir/SmartHomeServer-macos-arm64-$timestamp"
dotnet publish -c Release -r osx-arm64 --self-contained --no-restore -o $macPath/publish

if ($LASTEXITCODE -eq 0) {
    # Создаем bash скрипт
    @"
#!/bin/bash
cd "$$(dirname "$$0")"
./SmartHomeRepo
"@ | Out-File -Encoding ASCII "$macPath/run.sh"
    
    Compress-Archive -Path "$macPath/publish" -DestinationPath "$releaseDir/SmartHomeServer-macos-arm64-$timestamp.zip" -Force
    Remove-Item -Recurse -Force $macPath
    
    Write-Host "✅ macOS версия готова: SmartHomeServer-macos-arm64-$timestamp.zip" -ForegroundColor Green
} else {
    Write-Host "⚠️  Пропускаю macOS (может не поддерживаться на Windows)" -ForegroundColor Yellow
}

# 4. Docker Image
Write-Host "`n📦 Собираю Docker образ..." -ForegroundColor Cyan

$dockerTag = "smarthome-server:$version-$timestamp"
docker build -t $dockerTag -t "smarthome-server:latest" .

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Docker образ готов: $dockerTag" -ForegroundColor Green
    
    # Сохраняем в tar
    docker save $dockerTag -o "$releaseDir/smarthome-server-$timestamp.tar"
    Write-Host "   Сохранен в: smarthome-server-$timestamp.tar" -ForegroundColor Green
}

# 5. Создаем README
Write-Host "`n📝 Создаю документацию..." -ForegroundColor Cyan

Copy-Item -Path "DEPLOYMENT.md" -Destination "$releaseDir/README.md" -Force

# Финальный отчет
Write-Host "`n" -ForegroundColor Green
Write-Host "✅ РЕЛИЗ ГОТОВ!" -ForegroundColor Green
Write-Host "`n📂 Файлы в папке: $releaseDir/" -ForegroundColor Cyan
Get-ChildItem -Path $releaseDir | ForEach-Object {
    $size = if ($_.PSIsContainer) { "папка" } else { "{0:N0} KB" -f ($_.Length / 1KB) }
    Write-Host "   • $($_.Name) - $size" -ForegroundColor White
}

Write-Host "`n💡 Для быстрого запуска:" -ForegroundColor Yellow
Write-Host "   Windows: Распакуйте .zip и запустите run.bat" -ForegroundColor Gray
Write-Host "   Docker: docker run -p 5000:5000 smarthome-server:latest" -ForegroundColor Gray

Write-Host "`n✨ Готово! Передавайте студентам 🎓" -ForegroundColor Green
