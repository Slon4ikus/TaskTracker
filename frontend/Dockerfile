FROM node:20 AS build
WORKDIR /app

COPY tasktracker-ui/package*.json ./
RUN npm install

COPY tasktracker-ui ./

RUN npx ng build tasktracker-ui --configuration production

FROM nginx:alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/tasktracker-ui/browser /usr/share/nginx/html

EXPOSE 80
