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

echo "Building docker image"
docker build -t jamwiki .

echo "Running production docker build"
docker run -d -p 5000:80 --env-file env.production --name wikiapi jamwiki || { echo "Docker run failed" ; exit 1; }

echo "Production build complete and running :)"
