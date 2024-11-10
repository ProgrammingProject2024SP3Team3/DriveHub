#!/bin/bash

docker build -t drivehub-image -f DriveHubDocker .
docker build -t admin-image -f AdminDocker .
docker compose up -d
