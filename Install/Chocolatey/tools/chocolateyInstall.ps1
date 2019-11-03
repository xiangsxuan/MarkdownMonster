$packageName = 'markdownmonster'
$fileType = 'exe'
$url = 'https://github.com/RickStrahl/MarkdownMonsterReleases/raw/master/v1.19/MarkdownMonsterSetup-1.19.14.exe'

$silentArgs = '/VERYSILENT'
$validExitCodes = @(0)

Install-ChocolateyPackage "packageName" "$fileType" "$silentArgs" "$url"  -validExitCodes  $validExitCodes  -checksum "19F29888BD665846D385FD1742967534CB1C309E66CA558A00945B2E72458021" -checksumType "sha256"
