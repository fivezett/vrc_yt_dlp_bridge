@echo off
chcp 65001 >nul

echo ============================================
echo   vrc_yt_dlp_bridge アンインストール / Uninstall
echo ============================================
echo.
echo 以下のファイルを削除します。
echo The following files will be removed:
echo.
echo   %%LocalAppData%%Low\VRChat\VRChat\Tools\yt-dlp.exe
echo   %%LocalAppData%%Low\VRChat\VRChat\Tools\yt-dlp_bridge\
echo.
echo 続行するには何かキーを押してください...
pause >nul

echo.
echo 削除しています... / Removing...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$toolsDir = \"$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\";" ^
  "$exePath = Join-Path $toolsDir 'yt-dlp.exe';" ^
  "$bridgeDir = Join-Path $toolsDir 'yt-dlp_bridge';" ^
  "$found = $false;" ^
  "if (Test-Path $exePath) {" ^
  "  Remove-Item -Path $exePath -Force;" ^
  "  $found = $true" ^
  "};" ^
  "if (Test-Path $bridgeDir) {" ^
  "  Remove-Item -Path $bridgeDir -Recurse -Force;" ^
  "  $found = $true" ^
  "};" ^
  "if ($found) {" ^
  "  Write-Host '削除しました。';" ^
  "  Write-Host 'Uninstall complete.'" ^
  "} else {" ^
  "  Write-Host 'インストールされていません。';" ^
  "  Write-Host 'Not installed.'" ^
  "};" ^
  "Write-Host '';" ^
  "Write-Host '完了！ このウィンドウは閉じて大丈夫です。';" ^
  "Write-Host 'Done! You may close this window.'"

echo.
pause
