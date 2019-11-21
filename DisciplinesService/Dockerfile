#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-nanoserver-1903 AS build
WORKDIR /src
COPY ["DisciplinesService/DisciplinesService.csproj", "DisciplinesService/"]
RUN dotnet restore "DisciplinesService/DisciplinesService.csproj"
COPY . .
WORKDIR "/src/DisciplinesService"
RUN dotnet build "DisciplinesService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DisciplinesService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DisciplinesService.dll"]