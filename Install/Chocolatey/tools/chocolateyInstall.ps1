$packageName = 'markdownmonster'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/MarkdownMonsterReleases/raw/master/v1.20/MarkdownMonsterSetup-1.20.8.exe'

$silentArgs = '/VERYSILENT'
$validExitCodes = @(0)

Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "5BD6708D5D73C48CBB07B4F8F8CF363524292B913395F62B63DC1F939BDA0106" -checksumType "sha256"
