@echo off
chcp 65001 >nul

echo ============================================
echo   vrc_yt_dlp_bridge インストール / Install
echo ============================================
echo.
rem Tools\yt-dlp.exe は VRChat 公式の yt-dlp が置かれている場合もあるため、ファイルの有無ではなく
rem バージョン情報でブリッジかどうかを判定する。
rem   ProductName = vrc_yt_dlp_bridge : 現行のブリッジ
rem   CompanyName = yt-dlp かつ yt-dlp_bridge フォルダあり : Product 未設定だった旧バージョンのブリッジ (.NET の既定値)
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$toolsDir = \"$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\";" ^
  "$exePath = Join-Path $toolsDir 'yt-dlp.exe';" ^
  "$bridgeDir = Join-Path $toolsDir 'yt-dlp_bridge';" ^
  "$isBridge = $false;" ^
  "if (Test-Path $exePath) {" ^
  "  $vi = (Get-Item $exePath).VersionInfo;" ^
  "  $isBridge = ($vi.ProductName -eq 'vrc_yt_dlp_bridge') -or (($vi.CompanyName -eq 'yt-dlp') -and (Test-Path $bridgeDir))" ^
  "};" ^
  "if ($isBridge) {" ^
  "  $oldVer = $vi.ProductVersion;" ^
  "  Write-Host \"既にインストールされています (ver $oldVer)\";" ^
  "  Write-Host 'Already installed. It will be updated.';" ^
  "  Write-Host '最新版に更新します。'" ^
  "} elseif (Test-Path $exePath) {" ^
  "  Write-Host 'VRChat 公式の yt-dlp.exe が見つかりました。ブリッジに置き換えます。';" ^
  "  Write-Host 'Found the VRChat official yt-dlp.exe. It will be replaced with the bridge.'" ^
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
