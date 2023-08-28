#!/bin/bash

dotnet run --launch-profile http &
API_PID=$!


echo "Started dotnet API with process ID: $API_PID"

echo "Running schemathesis test to generate report"
docker run --net="host" schemathesis/schemathesis:stable run --stateful=links --checks all http://host.docker.internal:5170/swagger/v1/swagger.json > report.txt

echo "Stopping dotnet API"
kill -9 $API_PID