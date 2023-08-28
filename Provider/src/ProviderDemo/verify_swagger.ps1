Start-Process "dotnet" -ArgumentList "run --launch-profile http" -PassThru | ForEach-Object {
    $API_PID = $_.Id
    Write-Host "Started dotnet API with process ID: $API_PID"
}

Write-Host "Running schemathesis test to generate report"
docker run --net="host" schemathesis/schemathesis:stable run --stateful=links --checks all http://host.docker.internal:5170/swagger/v1/swagger.json > report.txt

Write-Host "Stopping dotnet API"
Stop-Process -Id $API_PID -Force
