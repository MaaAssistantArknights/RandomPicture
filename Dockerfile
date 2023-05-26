FROM mcr.microsoft.com/dotnet/aspnet:7.0

ARG APP_VERSION=0.0.0

WORKDIR /app

COPY ./publish /app/

RUN ["mkdir", "/app/data"]

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:80

ENV RP_APP_VERSION=$APP_VERSION
ENV RP_CONFIG_DIRECTORY=/app/config

VOLUME ["/app/config"]

EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "RandomPicture.dll"]
