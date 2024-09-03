FROM node as build
WORKDIR /app
COPY . .
RUN npm install
COPY . /apps
RUN npm run build -- --configuration=production
FROM nginx:alpine
COPY nginx-custom.conf /etc/nginx/conf.d/
COPY nginx-custom.conf /etc/nginx/conf.d/default.conf

RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/dist/messenger.app/browser /usr/share/nginx/html
