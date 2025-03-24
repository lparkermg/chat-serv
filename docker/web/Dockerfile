FROM node:20-alpine AS build

WORKDIR /build

COPY ./src/web .

RUN npm install
RUN npm run build

FROM nginx AS final

COPY --from=build /build/dist /usr/share/nginx/html
COPY ./scripts/nginx-local-config/default.conf /etc/nginx/conf.d/default.conf
COPY ./scripts/entrypoint.sh /entrypoint.sh

COPY ./certs/aspnetapp.pfx /external-certs/aspnetapp.pfx

RUN ["chmod", "+x", "/entrypoint.sh"]

RUN ["/entrypoint.sh"]