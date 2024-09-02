FROM node:latest as node
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build --prod
COPY nginx.conf /etc/nginx/nginx.conf

FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/
COPY --from=node /app/dist/messenger.app/browser  /usr/share/nginx/html