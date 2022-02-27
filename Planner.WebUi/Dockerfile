#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT "Testing"
ENV ConnectionStrings_DefaultConnection "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=planner_new;Pooling=true;"
ENV ConnectionStrings_MasterConnection "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=master_roomchecking;Pooling=true;"
ENV ConnectionStrings_DefaultTenantConnection "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=default_roomchecking_tenant;Pooling=true;"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm

WORKDIR /src
COPY ["Planner.WebUi/Planner.WebUi.csproj", "Planner.WebUi/"]
COPY ["Planner.Shared/Planner.Shared.csproj", "Planner.Shared/"]
COPY ["Planner.Common/Planner.Common.csproj", "Planner.Common/"]
COPY ["Planner.Persistence/Planner.Persistence.csproj", "Planner.Persistence/"]
COPY ["Planner.Application/Planner.Application.csproj", "Planner.Application/"]
COPY ["Planner.CpsatCleaningCalculator/Planner.CpsatCleaningCalculator.csproj", "Planner.CpsatCleaningCalculator/"]
COPY ["Planner.RccSynchronization/Planner.RccSynchronization.csproj", "Planner.RccSynchronization/"]
COPY ["Planner.Domain/Planner.Domain.csproj", "Planner.Domain/"]
COPY ["Planner.Infrastructure/Planner.Infrastructure.csproj", "Planner.Infrastructure/"]
RUN dotnet restore "Planner.WebUi/Planner.WebUi.csproj"
COPY . .
WORKDIR "/src/Planner.WebUi"
RUN dotnet build "Planner.WebUi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Planner.WebUi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#WORKDIR /app/wwwroot
#RUN mkdir temporary-uploads
#RUN mkdir uploaded-files
#WORKDIR /app
ENTRYPOINT ["dotnet", "Planner.WebUi.dll"]