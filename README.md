# project_form

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

## Docker setup
The project is now dockerized with:
- `form_API` running on .NET 10 and connected to SQL Server
- `project_form` built with Node and served by nginx
- `mssql` database container with persistent storage

The compose file is located in the `docker/` folder, not in the repository root.

Run from the project root:
```bash
docker compose -f docker/docker-compose.yml up --build
```

Then access:
- Frontend: http://localhost:8080
- API: http://localhost:5000

> The API will apply EF Core migrations on startup and create the `FormDB` database automatically.

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).
