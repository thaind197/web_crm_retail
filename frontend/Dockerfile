# Stage 1: Build React App
FROM node:20-alpine AS build-stage
WORKDIR /app

# Copy package descriptors first (cache optimization)
COPY package.json package-lock.json ./
RUN npm ci

# Copy source and configurations
COPY . .
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine
COPY --from=build-stage /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf.template

EXPOSE 80
CMD ["/bin/sh", "-c", "RESOLVER_IP=$(grep -i '^nameserver' /etc/resolv.conf | head -n1 | cut -d ' ' -f2); if [ -z \"$RESOLVER_IP\" ]; then RESOLVER_IP=\"127.0.0.11\"; fi; sed \"s/DNS_RESOLVER_IP/$RESOLVER_IP/g\" /etc/nginx/nginx.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
