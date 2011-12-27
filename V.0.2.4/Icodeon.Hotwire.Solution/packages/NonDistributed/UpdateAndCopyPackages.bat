rd "HttpClient.0.6.0" /S /Q
rd "JsonValue.0.6.0" /S /Q
nuget.exe install httpclient
copy %WinDir%\System32\InetSrv\Microsoft.Web.Administration.dll Microsoft.Web.Administration.6.1.7601\Microsoft.Web.Administration.dll