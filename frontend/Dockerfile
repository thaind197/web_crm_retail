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
CMD ["/bin/sh", "-c", "if [ -z \"$BACKEND_HOST\" ]; then BACKEND_HOST=\"sales-crm-backend-cisw\"; fi; BH_CLEAN=$(echo \"$BACKEND_HOST\" | sed 's#^https\\?://##; s#/.*##'); if [ \"$BH_CLEAN\" = \"sales-crm-backend\" ] || [ \"$BH_CLEAN\" = \"sales-crm-backend:8080\" ]; then BH_CLEAN=\"sales-crm-backend-cisw:8080\"; fi; if ! echo \"$BH_CLEAN\" | grep -q \":\"; then if echo \"$BH_CLEAN\" | grep -q \"\\.onrender\\.com\"; then BH_PORT=\"80\"; else BH_PORT=\"8080\"; fi; BH_CLEAN=\"$BH_CLEAN:$BH_PORT\"; fi; DNS_RESOLVER=$(grep -i '^nameserver' /etc/resolv.conf | head -n1 | awk '{print $2}'); if [ -z \"$DNS_RESOLVER\" ]; then DNS_RESOLVER=\"8.8.8.8\"; fi; sed \"s#NAMESERVER_PLACEHOLDER#$DNS_RESOLVER#g; s#BACKEND_HOST_PLACEHOLDER#$BH_CLEAN#g\" /etc/nginx/nginx.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
