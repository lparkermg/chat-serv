#! /bin/bash

mkdir /certs

openssl pkcs12 -in /external-certs/aspnetapp.pfx -passin pass:pAs5w0rd -clcerts -nokeys -out /certs/localhost.crt
openssl pkcs12 -in /external-certs/aspnetapp.pfx -passin pass:pAs5w0rd -nocerts -nodes -out /certs/localhost.rsa