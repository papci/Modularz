# Modularz

Modularz in another CMS that does nothing better than others. It is designed to be an awefull thing, just to provide another crappy CMS to the world. 
BUT : it's coded by me, designed for my own use. That's a good reason for its existence.

# I'm not you, but i want to use it anyway.

Bad Idea. You're probably a NoCoder, so go away. Really. 


Still There ?

Ok ...
Prerequisites :

1. .Net 6 SDK
2. PostgresSQL Database
3. Redis Server
4. Brain

What you need to do : 

1. Clone. (learn git)
2. Install .Net 6 SDK
3. Create an appsettings.json inside Modularz Directory. I'm pretty sure that you can't guess what to put inside, so here is an example.
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Database": "Server=my-postgresqldatabaseserver.com;Port=5432;Database=mydabatabse;User Id=user;Password=password;Include Error Detail=true;",

  "Environnement": {
    "RedisConnection": "myredisserver.com,ssl=false,password=myredispasssword,defaultDatabase=0",
    "EnvName": "MyEnvironment" 
  }
}

```
4. Open a console  and browse to your modularz directory
5. Install npm packages (npm i )
6. `npm run watch:webpack` to start static resources live builds, or `npm run build:prod` to build production ready assets.
7. Build or Launch Modularz project using `dotnet`
