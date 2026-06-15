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
CMD ["/bin/sh", "-c", "RESOLVER_IP=$(grep -i '^nameserver' /etc/resolv.conf | head -n1 | cut -d ' ' -f2); if [ -z \"$RESOLVER_IP\" ]; then RESOLVER_IP=\"127.0.0.11\"; fi; if [ -z \"$BACKEND_HOST\" ]; then BACKEND_HOST=\"sales-crm-backend-cisw\"; fi; BH_NO_SCHEME=$(echo \"$BACKEND_HOST\" | sed 's#^https\\?://##; s#/.*##'); BH_PORT=\"\"; if echo \"$BH_NO_SCHEME\" | grep -q \":\"; then BH_PORT=$(echo \"$BH_NO_SCHEME\" | sed 's/.*://'); BH_HOST=$(echo \"$BH_NO_SCHEME\" | sed 's/:.*//'); else BH_HOST=\"$BH_NO_SCHEME\"; fi; SEARCH_SUFFIX=$(grep -i '^search' /etc/resolv.conf | head -n1 | awk '{print $2}'); if [ -n \"$SEARCH_SUFFIX\" ] && ! echo \"$BH_HOST\" | grep -q \"\\.\"; then BH_HOST=\"$BH_HOST.$SEARCH_SUFFIX\"; fi; if [ -z \"$BH_PORT\" ]; then if echo \"$BH_HOST\" | grep -q \"\\.onrender\\.com\"; then BH_PORT=\"80\"; else BH_PORT=\"8080\"; fi; fi; BH_CLEAN=\"$BH_HOST:$BH_PORT\"; sed \"s#DNS_RESOLVER_IP#$RESOLVER_IP#g; s#BACKEND_HOST_PLACEHOLDER#$BH_CLEAN#g\" /etc/nginx/nginx.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
