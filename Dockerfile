FROM node:18-alpine AS build
WORKDIR /app
COPY ui/package*.json ./ui/
WORKDIR /app/ui
RUN npm install
COPY ui/ .
RUN npm run build

FROM nginx:stable-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/ui/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
