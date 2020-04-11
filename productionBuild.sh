#!/usr/bin/env bash

cd web/  || { echo "CD into web dir failed" ; exit 1; }

echo "Running npm run build"
npm run build  || { echo "Npm run build failed" ; exit 1; }

cd ../  || { echo "CD out of web dir failed" ; exit 1; }

echo "Deleting old frontend version"
rm -r /var/www/jampot.dev/html/* || { echo "Deleting old frontend version failed" ; exit 1; }

echo "Copying dist to /var/www/jampot.dev/html"
cp -r web/dist/* /var/www/jampot.dev/html/  || { echo "Copying failed" ; exit 1; }

echo "Reloading nginx"
systemctl reload nginx || { echo "Reloading nginx failed" ; exit 1; }

echo "Removing old docker image"
docker stop wikiapi; docker rm wikiapi

echo "Building docker image"
docker build -t jamwiki . || { echo "Building docker image failed" ; exit 1; }

echo "Running production docker build"
HOSTIP=`ip -4 addr show scope global dev docker0 | grep inet | awk '{print $2}' | cut -d / -f 1 | sed -n 1p`
docker run --add-host=database:${HOSTIP} -d -p 5000:5000 --env-file env.production --name wikiapi jamwiki || { echo "Docker run failed" ; exit 1; }

echo "Production build complete and running :)"
