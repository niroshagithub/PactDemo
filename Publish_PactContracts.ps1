# Pact Broker API endpoint to publish pacts
$PactBrokerBaseUrl = ${env:PACT_BROKER_BASE_URL}
$PactBrokerToken = ${env:PACT_BROKER_TOKEN}
if (-not $PactBrokerBaseUrl -or -not $PactBrokerToken) {
    Write-Host "Pact broker base url and token are not set up."
    exit 1
}
# Consumer and Provider details
$ConsumerName = "WeatherForecast Consumer"
$ProviderName = "WeatherForecast API"
$ConsumerVersion = ${env:PACT_CONSUMER_VERSION}  # Replace with the actual version

# Path to the directory containing pact files
$PactFilesDirectory = "pacts/WeatherForecast Consumer-WeatherForecast API.json"

# Loop through pact files in the directory and publish each one
Get-ChildItem -Path $PactFilesDirectory -Filter "*.json" | ForEach-Object {
    $PactFile = $_.FullName
    $PactUrl = "$PactBrokerBaseUrl/pacts/provider/$ProviderName/consumer/$ConsumerName/version/$ConsumerVersion"
    
     # Build headers with the authentication token
        $headers = @{
            "Authorization" = "Bearer $PactBrokerToken"
        }
    
    # Publish the pact file to the Pact Broker
    $result = Invoke-RestMethod -Method Put -Uri $PactUrl -InFile $PactFile -ContentType "application/json" -Headers $headers
    
    if ($result -ne $null) {
        Write-Host "Pact file '$PactFile' published successfully."
    } else {
        Write-Host "Failed to publish pact file '$PactFile'."
    }
}
