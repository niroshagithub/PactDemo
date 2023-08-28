# Pact Broker API endpoint to publish pacts
$pactBrokerBaseUrl = ${env:PACT_BROKER_BASE_URL}
$pactBrokerToken = ${env:PACT_BROKER_TOKEN}
if (-not $PactBrokerBaseUrl -or -not $PactBrokerToken) {
    Write-Host "Pact broker base url and token are not set up."
    exit 1
}

$providerName = "WeatherForecast API"
$version = ${env:PROVIDER_API_VERSION}
$openApiFile = ".\WeatherForecastProviderApi.json"
$pactUri = "$pactBrokerBaseUrl/pacticipants/$providerName/versions/$version/pacts"
$headers = @{
    "Authorization" = "Bearer $pactBrokerToken"
    "Content-Type" = "application/json"
}
Write-Host $pactUri
Invoke-RestMethod -Uri $pactUri -Method PUT -Headers $headers -InFile $openApiFile
