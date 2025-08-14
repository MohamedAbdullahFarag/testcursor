# Use a base Node.js image
FROM node:latest AS build

# Set the working directory inside the container
WORKDIR /app


# Install pnpm globally
RUN npm install -g pnpm
RUN npm install -g git

# Install dependencies using pnpm
ARG Environment

# Copy the entire project to the working directory
COPY . .
RUN rm -rf ./public/web.config

RUN pnpm install
WORKDIR /app/Core
RUN pnpm install 

# Build the Vite application for staging
WORKDIR /app
RUN pnpm run build:$Environment

# Use a lightweight Node.js image for production
FROM node:latest

# Set the working directory inside the container
WORKDIR /app

# Copy only the necessary files from the build stage
FROM nginx:alpine
ENV NODE_ENV production
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
