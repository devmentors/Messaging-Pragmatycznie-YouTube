![logo](./docs/logo.png)


# Messaging: Pragmatycznie (YouTube)
To repozytorium zawiera aplikację Filo, która stanowić będzie oś dyskusji na temat komunikacji asynchronicznej w systemach rozporszonych. Nowe odcinki będą publikowane na **[naszym kanale YouTube.](https://www.youtube.com/@DevMentorsPL)**


# Lista odcinków
1. [Messaging, czyli po co nam komunikacja asynchroniczna?](https://www.youtube.com/watch?v=cA1Cpqk1Zxo)
2. [COUPLING - powiązania i zależności w systemach rozproszonych](https://www.youtube.com/watch?v=q3KOp68QwRA)
3. [ORDERING, czyli dlaczego KOLEJNOŚĆ komunikatów nie jest oczywista?](https://www.youtube.com/watch?v=IXZ_JcGlJVY)
4. [PARTYCJONOWANIE, czyli jak zapewnić kolejność przetwarzania wiadomości? | ORDERING cz. 2](https://youtu.be/hcc1fCoK29A)


## Jak uruchomić?

### RabbitMQ

RabbitMQ uruchamiamy przez dedykowany skrypt `run.sh` ponieważ wymagane jest użycie niestandardowego obrazu RabbitMQ. Przechodzimy do katalogu `compose`, a następnie w konsoli uruchamiamy polecenie:

```bash
sh ./run.sh
```

Management UI powinien być dostępny pod adrsem `localhost:15672`.

### Konsumenci

Żeby uruchomić dwóch niezależnych konsumentów z explicite wskazaną partycją, do której dołączą należy uruchomić w niezależnych oknach terminala nastepujące komendy:
```bash
dotnet build && clear && dotnet run --urls http://127.0.0.1:21370 -- PartitionNum=1
```
```bash
dotnet build && clear && dotnet run --urls http://127.0.0.1:21371 -- PartitionNum=2
```

## Tworzenie topologii Filo (RabbitMQ)
Projekt zawiera aplikację konsolową `Filo.Tools.RabbitMqTopology`, która pozwala na szybkie utworzenie topologii w RabbitMQ. Uruchom aplikację, a następnie sprawdź czy została utworzona wymiana (`files-exchange`) wraz z kolejkami dla każdej z aplikacji konsumującej komunikaty.

## Tworzenie topologii Filo (Azure Service Bus)
Do uruchomienia przykladu z sesjami z Azure Service Bus należy:
- utworzyć darmowe konto na https://portal.azure.com,
- stworzyć ("wyklikać") nowy namespace, topic `files` oraz subscription `files_archive` na tym topicu
- dodać connectionString skopiowany z portalu Azure do:
  - https://github.com/devmentors/Messaging-Pragmatycznie-YouTube/blob/4e54edd2e53efba5ed9accec2a7295f9d14482a9/src/Filo.Services.Files/appsettings.json#L13
  - https://github.com/devmentors/Messaging-Pragmatycznie-YouTube/blob/4e54edd2e53efba5ed9accec2a7295f9d14482a9/src/Filo.Services.Archive/appsettings.json#L13
