# chat-serv
Chat Serv is a small chatroom service.

## Setup
To run Chat Serv some setup needs to be done on your local machine.

Please make sure that you have docker installed to run this project.

### Service HTTPS/WSS setup
A self-signed certificate needs to be created in the `${USERPROFILE}\.aspnet\https` folder on your system and the `ASPNETCORE_Kestrel__Certificates__Default__Path` and `ASPNETCORE_Kestrel__Certificates__Default__Password` updated in the `docker-compose.yml` to match the generated certificate.

### Nginx HTTPS setup
Another (or the same) self signed cert needs to be put into `./certs` as `aspnetapp.pfx` and the passwords in `./scripts/entrypoint.sh` need to be updated.

## Running

To run Chat Serv make sure ports 5002 and 5003 are free and in your terminal or choice run `docker compose up -d`.


## Future stuff?

This is just a small project, not really something I'll likely maintain. But if I decide to I do want to handle adding new rooms  on the UI (this is possible on the backend).