# Stage 1: Build the app
FROM node:18 AS build
WORKDIR /app

ARG VITE_BACKEND_SERVICE_ROUTE_URL
ENV VITE_BACKEND_SERVICE_ROUTE_URL=$VITE_BACKEND_SERVICE_ROUTE_URL

COPY package*.json ./
RUN npm install --legacy-peer-deps

COPY . .
RUN npm run build

# Stage 2: Serve using Nginx
FROM nginx:alpine

# Add custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built files
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
