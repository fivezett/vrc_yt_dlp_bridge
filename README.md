# vrc_yt_dlp_bridge

VRChat のビデオプレイヤー向け [yt-dlp](https://github.com/yt-dlp/yt-dlp) ブリッジアプリケーション

## 📖 概要

VRChatでoffcialのyt-dlpを使用するためのブリッジアプリケーションです。  
yt-dlp 2025.11.12移行からJavaScriptランタイムの使用が推奨となったため作ってみました。  
本ソフトウェアは、VRChat公式と一切の関係性はありません。全て自己責任でお願いします。  
denoの自動取得とyt-dlpの自動更新機能を備えています。  

## 📝 ライセンス
本ソフトウェアはThe Unlicenseのもとで提供されています。  
denoおよびyt-dlpのライセンスについてはそれぞれのリポジトリをご参照ください。  


## ✨ 機能
- 🔄 **yt-dlp の自動ダウンロード・更新** - 初回起動時に自動ダウンロード、24時間ごとに更新チェックを行います。  
- 🦕 **Deno の自動ダウンロード** - JavaScript ランタイムとして最新版を自動取得します。  


## 📦 配置方法

### A. PowerShellで以下を実行
```powershell
New-Item -ItemType Directory -Path "$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools" -Force | Out-Null; Invoke-WebRequest -Uri "https://github.com/fivezett/vrc_yt_dlp_bridge/releases/latest/download/yt-dlp.exe" -OutFile "$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\yt-dlp.exe"; Set-ItemProperty -Path "$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\yt-dlp.exe" -Name IsReadOnly -Value $true
```

### B. 手動配置
1. ビルドした `yt-dlp.exe` を VRChat の Tools フォルダにコピー:
```
%LocalAppData%Low\VRChat\VRChat\Tools\yt-dlp.exe
```

2. プロパティから、`読み取り専用`を付与してください。


3. 初回実行時に以下が自動的にダウンロードされます:
   - `yt-dlp_bridge/yt-dlp.exe` - 本家 yt-dlp
   - `yt-dlp_bridge/deno.exe` - Deno ランタイム

---

## 🗑️ 削除方法
### A. PowerShellで以下を実行
```powershell
Remove-Item -Path "$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\yt-dlp.exe" -Force; Remove-Item -Path "$env:LOCALAPPDATA\..\LocalLow\VRChat\VRChat\Tools\yt-dlp_bridge" -Recurse -Force
```

### B. 手動削除
1. `yt-dlp.exe` と `yt-dlp_bridge` フォルダを削除してください。

---

## 📂 ファイル構成

```
%LocalAppData%Low\VRChat\VRChat\Tools\
├── yt-dlp.exe                     # このブリッジアプリケーション
└── yt-dlp_bridge/
    ├── yt-dlp.exe                 # 本家 yt-dlp
    ├── deno.exe                   # Deno ランタイム
    ├── yt-dlp_last_update.txt     # 最終更新日時
    ├── ytdlpbridge_log.txt        # 実行ログ
    ├── managementresource_log.txt # リソース管理ログ
    ├── tmp/                       # 一時ファイル
    └── home/                      # ホームディレクトリ
```

---

## ⚙️ 動作仕様
- VRChat から渡された引数をそのまま yt-dlp に転送します。  
- VRChatから渡された引数の他に以下のオプションを自動付与します:  
  - `--ignore-config` - 設定ファイルを無視  
  - `--js-runtimes deno:./deno.exe` - Deno を JavaScript ランタイムとして使用  
- VRChatから渡されるCache無効オプションは無視されます。 
- URL形式の出力のみを標準出力に返却します。  