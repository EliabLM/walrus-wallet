Crea la carpeta WalrusWallet y el archivo .sln
`dotnet new sln -o WalrusWallet`

Crea la webapi base
`dotnet new webapi -o WalrusWallet.Api`

Opcional
`dotnet new classlib -o WalrusWallet.Contracts`

`dotnet new classlib -o WalrusWallet.Infrastructure`

`dotnet new classlib -o WalrusWallet.Application`

`dotnet new classlib -o WalrusWallet.Domain`

Agregar proyectos a la solución de manera recursiva

`dotnet sln add $(ls -r **/*.csproj)`

Agregar referencias entre proyectos

`dotnet add ./WalrusWallet.Api/ reference ./WalrusWallet.Application/`

`dotnet add ./WalrusWallet.Infrastructure/ reference ./WalrusWallet.Application/`

`dotnet add ./WalrusWallet.Application/ reference ./WalrusWallet.Domain/`

`dotnet add ./WalrusWallet.Api/ reference ./WalrusWallet.Infrastructure/`

Para configurar inyección de dependencias se instala el siguiente paquete:

`dotnet add ./WalrusWallet.Application/ package Microsoft.Extensions.DependencyInjection.Abstractions`
