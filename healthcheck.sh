#!/bin/bash
# Health check script for Docker container

set -e

# Check if the application is responding on the health endpoint
curl -f http://localhost:8080/health || exit 1

echo "Health check passed"