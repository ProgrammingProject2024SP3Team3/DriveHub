#!/bin/bash

sudo apt update
sudo apt upgrade -y
sudo apt install docker.io docker-buildx docker-compose-v2 docker-doc
sudo groupadd docker
sudo usermod -aG docker $USER
newgrp docker
docker build -t drivehub-image -f DriveHubDocker .
docker build -t admin-image -f AdminDocker .
docker compose up -d
