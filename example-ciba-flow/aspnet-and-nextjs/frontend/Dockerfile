FROM node:18.16.0-alpine as base
EXPOSE 3000

#install pnpm
RUN apk update && apk add --no-cache libc6-compat &&\
    corepack enable && corepack prepare pnpm@8.6.3 --activate
WORKDIR /app

FROM base as build
COPY package.json pnpm-lock.yaml ./
#install the dependencies
RUN pnpm install --frozen-lockfile

COPY . .
#starts the program
RUN pnpm build
CMD pnpm next start -p 3000
