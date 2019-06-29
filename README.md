# Kofti (Distributed Configuration Manager)
Distributed Configuration Manager for ASP.NET Core Applications

## Configure Client App

1. Install package to your project

```
dotnet add package Kofti
```

2. Add services to your service container with extensions

```
    ...
    services.Configure<KoftiOptions>(Configuration.GetSection("Kofti"));
    services.AddKofti();
    ...
```

3. Add redis and kofti configs to appsettings.json file

```
  "Kofti": {
    "ApplicationName": "KoftiExample",
    "OrchestratorName": "KoftiManager",
    "RedisServers": "localhost:6379",
    "RedisAllowAdmin": true,
    "RedisPassword": "MyP@ssw0rd"
  }
```

4. Inject 'IConfigService' and get config

```
        private readonly IConfigService _configService;
        public ValuesController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet("{key}")]
        public ActionResult<string> Get(string key)
        {
            return _configService.GetValue<string>(key, $"Not available config key: '{key}'");
        }

```

## Configure Server App (Manuel)

1. Clone this repo and open 'Kofti.Manager' project

2. Change configuration for your enviromment (database connection string, redis and kofti configs)

3. Run migrations
```
dotnet ef database update
```

4. Run or deploy application

## Dependencies

* ASP.NET Core 2.2
* Redis

## TODO

- [ ] Unit tests
- [ ] Kofti.Manager refactor and fully dockerize app
- [ ] More examples and documentation

## Contribution

* If you want to contribute to codes, create pull request
* If you find any bugs or error, create an issue

## License

This project is licensed under the MIT License

