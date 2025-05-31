#!/bin/bash

# Start the mock server status checker in the background
echo "Starting Mock Server Status Checker..."
cd "$(dirname "$0")"
asdf exec dotnet run --project MockServerStatusChecker &
MOCK_PID=$!

# Wait a moment for the mock server to start
sleep 2

# Start the SKELETON-KING server
echo "Starting SKELETON-KING server..."
asdf exec dotnet run --project SKELETON-KING

# When SKELETON-KING exits, kill the mock server
echo "Stopping Mock Server Status Checker..."
kill $MOCK_PID
