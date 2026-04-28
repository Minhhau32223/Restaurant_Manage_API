# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# CHỈNH SỬA TẠI ĐÂY:
# Vì Dockerfile nằm cùng thư mục với .sln và .csproj, ta copy trực tiếp
COPY RestaurantAPI.sln .
COPY RestaurantAPI.csproj .

# Restore dựa trên file csproj cho chắc chắn
RUN dotnet restore RestaurantAPI.csproj

# Copy toàn bộ code (thư mục Properties, src, Program.cs...)
COPY . .

# Build và Publish
RUN dotnet publish RestaurantAPI.csproj -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RestaurantAPI.dll"]