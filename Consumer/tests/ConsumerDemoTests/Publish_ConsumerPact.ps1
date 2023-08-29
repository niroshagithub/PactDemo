# Pact Broker API endpoint to publish pacts
$pactBrokerBaseUrl = ${env:PACT_BROKER_BASE_URL}
$pactBrokerToken = ${env:PACT_BROKER_TOKEN}
if (-not $PactBrokerBaseUrl -or -not $PactBrokerToken) {
    Write-Host "Pact broker base url and token are not set up."
    exit 1
}

$consumerName = "WeatherForecast Consumer"
$version = ${env:CONSUMER_API_VERSION}
$branch = $(git rev-parse --abbrev-ref HEAD)
$pactFileRelativePath = "pacts"
$pactPath=(Resolve-Path -Relative $pactFileRelativePath).Path
$pactFileWindowsPath = $pactFileAbsolutePath -replace '/', '\'
$BRANCH=$(git rev-parse --abbrev-ref HEAD)

$PWD = (Get-Location).Path
$dockerPath = $PWD -replace '\\', '/' -replace ':', ''


$dockerCommand = @"
docker run --rm -v /${dockerPath}:/${dockerPath} -w /${dockerPath} `
    -e $pactBrokerBaseUrl `
    -e $pactBrokerToken `
    pactfoundation/pact-cli:0.50.0.28 `
    publish $pactPath `
    --consumer-app-version $version `
    --branch $branch `
    --broker-token=$pactBrokerToken `
    --broker-base-url=$pactBrokerBaseUrl
"@

Write-Host $dockerCommand

Write-Host $pactFileRelativePath
Write-Host $pactPath

docker run --rm -v /${dockerPath}:/${dockerPath} -w /${dockerPath} `
    -e $pactBrokerBaseUrl `
    -e $pactBrokerToken `
    pactfoundation/pact-cli:0.50.0.28 `
    publish $pactFileRelativePath `
    --consumer-app-version $version `
    --branch $branch `
    --broker-token=$pactBrokerToken `
    --broker-base-url=$pactBrokerBaseUrl
