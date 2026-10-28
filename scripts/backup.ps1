# Local pre-migration DB backup. Usage: .\scripts\backup.ps1
# Restore (destructive): pg_restore -h <host> -U <user> -d <db> --clean --if-exists <file>

$bin = 'C:\Program Files\PostgreSQL\18\bin'
$line = dotnet user-secrets list --project "$PSScriptRoot\..\BackendApi" |
  Where-Object { $_ -like 'ConnectionStrings:*' } | Select-Object -First 1
if (-not $line) { Write-Error 'Connection string not found in user-secrets.'; exit 1 }

$kv = @{}
foreach ($p in ($line -split ' = ', 2)[1].Split(';')) {
  if ($p -match '^\s*([^=]+?)\s*=\s*(.*)$') { $kv[$matches[1].ToLower()] = $matches[2] }
}
$dbHost = if ($kv['host']) { $kv['host'] } else { $kv['server'] }
$port   = if ($kv['port']) { $kv['port'] } else { '5432' }
$user   = if ($kv['username']) { $kv['username'] } else { $kv['user id'] }
$db     = $kv['database']

$dir = Join-Path $HOME 'db-backups'
New-Item -ItemType Directory -Force $dir | Out-Null
$file = Join-Path $dir ('{0}_{1}.dump' -f $db, (Get-Date -Format 'yyyyMMdd_HHmmss'))

$env:PGPASSWORD = $kv['password']
try {
  & "$bin\pg_dump.exe" -h $dbHost -p $port -U $user -d $db -F c -f $file
  if ($LASTEXITCODE -ne 0) { Write-Error 'pg_dump failed.'; exit 1 }
  & "$bin\pg_restore.exe" -l $file | Out-Null
  if ($LASTEXITCODE -ne 0) { Write-Error 'Backup is not readable.'; exit 1 }
} finally {
  Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
}
Write-Host ("Backup OK: {0} ({1} KB)" -f $file, [math]::Round((Get-Item $file).Length / 1KB))