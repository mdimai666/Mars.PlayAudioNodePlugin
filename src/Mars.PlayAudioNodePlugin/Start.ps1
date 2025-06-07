$exe = "..\\..\\..\\Mars\\src\\Mars.WebApp\\bin\\Debug\\net9.0\\Mars.exe"
$cfg = "..\\..\\..\\Mars\\src\\Mars.WebApp\\appsettings.local.json"

$env:ASPNETCORE_ENVIRONMENT="Development"

& $exe -cfg $cfg 
