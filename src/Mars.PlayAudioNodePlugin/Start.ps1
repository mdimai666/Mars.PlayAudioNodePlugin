$exe = "C:\\Users\\D\\Documents\\VisualStudio\\2023\\Blast\\Blast\\bin\\Debug\\net6.0\\Blast.exe"
$cfg = "C:\\Users\\D\\Documents\\VisualStudio\\2023\\Blast\\Blast\\appsettings.summer2023.local.json"

$env:ASPNETCORE_ENVIRONMENT="Development"

& $exe -cfg $cfg 