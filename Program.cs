await Task.WhenAll(
    Clients.DownloadDeno(),
    Clients.YtDlpDownloadAndUpdate()
);
await Clients.ExecYtDlp(args);