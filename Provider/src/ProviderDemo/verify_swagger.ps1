Start-Process "dotnet" -ArgumentList "run --launch-profile http" -PassThru | ForEach-Object {
    $API_PID = $_.Id
    Write-Host "Started dotnet API with process ID: $API_PID"
}

Write-Host "Running schemathesis test to generate report"
docker run --net="host" schemathesis/schemathesis:stable run --stateful=links --checks all http://host.docker.internal:5170/swagger/v1/swagger.json > report.txt

Write-Host "Saving the Open Api swagger file"
$swaggerUrl = "http://localhost:5170/swagger/v1/swagger.json"  # Replace with the actual URL of your Swagger JSON
$outputPath = ".\WeatherForecastProviderApi.json"  # Replace with the desired path to save the file
$response = Invoke-WebRequest -Uri $swaggerUrl -OutFile $outputPath

Write-Host "Stopping dotnet API"
Stop-Process -Id $API_PID -Force
