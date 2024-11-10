#!/bin/bash

git clone https://github.com/Jove2001/DriveHub.git
cd DriveHub
docker build -t drivehub-image -f DriveHubDocker .
docker build -t admin-image -f AdminDocker .
docker compose up -d
