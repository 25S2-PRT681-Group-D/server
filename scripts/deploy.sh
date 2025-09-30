#!/bin/bash

# AgroScan API Deployment Script
# This script handles the deployment of the AgroScan API

set -e

# Configuration
APP_NAME="agroscan-api"
DOCKER_IMAGE="agroscan-api:latest"
DOCKER_COMPOSE_FILE="docker-compose.yml"
ENVIRONMENT=${1:-development}

echo "🚀 Starting deployment of AgroScan API..."
echo "Environment: $ENVIRONMENT"

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command_exists docker; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command_exists docker-compose; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

echo "✅ Prerequisites check passed"

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker-compose -f $DOCKER_COMPOSE_FILE down || true

# Clean up old images (optional)
echo "🧹 Cleaning up old images..."
docker image prune -f || true

# Build the application
echo "🔨 Building the application..."
docker-compose -f $DOCKER_COMPOSE_FILE build --no-cache

# Start the services
echo "🚀 Starting services..."
docker-compose -f $DOCKER_COMPOSE_FILE up -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 30

# Health check
echo "🏥 Performing health check..."
max_attempts=30
attempt=1

while [ $attempt -le $max_attempts ]; do
    if curl -f http://localhost:7001/health >/dev/null 2>&1; then
        echo "✅ Health check passed"
        break
    else
        echo "⏳ Attempt $attempt/$max_attempts - Waiting for API to be ready..."
        sleep 10
        attempt=$((attempt + 1))
    fi
done

if [ $attempt -gt $max_attempts ]; then
    echo "❌ Health check failed after $max_attempts attempts"
    echo "📋 Container logs:"
    docker-compose -f $DOCKER_COMPOSE_FILE logs agroscan-api
    exit 1
fi

# Display service status
echo "📊 Service status:"
docker-compose -f $DOCKER_COMPOSE_FILE ps

# Display useful information
echo ""
echo "🎉 Deployment completed successfully!"
echo ""
echo "📋 Service URLs:"
echo "  - API: http://localhost:7001"
echo "  - Swagger UI: http://localhost:7001/swagger"
echo "  - Hangfire Dashboard: http://localhost:7001/hangfire"
echo "  - Health Check: http://localhost:7001/health"
echo "  - Seq Logging: http://localhost:5341"
echo ""
echo "📝 Useful commands:"
echo "  - View logs: docker-compose -f $DOCKER_COMPOSE_FILE logs -f"
echo "  - Stop services: docker-compose -f $DOCKER_COMPOSE_FILE down"
echo "  - Restart services: docker-compose -f $DOCKER_COMPOSE_FILE restart"
echo ""
echo "🔍 Monitoring:"
echo "  - Check container status: docker-compose -f $DOCKER_COMPOSE_FILE ps"
echo "  - View API logs: docker-compose -f $DOCKER_COMPOSE_FILE logs agroscan-api"
echo "  - View database logs: docker-compose -f $DOCKER_COMPOSE_FILE logs sqlserver"
