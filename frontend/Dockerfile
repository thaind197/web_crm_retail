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
CMD ["/bin/sh", "-c", "if [ -z \"$BACKEND_HOST\" ]; then BACKEND_HOST=\"http://sales-crm-backend:8080\"; fi; BH_SCHEME=\"http\"; if echo \"$BACKEND_HOST\" | grep -q \"^https://\"; then BH_SCHEME=\"https\"; fi; BH_CLEAN=$(echo \"$BACKEND_HOST\" | sed 's#^https\\?://##; s#/.*##'); BH_HOST_HEADER=\"$BH_CLEAN\"; if ! echo \"$BH_CLEAN\" | grep -q \":\"; then if echo \"$BH_CLEAN\" | grep -q \"\\.onrender\\.com\"; then BH_PORT=\"443\"; BH_SCHEME=\"https\"; else BH_PORT=\"8080\"; fi; BH_CLEAN=\"$BH_CLEAN:$BH_PORT\"; fi; DNS_RESOLVER=$(grep -i '^nameserver' /etc/resolv.conf | head -n1 | awk '{print $2}'); if [ -z \"$DNS_RESOLVER\" ]; then DNS_RESOLVER=\"8.8.8.8\"; fi; sed \"s#NAMESERVER_PLACEHOLDER#$DNS_RESOLVER#g; s#BACKEND_HOST_PLACEHOLDER#$BH_CLEAN#g; s#SCHEME_PLACEHOLDER#$BH_SCHEME#g; s#HOST_HEADER_PLACEHOLDER#$BH_HOST_HEADER#g\" /etc/nginx/nginx.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
