# Pact Broker API endpoint to publish pacts
$pactBrokerBaseUrl = ${env:PACT_BROKER_BASE_URL}
$pactBrokerToken = ${env:PACT_BROKER_TOKEN}
if (-not $PactBrokerBaseUrl -or -not $PactBrokerToken) {
    Write-Host "Pact broker base url and token are not set up."
    exit 1
}

$providerName = "WeatherForecast API"
$version = ${env:PROVIDER_API_VERSION}
$branch = ${env:PROVIDER_API_BRANCH}
$openApiFile = ".\WeatherForecastProviderApi.json"
$BRANCH=$(git rev-parse --abbrev-ref HEAD)
$OAS_PATH=$openApiFile
$REPORT_PATH="report.txt"
$REPORT_FILE_CONTENT_TYPE="text/plain"
$VERIFIER_TOOL="schemathesis"
$PWD = (Get-Location).Path
$dockerPath = $PWD -replace '\\', '/' -replace ':', ''

$dockerCommand = @"
docker run --rm -v /${dockerPath}:/${dockerPath} -w /${dockerPath} `
    -e $pactBrokerBaseUrl `
    -e $pactBrokerToken `
    pactfoundation/pact-cli:0.50.0.28 `
    pactflow publish-provider-contract $OAS_PATH `
    --provider $providerName `
    --provider-app-version $version `
    --branch $branch `
    --content-type application/json `
    --verification-exit-code 0 `
    --verification-results $REPORT_PATH `
    --verification-results-content-type $REPORT_FILE_CONTENT_TYPE `
    --verifier $VERIFIER_TOOL
    --broker-token=$pactBrokerToken
    --broker-base-url=$pactBrokerBaseUrl
"@

Write-Host $dockerCommand

docker run --rm -v /${dockerPath}:/${dockerPath} -w /${dockerPath} `
    -e $pactBrokerBaseUrl `
    -e $pactBrokerToken `
    pactfoundation/pact-cli:0.50.0.28 `
    pactflow publish-provider-contract $OAS_PATH `
    --provider $providerName `
    --provider-app-version $version `
    --branch $branch `
    --content-type application/json `
    --verification-exit-code 0 `
    --verification-results $REPORT_PATH `
    --verification-results-content-type $REPORT_FILE_CONTENT_TYPE `
    --verifier $VERIFIER_TOOL `
    --broker-token=$pactBrokerToken `
    --broker-base-url=$pactBrokerBaseUrl
