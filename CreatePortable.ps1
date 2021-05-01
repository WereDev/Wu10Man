$compress = @{
  Path = "Wu10Man\bin\Release\Wu10Man.exe",
        "Wu10Man\bin\Release\app.settings.json",
        "Wu10Man\bin\Release\nlog.config",
        "Wu10Man\bin\Release\*.dll"
  CompressionLevel = "Optimal"
  DestinationPath = "Publish\Wu10Man_Portable.zip"
}
Compress-Archive @compress -Force