@echo off
chcp 65001 >nul

echo ============================================
echo   vrc_yt_dlp_bridge インストール / Install
echo ============================================
echo.
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$toolsDir = \"$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\";" ^
  "$exePath = Join-Path $toolsDir 'yt-dlp.exe';" ^
  "if (Test-Path $exePath) {" ^
  "  $oldVer = (Get-Item $exePath).VersionInfo.ProductVersion;" ^
  "  Write-Host \"既にインストールされています (ver $oldVer)\";" ^
  "  Write-Host 'Already installed. It will be updated.';" ^
  "  Write-Host '最新版に更新します。'" ^
  "} else {" ^
  "  Write-Host '新しくインストールします。';" ^
  "  Write-Host 'A fresh install will be performed.'" ^
  "}"

echo.
echo 場所 / Location:
echo   %%LocalAppData%%Low\VRChat\VRChat\Tools\yt-dlp.exe

echo.
echo 続行するには何かキーを押してください...
pause >nul

echo.
echo ダウンロードしています... / Downloading...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$ErrorActionPreference = 'Stop';" ^
  "$toolsDir = \"$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\";" ^
  "$exePath = Join-Path $toolsDir 'yt-dlp.exe';" ^
  "try {" ^
  "  New-Item -ItemType Directory -Path $toolsDir -Force | Out-Null;" ^
  "  if (Test-Path $exePath) {" ^
  "    Set-ItemProperty -Path $exePath -Name IsReadOnly -Value $false" ^
  "  };" ^
  "  Invoke-WebRequest -Uri 'https://github.com/fivezett/vrc_yt_dlp_bridge/releases/latest/download/yt-dlp.exe' -OutFile $exePath;" ^
  "  Set-ItemProperty -Path $exePath -Name IsReadOnly -Value $true;" ^
  "  $newVer = (Get-Item $exePath).VersionInfo.ProductVersion;" ^
  "  Write-Host \"ver $newVer をインストールしました。 / Installed ver $newVer.\";" ^
  "  Write-Host '';" ^
  "  Write-Host '完了！ このウィンドウは閉じて大丈夫です。';" ^
  "  Write-Host 'Done! You may close this window.'" ^
  "} catch {" ^
  "  if (Test-Path $exePath) {" ^
  "    Set-ItemProperty -Path $exePath -Name IsReadOnly -Value $true" ^
  "  };" ^
  "  Write-Host '';" ^
  "  Write-Host \"エラー: ダウンロードに失敗しました。 / Error: Download failed.\" -ForegroundColor Red;" ^
  "  Write-Host $_.Exception.Message -ForegroundColor Red" ^
  "}"

echo.
pause
